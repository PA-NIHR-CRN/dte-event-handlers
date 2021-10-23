using System.Threading.Tasks;
using Amazon.Lambda.Core;
using MessageListener.Base.Handlers;
using Microsoft.Extensions.Logging;

namespace MessageListener.Handlers
{
    public class SendNotificationV1Handler : IMessageHandler
    {
        private readonly ILogger<SendNotificationV1Handler> _logger;
        
        public SendNotificationV1Handler(ILogger<SendNotificationV1Handler> logger)
        {
            _logger = logger;
        }

        public string MessageType => "SendNotificationV1";

        public async Task HandleAsync(string messageBody, ILambdaContext context)
        {
            _logger.LogInformation($"Handle started for: {nameof(SendNotificationV1Handler)}");
            _logger.LogInformation(messageBody);
            // _logger.LogInformation(JsonConvert.SerializeObject(messageBase, Formatting.Indented));
            
            // Call a processor here
            
            _logger.LogInformation($"Handle completed for: {nameof(SendNotificationV1Handler)}");
            
            await Task.CompletedTask;
        }
    }
}