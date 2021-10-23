using System.Threading.Tasks;
using Amazon.Lambda.Core;

namespace MessageListenerBase.EventHandlers
{
    public interface IEventHandler<in TInput>
    {
        Task HandleAsync(TInput input, ILambdaContext context);
    }
}