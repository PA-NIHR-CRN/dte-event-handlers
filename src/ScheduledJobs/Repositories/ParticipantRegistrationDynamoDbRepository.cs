using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Dte.Common.Persistence;
using ScheduledJobs.Contracts;
using ScheduledJobs.Domain;
using ScheduledJobs.Settings;

namespace ScheduledJobs.Repositories
{
    public class ParticipantRegistrationDynamoDbRepository : BaseDynamoDbRepository,
        IParticipantRegistrationDynamoDbRepository
    {
        private readonly IDynamoDBContext _context;
        private readonly DynamoDBOperationConfig _config;

        public ParticipantRegistrationDynamoDbRepository(IAmazonDynamoDB client, IDynamoDBContext context,
            AwsSettings awsSettings)
            : base(client, context)
        {
            _context = context;
            _config = new DynamoDBOperationConfig
                { OverrideTableName = awsSettings.ParticipantRegistrationDynamoDbTableName };
        }

        public async IAsyncEnumerable<Participant> GetAllAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var search = _context.ScanAsync<Participant>(null, _config);

            while (!search.IsDone)
            {
                var page = await search.GetNextSetAsync(cancellationToken);
                foreach (var item in page)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    yield return item;
                }
            }
        }

        public async Task<Participant> GetParticipantAsync(string participantId,
            CancellationToken cancellationToken = default)
        {
            var participant = await _context.LoadAsync<Participant>(participantId, _config, cancellationToken);
            participant.SelectedLocale = new CultureInfo(participant.SelectedLocale).TwoLetterISOLanguageName;
            return participant;
        }
    }
}