using System.Threading.Tasks;
using MessageListener.Messages;
using MessageListenerBase.Handlers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MessageListener.Handlers
{
    public class SendNotificationV1Handler : IHandler<SendNotificationV1, bool>
    {
        private readonly ILogger<SendNotificationV1Handler> _logger;
        
        public SendNotificationV1Handler(ILogger<SendNotificationV1Handler> logger)
        {
            _logger = logger;
        }

        public async Task<bool> HandleAsync(SendNotificationV1 message)
        {
            _logger.LogInformation("************** SendNotificationV1Handler STARTED");
            _logger.LogInformation(JsonConvert.SerializeObject(message));
            await Task.Delay(1000);
            _logger.LogInformation("************** SendNotificationV1Handler FINISHED");

            return true;
        }
    }
}