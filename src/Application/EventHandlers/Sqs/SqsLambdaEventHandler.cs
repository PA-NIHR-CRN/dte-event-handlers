using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Application.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application.EventHandlers.Sqs
{
    public class SqsLambdaEventHandler : ILambdaEventHandler<SQSEvent>
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public SqsLambdaEventHandler(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory?.CreateLogger("SqsEventHandler") ?? throw new ArgumentNullException(nameof(loggerFactory));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task HandleLambdaEventAsync(SQSEvent @event, ILambdaContext context)
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