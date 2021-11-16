using System.Threading.Tasks;
using MessageListener.Messages;
using Application.Contracts;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MessageListener.MessageHandlers
{
    public class SendNotificationV1MessageHandler : IHandler<SendNotificationV1, bool>
    {
        private readonly ILogger<SendNotificationV1MessageHandler> _logger;
        
        public SendNotificationV1MessageHandler(ILogger<SendNotificationV1MessageHandler> logger)
        {
            _logger = logger;
        }

        public async Task<bool> HandleAsync(SendNotificationV1 source)
        {
            _logger.LogInformation($"************** {nameof(SendNotificationV1MessageHandler)} STARTED");
            _logger.LogInformation(JsonConvert.SerializeObject(source));
            await Task.Delay(1000);
            _logger.LogInformation($"************** {nameof(SendNotificationV1MessageHandler)} FINISHED");

            return true;
        }
    }
}