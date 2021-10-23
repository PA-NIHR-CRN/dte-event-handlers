using System.Threading.Tasks;
using Amazon.Lambda.Core;

namespace MessageListener.Base.Handlers
{
    public interface IMessageHandler
    {
        string MessageType { get; }
        Task HandleAsync(string messageBody, ILambdaContext context);
    }
}