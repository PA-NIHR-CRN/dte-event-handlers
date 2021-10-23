using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using MessageListenerBase.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MessageListenerBase.EventHandlers
{
    public class SqsEventHandler : IEventHandler<SQSEvent>
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public SqsEventHandler(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory?.CreateLogger("SqsEventHandler") ?? throw new ArgumentNullException(nameof(loggerFactory));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task HandleAsync(SQSEvent input, ILambdaContext context)
        {
            foreach (var record in input.Records)
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
            }
        }
        
        
    }
}