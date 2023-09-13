using System.Globalization;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using CognitoCustomMessageProcessor.Contracts;
using CognitoCustomMessageProcessor.Domain;
using CognitoCustomMessageProcessor.Settings;
using Dte.Common.Persistence;

namespace CognitoCustomMessageProcessor.Repository
{
    public class ParticipantRegistrationDynamoDbRepository : BaseDynamoDbRepository, IParticipantRegistrationDynamoDbRepository
    {
        private readonly IAmazonDynamoDB _client;
        private readonly IDynamoDBContext _context;
        private readonly DynamoDBOperationConfig _config;

        public ParticipantRegistrationDynamoDbRepository(IAmazonDynamoDB client, IDynamoDBContext context, AwsSettings awsSettings)
            : base(client, context)
        {
            _client = client;
            _context = context;
            _config = new DynamoDBOperationConfig { OverrideTableName = awsSettings.ParticipantRegistrationDynamoDbTableName };
        }

        public async Task<Participant> GetParticipantAsync(string participantId)
        {
            var participant = await _context.LoadAsync<Participant>(participantId, _config);
            participant.SelectedLocale = new CultureInfo(participant.SelectedLocale).TwoLetterISOLanguageName;
            return participant;
        }
    }
}