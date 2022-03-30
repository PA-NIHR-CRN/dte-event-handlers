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
    public class UpdateUserAttributeHandler : IHandler<CustomMessageUpdateUserAttribute, CognitoCustomMessageEvent>
    {
        private readonly ILinkBuilder _linkBuilder;
        private readonly AppSettings _appSettings;
        private readonly ILogger<UpdateUserAttributeHandler> _logger;

        public UpdateUserAttributeHandler(ILinkBuilder linkBuilder, AppSettings appSettings, ILogger<UpdateUserAttributeHandler> logger)
        {
            _linkBuilder = linkBuilder;
            _appSettings = appSettings;
            _logger = logger;
        }
        
        public async Task<CognitoCustomMessageEvent> HandleAsync(CustomMessageUpdateUserAttribute source)
        {
            var requestCodeParameter = source.Request.CodeParameter;
            var userAttributesEmail = HttpUtility.UrlEncode(source.Request.UserAttributes.Email);
            
            var links = _linkBuilder
                .AddLink("VerifyEmail", $"{_appSettings.DteWebBaseUrl}verifyemail", "0", userAttributesEmail)
                .Build();
            
            source.Response.EmailSubject = "Be Part of Research email address updated";
            source.Response.EmailMessage = CustomMessageEmail.GetCustomMessageHtml()
                .Replace("###TITLE_REPLACE1###", "Updated email address")
                .Replace("###TEXT_REPLACE1###", "Thank you for updating your email address. You will need to use your updated email address when you sign in. We will use this updated email address to contact you about studies and research you may be interested in.")
                .Replace("###TEXT_REPLACE2###", "Thank you for your ongoing commitment and support.")
                .Replace("###LINK_REPLACE###", null)
                .Replace("###TEXT_REPLACE3###", null);
            
            return await Task.FromResult(source);
        }
    }
}