using System.Threading.Tasks;
using Dte.Common.Lambda.Events;

namespace Dte.Common.Lambda.Contracts
{
    public interface ICognitoMessageHandlerExecutor
    {
        Task<(string, CognitoCustomMessageEvent)> ExecuteHandlerAsync(CognitoCustomMessageEvent @event);
    }
}