using System.Threading.Tasks;

namespace Dte.Common.Lambda.Contracts
{
    public interface ILambdaEventHandler<in TInput>
    {
        Task HandleLambdaEventAsync(TInput @event);
    }
}