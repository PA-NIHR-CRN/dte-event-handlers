using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Application.Contracts;
using Application.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application.EventHandlers.Cognito
{
    public class CognitoCustomMessageLambdaEventHandler : ILambdaEventHandler<CognitoCustomMessageEvent>
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public CognitoCustomMessageLambdaEventHandler(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory?.CreateLogger("CognitoCustomMessageSignUpEventHandler") ?? throw new ArgumentNullException(nameof(loggerFactory));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task HandleLambdaEventAsync(CognitoCustomMessageEvent @event, ILambdaContext context)
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