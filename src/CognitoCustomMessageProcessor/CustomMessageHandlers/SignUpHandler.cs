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
    public class SignUpHandler : IHandler<CustomMessageSignUp, CognitoCustomMessageEvent>
    {
        private readonly ILinkBuilder _linkBuilder;
        private readonly AppSettings _appSettings;
        private readonly ILogger<SignUpHandler> _logger;

        public SignUpHandler(ILinkBuilder linkBuilder, AppSettings appSettings, ILogger<SignUpHandler> logger)
        {
            _linkBuilder = linkBuilder;
            _appSettings = appSettings;
            _logger = logger;
        }
        
        public async Task<CognitoCustomMessageEvent> HandleAsync(CustomMessageSignUp source)
        {
            var requestCodeParameter = source.Request.CodeParameter;
            var userAttributesEmail = HttpUtility.UrlEncode(source.Request.UserAttributes.Email);
            
            var links = _linkBuilder
                .AddLink("Verify", $"{_appSettings.DteWebBaseUrl}verify", requestCodeParameter, userAttributesEmail)
                .Build();
            
            source.Response.EmailSubject = $"hi, from SignUp";
            source.Response.EmailMessage = CustomMessageEmail.GetCustomMessageHtml()
                .Replace("###TEXT_REPLACE1###", "Welcome to Be Part of Research.")
                .Replace("###TEXT_REPLACE2###", "Please click the link below to verify your account")
                .Replace("###LINK_REPLACE###", links);
            
            return await Task.FromResult(source);
        }
    }
}