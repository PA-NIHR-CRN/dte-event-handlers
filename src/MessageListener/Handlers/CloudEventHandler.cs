using System.Threading.Tasks;
using Adapter;
using Adapter.Contracts;
using Amazon.Lambda.Core;
using MessageListener.Base;
using Microsoft.Extensions.Logging;

namespace MessageListener.Handlers
{
    public class CloudEventHandler : IMessageHandler<CloudEvent>
    {
        private readonly IWorker _worker;
        private readonly ILogger<CloudEventHandler> _logger;
        

        public CloudEventHandler(IWorker worker, ILogger<CloudEventHandler> logger)
        {
            _worker = worker;
            _logger = logger;
        }
        
        public async Task HandleAsync(CloudEvent message, ILambdaContext context)
        {
            _logger.LogInformation($"Handle started for: {nameof(CloudEventHandler)}");
            
            string eventId = message.Data.ToString();
            _logger.LogInformation(eventId);
            
            _worker.Process(message);
            
            _logger.LogInformation($"Handle completed for: {nameof(CloudEventHandler)}");
            
            await Task.CompletedTask;
        }
    }
}