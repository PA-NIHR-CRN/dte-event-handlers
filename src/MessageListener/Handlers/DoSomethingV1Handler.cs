using System.Threading.Tasks;
using MessageListener.Messages;
using MessageListenerBase.Handlers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MessageListener.Handlers
{
    public class DoSomethingV1Handler : IHandler<DoSomethingV1, bool>
    {
        private readonly ILogger<DoSomethingV1Handler> _logger;

        public DoSomethingV1Handler(ILogger<DoSomethingV1Handler> logger)
        {
            _logger = logger;
        }
        
        public async Task<bool> HandleAsync(DoSomethingV1 message)
        {
            _logger.LogInformation("************** DoSomethingV1Handler STARTED");
            _logger.LogInformation(JsonConvert.SerializeObject(message));
            await Task.Delay(1000);
            _logger.LogInformation("************** DoSomethingV1Handler FINISHED");

            return true;
        }
    }
}