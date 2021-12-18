using System;
using System.Threading.Tasks;
using Dte.Common.Lambda.Contracts;
using Dte.Common.Lambda.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dte.Common.Lambda.EventHandlers.Cognito
{
    public class CognitoCustomMessageEventLambdaHandler : ILambdaEventHandler<CognitoCustomMessageEvent>
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public CognitoCustomMessageEventLambdaHandler(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory?.CreateLogger("CognitoCustomMessageSignUpEventHandler") ?? throw new ArgumentNullException(nameof(loggerFactory));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task HandleLambdaEventAsync(CognitoCustomMessageEvent @event)
        {
            using var scope = _serviceProvider.CreateScope();
            var handlerExecutor = _serviceProvider.GetService<ICognitoMessageHandlerExecutor>();

            if (handlerExecutor == null)
            {
                throw new Exception("No IHandlerExecutor found");
            }

            var (handlerName, cognitoCustomMessage) = await handlerExecutor.ExecuteHandlerAsync(@event);
            _logger.LogInformation($"**** Handle {(cognitoCustomMessage != null ? "SUCCESS" : "FAILURE")} for handler: {handlerName}");

            if (cognitoCustomMessage == null)
            {
                throw new Exception($"{handlerExecutor.GetType().Name} could not handle Cognito Custom Message");
            }

            @event.Response = cognitoCustomMessage.Response;
        }
    }
}