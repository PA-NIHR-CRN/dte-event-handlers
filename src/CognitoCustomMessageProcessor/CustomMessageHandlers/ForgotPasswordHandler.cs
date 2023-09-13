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
            var requestCodeParameter = source.Request.CodeParameter;
            var userAttributesId = HttpUtility.UrlEncode(source.Request.UserAttributes.Sub.ToString());

            var link = _linkBuilder
                .AddLink(null, $"{_appSettings.WebAppBaseUrl}resetpassword", requestCodeParameter, userAttributesId)
                .Build();

            var participant = await _repository.GetParticipantAsync(source.Request.UserAttributes.Sub.ToString());

            var request = new EmailContentRequest
            {
                EmailName = _contentfulSettings.EmailTemplates.ForgotPassword,
                Link = link,
                SelectedLocale = new CultureInfo(participant.SelectedLocale)
            };

            var contentfulEmail = await _contentfulService.GetEmailContentAsync(request);

            source.Response.EmailSubject = contentfulEmail.EmailSubject;
            source.Response.EmailMessage = contentfulEmail.EmailBody;

            return await Task.FromResult(source);
        }
    }
}