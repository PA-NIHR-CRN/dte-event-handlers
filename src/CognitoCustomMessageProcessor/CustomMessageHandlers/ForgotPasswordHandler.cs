using System.Globalization;
using System.Threading.Tasks;
using System.Web;
using Dte.Common.Lambda.Contracts;
using Dte.Common.Lambda.Events;
using CognitoCustomMessageProcessor.Contracts;
using CognitoCustomMessageProcessor.CustomMessages;
using Dte.Common;
using Dte.Common.Contracts;
using Dte.Common.Models;

namespace CognitoCustomMessageProcessor.CustomMessageHandlers
{
    public class ForgotPasswordHandler : IHandler<CustomMessageForgotPassword, CognitoCustomMessageEvent>
    {
        private readonly ILinkBuilder _linkBuilder;
        private readonly AppSettings _appSettings;
        private readonly IContentfulService _contentfulService;
        private readonly IParticipantRegistrationDynamoDbRepository _repository;
        private readonly ContentfulSettings _contentfulSettings;

        public ForgotPasswordHandler(ILinkBuilder linkBuilder, AppSettings appSettings,
            IContentfulService contentfulService, IParticipantRegistrationDynamoDbRepository repository,
            ContentfulSettings contentfulSettings)
        {
            _linkBuilder = linkBuilder;
            _appSettings = appSettings;
            _contentfulService = contentfulService;
            _repository = repository;
            _contentfulSettings = contentfulSettings;
        }

        public async Task<CognitoCustomMessageEvent> HandleAsync(CustomMessageForgotPassword source)
        {
            var requestCodeParameter = "100";
            var userAttributesId = "555";

            var link =
                $"{_appSettings.WebAppBaseUrl}resetpassword?code={requestCodeParameter}&userId={userAttributesId}";

            var participantLocale =
                await _repository.GetParticipantLocaleAsync(source.Request.UserAttributes.Sub.ToString());

            var request = new EmailContentRequest
            {
                EmailName = _contentfulSettings.EmailTemplates.ForgotPassword,
                Link = $"<a href={link}>{link}</a>",
                SelectedLocale = new CultureInfo(participantLocale)
            };

            var contentfulEmail = await _contentfulService.GetEmailContentAsync(request);

            source.Response.EmailSubject = contentfulEmail.EmailSubject;
            source.Response.EmailMessage = contentfulEmail.EmailBody;

            return await Task.FromResult(source);
        }
    }
}