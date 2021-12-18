using System.Threading.Tasks;
using System.Web;
using Dte.Common.Lambda.Contracts;
using Dte.Common.Lambda.Events;
using CognitoCustomMessageProcessor.Content;
using CognitoCustomMessageProcessor.Contracts;
using CognitoCustomMessageProcessor.CustomMessages;
using Microsoft.Extensions.Logging;

namespace CognitoCustomMessageProcessor.CustomMessageHandlers
{
    public class ResendCodeHandler : IHandler<CustomMessageResendCode, CognitoCustomMessageEvent>
    {
        private readonly ILinkBuilder _linkBuilder;
        private readonly ILogger<ResendCodeHandler> _logger;

        public ResendCodeHandler(ILinkBuilder linkBuilder, ILogger<ResendCodeHandler> logger)
        {
            _linkBuilder = linkBuilder;
            _logger = logger;
        }
        
        public async Task<CognitoCustomMessageEvent> HandleAsync(CustomMessageResendCode source)
        {
            _logger.LogInformation($"************** {nameof(ResendCodeHandler)} STARTED");

            var requestCodeParameter = source.Request.CodeParameter;
            var userAttributesEmail = HttpUtility.UrlEncode(source.Request.UserAttributes.Email);
            
            var links = _linkBuilder
                .AddLink("Verify on localhost", "http://localhost:3000/verify", requestCodeParameter, userAttributesEmail)
                .AddLink("Verify on dev", "https://nihr-dev.dte-pilot.net/verify", requestCodeParameter, userAttributesEmail)
                .AddLink("Verify on qa", "https://nihr-qa.dte-pilot.net/verify", requestCodeParameter, userAttributesEmail)
                .AddLink("Verify on uat", "https://nihr-uat.dte-pilot.net/verify", requestCodeParameter, userAttributesEmail)
                .Build();
            
            source.Response.EmailSubject = $"hi, from ResendCode";
            source.Response.EmailMessage = CustomMessageEmail.GetCustomMessageHtml()
                .Replace("###TEXT_REPLACE1###", "You have asked us to resend a verification email.")
                .Replace("###TEXT_REPLACE2###", "Please click the link below to verify your account")
                .Replace("###LINK_REPLACE###", links);
            
            _logger.LogInformation($"************** {nameof(ResendCodeHandler)} FINISHED");
            
            return await Task.FromResult(source);
        }
    }
}