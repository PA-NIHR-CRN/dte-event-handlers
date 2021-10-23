using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using MessageListener.Base.Messages;
using MessageListener.Factories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MessageListener.Base.Handlers
{
    public class SqsEventHandler<TMessage> : IEventHandler<SQSEvent> where TMessage : class
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
            _logger.LogInformation($"**** TMessage: {typeof(TMessage).Name}");

            if (typeof(TMessage) == typeof(ManualSqsQueueUrlMessage))
            {
                var scope = _serviceProvider.CreateScope();
                var factory = scope.ServiceProvider.GetService<IMessageHandlerFactory>();
                if (factory == null) throw new Exception("Message handler factory");
                var handler = factory.Create("ManualMessage");
                await handler.HandleAsync(input.Records[0].Body, context).ConfigureAwait(false);
                
                return;
            }
            
            foreach (var record in input.Records)
            {
                using var scope = _serviceProvider.CreateScope();
                var sqsMessage = record.Body;
                var message = JsonConvert.DeserializeObject<MessageBase>(sqsMessage);
                if (message == null) throw new Exception("Message base is null");
                var factory = scope.ServiceProvider.GetService<IMessageHandlerFactory>();
                if (factory == null) throw new Exception("Message handler factory");
                var handler = factory.Create(message.Type);
                await handler.HandleAsync(sqsMessage, context).ConfigureAwait(false);
            }
        }
    }
}