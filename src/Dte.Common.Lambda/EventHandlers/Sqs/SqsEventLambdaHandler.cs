using System;
using System.Threading.Tasks;
using Amazon.Lambda.SQSEvents;
using Dte.Common.Lambda.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dte.Common.Lambda.EventHandlers.Sqs
{
    public class SqsEventLambdaHandler : ILambdaEventHandler<SQSEvent>
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public SqsEventLambdaHandler(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory?.CreateLogger("SqsEventHandler") ?? throw new ArgumentNullException(nameof(loggerFactory));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task HandleLambdaEventAsync(SQSEvent @event)
        {
            foreach (var record in @event.Records)
            {
                using var scope = _serviceProvider.CreateScope();
                var sqsMessageBody = record.Body;
                var handlerExecutor = _serviceProvider.GetService<ISqsMessageHandlerExecutor>();
                
                if (handlerExecutor == null)
                {
                    throw new Exception("No IHandlerExecutor found");
                }

                var (handlerName, success) = await handlerExecutor.ExecuteHandlerAsync(sqsMessageBody);
                _logger.LogInformation($"**** Handle {(success ? "SUCCESS" : "FAILURE")} for handler: {handlerName}");
            }
        }
    }
}