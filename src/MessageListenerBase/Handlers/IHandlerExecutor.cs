using System.Threading.Tasks;

namespace MessageListenerBase.Handlers
{
    public interface IHandlerExecutor
    {
        Task<(string, bool)> ExecuteHandlerAsync(string messageBody);
    }
}