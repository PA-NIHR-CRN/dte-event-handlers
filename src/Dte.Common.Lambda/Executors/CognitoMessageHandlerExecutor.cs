using System;
using System.Threading.Tasks;
using Dte.Common.Lambda.Contracts;
using Dte.Common.Lambda.Events;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Dte.Common.Lambda.Executors
{
    public class CognitoMessageHandlerExecutor : ICognitoMessageHandlerExecutor
    {
        private readonly IHandlerResolver _handlerResolver;

        public CognitoMessageHandlerExecutor(IHandlerResolver handlerResolver)
        {
            _handlerResolver = handlerResolver;
        }
        
        public async Task<(string, CognitoCustomMessageEvent)> ExecuteHandlerAsync(CognitoCustomMessageEvent @event)
        {
            if (@event == null)
            {
                throw new Exception("Input is null");
            }   

            if (string.IsNullOrWhiteSpace(@event.TriggerSource))
            {
                throw new Exception("Input does not have a property called \"TriggerSource\", dont know what this Cognito Custom Message is!");
            }

            var triggerSource = @event.TriggerSource.Replace("_", string.Empty);

            var (handlerImpl, invoke) = _handlerResolver.ResolveHandler(triggerSource, JsonSerializer.Serialize(@event));

            return (handlerImpl.GetType().Name, await (Task<CognitoCustomMessageEvent>)invoke);
        }
    }
}