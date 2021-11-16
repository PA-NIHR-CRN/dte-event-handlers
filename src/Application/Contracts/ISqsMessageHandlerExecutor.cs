using System.Threading.Tasks;

namespace Application.Contracts
{
    public interface ISqsMessageHandlerExecutor
    {
        Task<(string, bool)> ExecuteHandlerAsync(string messageBody);
    }
}