using System.Threading.Tasks;
using Dte.Common.Lambda.Contracts;
using Dte.Common.Lambda.Events;
using CognitoCustomMessageProcessor.Contracts;
using CognitoCustomMessageProcessor.CustomMessages;
using CognitoCustomMessageProcessor.Settings;
using ScheduledJobs.Contracts;

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
            var contentfulEmail = await _contentfulService.GetEmailContentAsync(
                _contentfulSettings.EmailTemplates.UpdateUserAttribute,
                participant.SelectedLocale);

            source.Response.EmailSubject = contentfulEmail.EmailSubject;
            source.Response.EmailMessage = contentfulEmail.EmailBody;

            return await Task.FromResult(source);
        }
    }
}