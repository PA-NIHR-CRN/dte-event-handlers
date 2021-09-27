using System.Threading.Tasks;
using Adapter;
using Amazon.Lambda.Core;
using MessageListener.Base;
using MessageListener.Settings;
using Microsoft.Extensions.Logging;

namespace MessageListener.Handlers
{
    public class CloudEventHandler : IMessageHandler<CloudEvent>
    {
        private readonly AppSettings _appSettings;
        private readonly IWorker _worker;
        private readonly ILogger<CloudEventHandler> _logger;
        

        public CloudEventHandler(AppSettings appSettings, IWorker worker, ILogger<CloudEventHandler> logger)
        {
            _appSettings = appSettings;
            _worker = worker;
            _logger = logger;
        }
        
        public async Task HandleAsync(CloudEvent message, ILambdaContext context)
        {
            string eventId = message.Data.ToString();
            _logger.LogInformation(eventId);
            _logger.LogInformation($"App setting: {_appSettings.SomeAppSetting}");
            
            _worker.Process(message);
            
            await Task.CompletedTask;
        }
    }
}