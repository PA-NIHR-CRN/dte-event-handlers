using System;
using System.Linq;
using System.Reflection;
using Dte.Common.Lambda.Contracts;
using Newtonsoft.Json;

namespace Dte.Common.Lambda.Resolvers
{
    public class HandlerResolver : IHandlerResolver
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Assembly _assembly;

        public HandlerResolver(IServiceProvider serviceProvider, Assembly assembly)
        {
            _serviceProvider = serviceProvider;
            _assembly = assembly;
        }
        
        public (object handlerImpl, object method) ResolveHandler(string handlerType, string source)
        {
            var @interface = typeof(IHandler<,>);
            var handlerTypeInterfaces = _assembly.GetTypes().Where(t => t.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == @interface));

            var handlers = handlerTypeInterfaces
                .Select(x => x.GetInterfaces()[0])
                .Where(x => string.Equals(x.GetGenericArguments()[0].Name, handlerType, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
            
            if (!handlers.Any())
            {
                throw new Exception($"No IHandler found for: {handlerType}");
            }
	
            if(handlers.Count > 1)
            {
                throw new Exception($"More than 1 IHandler found for: {handlerType}");
            }
            
            var handler = handlers.First();
            var handlerTypeParam = handler.GetGenericArguments()[0];
            var message = JsonConvert.DeserializeObject(source, handlerTypeParam);
            var handlerImpl = _serviceProvider.GetService(handler);
	        
            var method = handlerImpl.GetType().GetMethod("HandleAsync");
            
            if (method == null)
            {
                throw new Exception($"HandleAsync method not found on : {handler.Name}");
            }

            var invoke = method.Invoke(handlerImpl, new[] { message });

            if (invoke == null)
            {
                throw new Exception("invoked method returned null");   
            }
            
            return (handlerImpl, invoke);
        }
    }
}