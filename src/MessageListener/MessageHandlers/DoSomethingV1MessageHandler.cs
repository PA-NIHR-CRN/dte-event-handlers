using System.Threading.Tasks;
using MessageListener.Messages;
using Dte.Common.Lambda.Contracts;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MessageListener.MessageHandlers
{
    public class DoSomethingV1MessageHandler : IHandler<DoSomethingV1, bool>
    {
        private readonly ILogger<DoSomethingV1MessageHandler> _logger;

        public DoSomethingV1MessageHandler(ILogger<DoSomethingV1MessageHandler> logger)
        {
            _logger = logger;
        }
        
        public async Task<bool> HandleAsync(DoSomethingV1 source)
        {
            _logger.LogInformation($"************** {nameof(DoSomethingV1MessageHandler)} STARTED");
            _logger.LogInformation(JsonConvert.SerializeObject(source));
            await Task.Delay(1000);
            _logger.LogInformation($"************** {nameof(DoSomethingV1MessageHandler)} FINISHED");

            return true;
        }   
    }
}