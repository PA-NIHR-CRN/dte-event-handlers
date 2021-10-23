using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Common.Extensions;
using MessageListenerBase.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MessageListenerBase.EventHandlers
{
    public class ParallelSqsEventHandler : IEventHandler<SQSEvent>
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ParallelSqsExecutionOptions _options;

        public ParallelSqsEventHandler(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, IOptions<ParallelSqsExecutionOptions> options)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = loggerFactory?.CreateLogger("SqsForEachAsyncEventHandler") ?? throw new ArgumentNullException(nameof(loggerFactory));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task HandleAsync(SQSEvent input, ILambdaContext context)
        {
            if (input.Records.Any())
            {
                await input.Records.ForEachAsync(_options.MaxDegreeOfParallelism, async record =>
                {
                    using var scope = _serviceProvider.CreateScope();
                    var sqsMessage = record.Body;
                    var handlerExecutor = _serviceProvider.GetService<IHandlerExecutor>();

                    if (handlerExecutor == null)
                    {
                        throw new Exception("No IHandlerExecutor found");
                    }

                    var (handlerName, success) = await handlerExecutor.ExecuteHandlerAsync(sqsMessage);
                    _logger.LogInformation($"**** Handle {(success ? "SUCCESS" : "FAILURE")} for handler: {handlerName}");
                });
            }
        }
    }
}