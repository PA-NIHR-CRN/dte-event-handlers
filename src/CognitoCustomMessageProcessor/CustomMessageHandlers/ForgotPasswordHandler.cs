using System.Threading.Tasks;
using Application.Contracts;
using Application.Events;
using CognitoCustomMessageProcessor.Content;
using CognitoCustomMessageProcessor.Contracts;
using CognitoCustomMessageProcessor.CustomMessages;
using Microsoft.Extensions.Logging;

namespace CognitoCustomMessageProcessor.CustomMessageHandlers
{
    public class ForgotPasswordHandler : IHandler<CustomMessageForgotPassword, CognitoCustomMessageEvent>
    {
        private readonly ILinkBuilder _linkBuilder;
        private readonly ILogger<ForgotPasswordHandler> _logger;

        public ForgotPasswordHandler(ILinkBuilder linkBuilder, ILogger<ForgotPasswordHandler> logger)
        {
            _linkBuilder = linkBuilder;
            _logger = logger;
        }
        
        public async Task<CognitoCustomMessageEvent> HandleAsync(CustomMessageForgotPassword source)
        {
            _logger.LogInformation($"************** {nameof(ForgotPasswordHandler)} STARTED");
            
            var links = _linkBuilder
                .AddLink("Reset your password on localhost", "http://localhost:3000/resetpassword", source.Request.CodeParameter, source.Request.UserAttributes.Email)
                .AddLink("Reset your password on dev", "https://nihr-dev.dte-pilot.net/resetpassword", source.Request.CodeParameter, source.Request.UserAttributes.Email)
                .AddLink("Reset your password on qa", "https://nihr-qa.dte-pilot.net/resetpassword", source.Request.CodeParameter, source.Request.UserAttributes.Email)
                .AddLink("Reset your password on uat", "https://nihr-uat.dte-pilot.net/resetpassword", source.Request.CodeParameter, source.Request.UserAttributes.Email)
                .Build();
            
            source.Response.EmailSubject = $"hi, from ForgotPassword";
            source.Response.EmailMessage = CustomMessageEmail.GetCustomMessageHtml()
                .Replace("###TEXT_REPLACE1###", "You have asked us to reset your password.")
                .Replace("###TEXT_REPLACE2###", "Please click the link below to reset your password")
                .Replace("###LINK_REPLACE###", links);
            
            _logger.LogInformation($"************** {nameof(ForgotPasswordHandler)} FINISHED");
            
            return await Task.FromResult(source);
        }
    }
}