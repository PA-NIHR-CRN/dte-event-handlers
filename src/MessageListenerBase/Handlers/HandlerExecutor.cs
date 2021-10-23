using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MessageListenerBase.Messages;
using Newtonsoft.Json;

namespace MessageListenerBase.Handlers
{
    public class HandlerExecutor : IHandlerExecutor
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Assembly _assembly;

        public HandlerExecutor(IServiceProvider serviceProvider, Assembly assembly)
        {
            _serviceProvider = serviceProvider;
            _assembly = assembly;
        }
        
        public async Task<(string, bool)> ExecuteHandlerAsync(string messageBody)
        {
            var messageBase = JsonConvert.DeserializeObject<MessageBase>(messageBody);
                
            if (messageBase == null)
            {
                throw new Exception("MessageBase is null");
            }
            
            if (string.IsNullOrWhiteSpace(messageBase.Type)) throw new Exception("MessageBase does not have a property called \"Type\", dont know what this message is!");
            
            var @interface = typeof(IHandler<,>);
            var handlerTypeInterfaces = _assembly.GetTypes().Where(t => t.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == @interface));

            var handlers = handlerTypeInterfaces
                .Select(x => x.GetInterfaces()[0])
                .Where(x => string.Equals(x.GetGenericArguments()[0].Name, messageBase.Type, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
            
            if (!handlers.Any())
            {
                throw new Exception($"No IHandler found for: {messageBase.Type}");
            }
	
            if(handlers.Count > 1)
            {
                throw new Exception($"More than 1 IHandler found for: {messageBase.Type}");
            }
            
            var handler = handlers.First();
            var handlerTypeParam = handler.GetGenericArguments()[0];
            var message = (Message)JsonConvert.DeserializeObject(messageBody, handlerTypeParam);
            var handlerImpl = _serviceProvider.GetService(handler);
	
            var method = handlerImpl.GetType().GetMethod("HandleAsync");
            
            if (method == null)
            {
                throw new Exception($"HandleAsync method not found on : {handler.Name}");
            }

            var invoke = method.Invoke(handlerImpl, new object[] { message });

            if (invoke == null)
            {
                throw new Exception("invoked method returned null");   
            }

            return (handlerImpl.GetType().Name, await (Task<bool>)invoke);
        }
    }
}