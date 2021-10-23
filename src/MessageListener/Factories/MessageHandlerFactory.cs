using System;
using System.Collections.Generic;
using System.Linq;
using MessageListener.Base.Handlers;

namespace MessageListener.Factories
{
    public class MessageHandlerFactory : IMessageHandlerFactory
    {
        private readonly IEnumerable<IMessageHandler> _handlers;

        public MessageHandlerFactory(IEnumerable<IMessageHandler> handlers)
        {
            _handlers = handlers;
        }
        
        public IMessageHandler Create(string messageType)
        {
            var handler = _handlers.FirstOrDefault(x => x.MessageType == messageType);

            if (handler == null)
            {
                throw new Exception($"Message handler for {messageType} not found");
            }

            return handler;
        }
    }
}