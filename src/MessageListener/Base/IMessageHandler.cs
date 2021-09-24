using System.Threading.Tasks;
using Amazon.Lambda.Core;

namespace MessageListener.Base
{
    public interface IMessageHandler<in TMessage> where TMessage : class
    {
        Task HandleAsync(TMessage message, ILambdaContext context);
    }
}