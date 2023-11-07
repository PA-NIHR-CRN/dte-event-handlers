using System.Globalization;
using System.Threading.Tasks;
using CognitoCustomMessageProcessor.Contracts;
using CognitoCustomMessageProcessor.CustomMessages;
using Dte.Common;
using Dte.Common.Contracts;
using Dte.Common.Lambda.Contracts;
using Dte.Common.Lambda.Events;
using Dte.Common.Models;
using Microsoft.Extensions.Logging;

namespace CognitoCustomMessageProcessor.CustomMessageHandlers
{
    public class UpdateUserAttributeHandler : IHandler<CustomMessageUpdateUserAttribute, CognitoCustomMessageEvent>
    {
        private readonly IContentfulService _contentfulService;
        private readonly IParticipantRegistrationDynamoDbRepository _repository;
        private readonly ContentfulSettings _contentfulSettings;
        private readonly ILogger<UpdateUserAttributeHandler> _logger;

        public UpdateUserAttributeHandler(IContentfulService contentfulService,
            IParticipantRegistrationDynamoDbRepository repository, ContentfulSettings contentfulSettings, ILogger<UpdateUserAttributeHandler> logger)
        {
            _contentfulService = contentfulService;
            _repository = repository;
            _contentfulSettings = contentfulSettings;
            _logger = logger;
        }

        public async Task<CognitoCustomMessageEvent> HandleAsync(CustomMessageUpdateUserAttribute source)
        {
            _logger.LogInformation("**** Getting participant locale from DynamoDB table with user sub: {UserSub}",
                source.Request.UserAttributes.Sub);
            var participantLocale =
                await _repository.GetParticipantLocaleAsync(source.Request.UserAttributes.Sub.ToString());
            _logger.LogInformation("**** Participant locale: {ParticipantLocale}", participantLocale);

            _logger.LogInformation("**** Getting email content from Contentful with email name: {EmailName} and locale: {SelectedLocale}",
                _contentfulSettings.EmailTemplates.UpdateUserAttribute, participantLocale);
            var request = new EmailContentRequest
            {
                EmailName = _contentfulSettings.EmailTemplates.UpdateUserAttribute,
                SelectedLocale = new CultureInfo(participantLocale)
            };

            var contentfulEmail = await _contentfulService.GetEmailContentAsync(request);
            _logger.LogInformation("**** Contentful email: {@ContentfulEmail}", contentfulEmail);

            source.Response.EmailSubject = contentfulEmail.EmailSubject;
            _logger.LogInformation("**** Email subject: {EmailSubject}", source.Response.EmailSubject);
            source.Response.EmailMessage = contentfulEmail.EmailBody;
            _logger.LogInformation("**** Email message: {EmailMessage}", source.Response.EmailMessage);

            return await Task.FromResult(source);
        }
    }
}
