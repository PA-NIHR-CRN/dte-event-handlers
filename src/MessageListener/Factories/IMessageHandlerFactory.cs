using MessageListener.Base.Handlers;

namespace MessageListener.Factories
{
    public interface IMessageHandlerFactory
    {
        IMessageHandler Create(string messageType);
    }
}