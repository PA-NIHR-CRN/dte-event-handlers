using System.Globalization;
using System.Threading.Tasks;
using System.Web;
using CognitoCustomMessageProcessor.Contracts;
using CognitoCustomMessageProcessor.CustomMessages;
using Dte.Common;
using Dte.Common.Contracts;
using Dte.Common.Lambda.Contracts;
using Dte.Common.Lambda.Events;
using Dte.Common.Models;

namespace CognitoCustomMessageProcessor.CustomMessageHandlers
{
    public class SignUpHandler : IHandler<CustomMessageSignUp, CognitoCustomMessageEvent>
    {
        private readonly ILinkBuilder _linkBuilder;
        private readonly AppSettings _appSettings;
        private readonly IContentfulService _contentfulService;
        private readonly ContentfulSettings _contentfulSettings;

        public SignUpHandler(ILinkBuilder linkBuilder, AppSettings appSettings, IContentfulService contentfulService,
            ContentfulSettings contentfulSettings)
        {
            _linkBuilder = linkBuilder;
            _appSettings = appSettings;
            _contentfulService = contentfulService;
            _contentfulSettings = contentfulSettings;
        }

        public async Task<CognitoCustomMessageEvent> HandleAsync(CustomMessageSignUp source)
        {
            var requestCodeParameter = source.Request.CodeParameter;
            var userAttributesId = HttpUtility.UrlEncode(source.Request.UserAttributes.Sub.ToString());

            var link = _linkBuilder
                .AddLink(null, $"{_appSettings.WebAppBaseUrl}verify", requestCodeParameter, userAttributesId)
                .Build();

            var selectedLocale = source.Request.ClientMetadata["selectedLocale"] ?? "en-GB";

            var request = new EmailContentRequest
            {
                EmailName = _contentfulSettings.EmailTemplates.SignUp,
                Link = link,
                SelectedLocale = new CultureInfo(selectedLocale)
            };

            var contentfulEmail = await _contentfulService.GetEmailContentAsync(request);

            source.Response.EmailSubject = contentfulEmail.EmailSubject;
            source.Response.EmailMessage = contentfulEmail.EmailBody;

            return await Task.FromResult(source);
        }
    }
}