using System;
using System.Threading.Tasks;
using Dte.Common.Lambda.Contracts;
using Dte.Common.Lambda.Messages;
using Newtonsoft.Json;

namespace Dte.Common.Lambda.Executors
{
    public class SqsMessageHandlerExecutor : ISqsMessageHandlerExecutor
    {
        private readonly IHandlerResolver _handlerResolver;

        public SqsMessageHandlerExecutor(IHandlerResolver handlerResolver)
        {
            _handlerResolver = handlerResolver;
        }
        
        public async Task<(string, bool)> ExecuteHandlerAsync(string messageBody)
        {
            var messageBase = JsonConvert.DeserializeObject<MessageBase>(messageBody);
                
            if (messageBase == null)
            {
                throw new Exception("MessageBase is null");
            }

            if (string.IsNullOrWhiteSpace(messageBase.Type))
            {
                throw new Exception("MessageBase does not have a property called \"Type\", dont know what this message is!");
            }
            
            var (handlerImpl, invoke) = _handlerResolver.ResolveHandler(messageBase.Type, messageBody);

            return (handlerImpl.GetType().Name, await (Task<bool>)invoke);
        }
    }
}