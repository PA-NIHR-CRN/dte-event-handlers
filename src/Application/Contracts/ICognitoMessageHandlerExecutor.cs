using System.Threading.Tasks;
using Application.Events;

namespace Application.Contracts
{
    public interface ICognitoMessageHandlerExecutor
    {
        Task<(string, CognitoCustomMessageEvent)> ExecuteHandlerAsync(CognitoCustomMessageEvent @event);
    }
}