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
    public class SignUpHandler : IHandler<CustomMessageSignUp, CognitoCustomMessageEvent>
    {
        private readonly ILinkBuilder _linkBuilder;
        private readonly ILogger<SignUpHandler> _logger;

        public SignUpHandler(ILinkBuilder linkBuilder, ILogger<SignUpHandler> logger)
        {
            _linkBuilder = linkBuilder;
            _logger = logger;
        }
        
        public async Task<CognitoCustomMessageEvent> HandleAsync(CustomMessageSignUp source)
        {
            _logger.LogInformation($"************** {nameof(SignUpHandler)} STARTED");

            var requestCodeParameter = source.Request.CodeParameter;
            var userAttributesEmail = HttpUtility.UrlEncode(source.Request.UserAttributes.Email);
            
            var links = _linkBuilder
                .AddLink("Verify on localhost", "http://localhost:3000/verify", requestCodeParameter, userAttributesEmail)
                .AddLink("Verify on dev", "https://nihr-dev.dte-pilot.net/verify", requestCodeParameter, userAttributesEmail)
                .AddLink("Verify on qa", "https://nihr-qa.dte-pilot.net/verify", requestCodeParameter, userAttributesEmail)
                .AddLink("Verify on uat", "https://nihr-uat.dte-pilot.net/verify", requestCodeParameter, userAttributesEmail)
                .Build();
            
            source.Response.EmailSubject = $"hi, from SignUp";
            source.Response.EmailMessage = CustomMessageEmail.GetCustomMessageHtml()
                .Replace("###TEXT_REPLACE1###", "Welcome to Be Part of Research.")
                .Replace("###TEXT_REPLACE2###", "Please click the link below to verify your account")
                .Replace("###LINK_REPLACE###", links);
            
            _logger.LogInformation($"************** {nameof(SignUpHandler)} FINISHED");
            
            return await Task.FromResult(source);
        }
    }
}