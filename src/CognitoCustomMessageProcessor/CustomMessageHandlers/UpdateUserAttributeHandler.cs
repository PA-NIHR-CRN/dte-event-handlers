using System.Globalization;
using System.Threading.Tasks;
using CognitoCustomMessageProcessor.Contracts;
using CognitoCustomMessageProcessor.CustomMessages;
using Dte.Common;
using Dte.Common.Contracts;
using Dte.Common.Lambda.Contracts;
using Dte.Common.Lambda.Events;
using Dte.Common.Models;

namespace CognitoCustomMessageProcessor.CustomMessageHandlers
{
    public class UpdateUserAttributeHandler : IHandler<CustomMessageUpdateUserAttribute, CognitoCustomMessageEvent>
    {
        private readonly IContentfulService _contentfulService;
        private readonly IParticipantRegistrationDynamoDbRepository _repository;
        private readonly ContentfulSettings _contentfulSettings;

        public UpdateUserAttributeHandler(IContentfulService contentfulService,
            IParticipantRegistrationDynamoDbRepository repository, ContentfulSettings contentfulSettings)
        {
            _contentfulService = contentfulService;
            _repository = repository;
            _contentfulSettings = contentfulSettings;
        }

        public async Task<CognitoCustomMessageEvent> HandleAsync(CustomMessageUpdateUserAttribute source)
        {
            var participant = await _repository.GetParticipantAsync(source.Request.UserAttributes.Sub.ToString());
            
            var request = new EmailContentRequest
            {
                EmailName = _contentfulSettings.EmailTemplates.UpdateUserAttribute,
                SelectedLocale = new CultureInfo(participant.SelectedLocale)
            };

            var contentfulEmail = await _contentfulService.GetEmailContentAsync(request);

            source.Response.EmailSubject = contentfulEmail.EmailSubject;
            source.Response.EmailMessage = contentfulEmail.EmailBody;

            return await Task.FromResult(source);
        }
    }
}