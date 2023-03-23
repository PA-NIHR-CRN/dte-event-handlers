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
            var userAttributesId = HttpUtility.UrlEncode(source.Request.UserAttributes.Sub.ToString());

            var link = _linkBuilder
                .AddLink(null, $"{_appSettings.DteWebBaseUrl}verify", requestCodeParameter, userAttributesId)
                .Build();
            
            source.Response.EmailSubject = "Be Part of Research email verification";
            source.Response.EmailMessage = CustomMessageEmail.GetCustomMessageHtml()
                .Replace("###TITLE_REPLACE1###", "Confirm your email address")
                .Replace("###TEXT_REPLACE1###", "Thank you for your interest in Be Part of Research. By signing up, you are joining our community of amazing volunteers who are helping researchers to understand more about health and care conditions. And as a result, you are playing an important part in helping us all to live healthier and better lives, now and in the future.")
                .Replace("###TEXT_REPLACE2###", "Confirm your email address and continue your registration by clicking the link.")
                .Replace("###LINK_REPLACE###", link)
                .Replace("###LINK_DISPLAY_VALUE_REPLACE###", "block")
                .Replace("###TEXT_REPLACE3###", "After 24 hours this link will not work.");
            
            return await Task.FromResult(source);
        }
    }
}