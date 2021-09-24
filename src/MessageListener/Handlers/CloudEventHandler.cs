using System.Threading.Tasks;
using Amazon.Lambda.Core;
using MessageListener.Base;
using MessageListener.Settings;
using Microsoft.Extensions.Logging;

namespace MessageListener.Handlers
{
    public class CloudEventHandler : IMessageHandler<CloudEventMessage>
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger<CloudEventHandler> _logger;

        public CloudEventHandler(AppSettings appSettings, ILogger<CloudEventHandler> logger)
        {
            _appSettings = appSettings;
            _logger = logger;
        }
        
        public async Task HandleAsync(CloudEventMessage message, ILambdaContext context)
        {
            string eventId = message.Data.ToString();
            _logger.LogInformation(eventId);
            _logger.LogInformation($"App setting: {_appSettings.SomeAppSetting}");
            
            await Task.CompletedTask;
        }
    }
}