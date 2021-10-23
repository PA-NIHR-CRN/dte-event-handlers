using System.Threading.Tasks;
using Amazon.Lambda.Core;
using MessageListener.Base.Handlers;
using Microsoft.Extensions.Logging;

namespace MessageListener.Handlers
{
    public class StudyAddedV1Handler : IMessageHandler
    {
        private readonly ILogger<StudyAddedV1Handler> _logger;

        public StudyAddedV1Handler(ILogger<StudyAddedV1Handler> logger)
        {
            _logger = logger;
        }

        public string MessageType => "StudyAddedV1";

        public async Task HandleAsync(string messageBody, ILambdaContext context)
        {
            _logger.LogInformation($"Handle started for: {nameof(SendNotificationV1Handler)}");
            _logger.LogInformation(messageBody);
        }
    }
}