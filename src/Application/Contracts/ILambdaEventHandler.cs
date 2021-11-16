using System.Threading.Tasks;
using Amazon.Lambda.Core;

namespace Application.Contracts
{
    public interface ILambdaEventHandler<in TInput>
    {
        Task HandleLambdaEventAsync(TInput @event, ILambdaContext context);
    }
}