using System.Threading.Tasks;

namespace MessageListenerBase.Handlers
{
    public interface IHandler<in TMessage, TResponse>
    {
        Task<TResponse> HandleAsync(TMessage message);
    }
}