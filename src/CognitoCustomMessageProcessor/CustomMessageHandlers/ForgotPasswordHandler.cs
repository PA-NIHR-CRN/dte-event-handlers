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
    public class ForgotPasswordHandler : IHandler<CustomMessageForgotPassword, CognitoCustomMessageEvent>
    {
        private readonly ILinkBuilder _linkBuilder;
        private readonly AppSettings _appSettings;
        private readonly ILogger<ForgotPasswordHandler> _logger;

        public ForgotPasswordHandler(ILinkBuilder linkBuilder, AppSettings appSettings, ILogger<ForgotPasswordHandler> logger)
        {
            _linkBuilder = linkBuilder;
            _appSettings = appSettings;
            _logger = logger;
        }
        
        public async Task<CognitoCustomMessageEvent> HandleAsync(CustomMessageForgotPassword source)
        {
            var requestCodeParameter = source.Request.CodeParameter;
            var userAttributesEmail = HttpUtility.UrlEncode(source.Request.UserAttributes.Email);
            
            var links = _linkBuilder
                .AddLink("Reset your password", $"{_appSettings.DteWebBaseUrl}resetpassword", requestCodeParameter, userAttributesEmail)
                .Build();
            
            source.Response.EmailSubject = "Be Part of Research password reset";
            source.Response.EmailMessage = CustomMessageEmail.GetCustomMessageHtml()
                .Replace("###TITLE_REPLACE1###", "Password reset")
                .Replace("###TEXT_REPLACE1###", "You have asked us to reset your password.")
                .Replace("###TEXT_REPLACE2###", "Please click the link below to reset your password")
                .Replace("###LINK_REPLACE###", links);
            
            return await Task.FromResult(source);
        }
    }
}