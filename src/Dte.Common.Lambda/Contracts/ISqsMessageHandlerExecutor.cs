using System.Threading.Tasks;

namespace Dte.Common.Lambda.Contracts
{
    public interface ISqsMessageHandlerExecutor
    {
        Task<(string, bool)> ExecuteHandlerAsync(string messageBody);
    }
}