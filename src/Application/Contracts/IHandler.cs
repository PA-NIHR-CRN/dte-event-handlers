using System.Threading.Tasks;

namespace Application.Contracts
{
    public interface IHandler<in TMessage, TResponse>
    {
        Task<TResponse> HandleAsync(TMessage source);
    }
}