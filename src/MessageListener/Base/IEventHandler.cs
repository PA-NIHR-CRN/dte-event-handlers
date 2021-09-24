using System.Threading.Tasks;
using Amazon.Lambda.Core;

namespace MessageListener.Base
{
    public interface IEventHandler<in TInput>
    {
        Task HandleAsync(TInput input, ILambdaContext context);
    }
}