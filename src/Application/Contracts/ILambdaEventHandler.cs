using System.Threading.Tasks;

namespace Application.Contracts
{
    public interface ILambdaEventHandler<in TInput>
    {
        Task HandleLambdaEventAsync(TInput @event);
    }
}