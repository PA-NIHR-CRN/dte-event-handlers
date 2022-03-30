using System.Threading.Tasks;
using System.Web;
using Dte.Common.Lambda.Contracts;
using Dte.Common.Lambda.Events;
using CognitoCustomMessageProcessor.Content;
using CognitoCustomMessageProcessor.Contracts;
using CognitoCustomMessageProcessor.CustomMessages;
using CognitoCustomMessageProcessor.Settings;
using Microsoft.Extensions.Logging;

namespace CognitoCustomMessageProcessor.CustomMessageHandlers
{
    public class ResendCodeHandler : IHandler<CustomMessageResendCode, CognitoCustomMessageEvent>
    {
        private readonly ILinkBuilder _linkBuilder;
        private readonly AppSettings _appSettings;
        private readonly ILogger<ResendCodeHandler> _logger;

        public ResendCodeHandler(ILinkBuilder linkBuilder, AppSettings appSettings, ILogger<ResendCodeHandler> logger)
        {
            _linkBuilder = linkBuilder;
            _appSettings = appSettings;
            _logger = logger;
        }
        
        public async Task<CognitoCustomMessageEvent> HandleAsync(CustomMessageResendCode source)
        {
            var requestCodeParameter = source.Request.CodeParameter;
            var userAttributesEmail = HttpUtility.UrlEncode(source.Request.UserAttributes.Email);
            
            var links = _linkBuilder
                .AddLink("Verify", $"{_appSettings.DteWebBaseUrl}verify", requestCodeParameter, userAttributesEmail)
                .Build();
            
            source.Response.EmailSubject = "Be Part of Research email verification";
            source.Response.EmailMessage = CustomMessageEmail.GetCustomMessageHtml()
                .Replace("###TITLE_REPLACE1###", "Confirm your email address")
                .Replace("###TEXT_REPLACE1###", "You have asked us to resend a verification email.")
                .Replace("###TEXT_REPLACE2###", "Please click the link below to verify your account")
                .Replace("###LINK_REPLACE###", links);
            
            return await Task.FromResult(source);
        }
    }
}