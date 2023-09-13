using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Dte.Common.Persistence;
using ScheduledJobs.Contracts;
using ScheduledJobs.Domain;
using ScheduledJobs.Settings;

namespace ScheduledJobs.Repositories
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

        public Task<IAsyncEnumerable<Participant>> GetAllAsync()
        {
            return Task.FromResult(GetAllAsyncEnumerable());
        }

        public async IAsyncEnumerable<TModel> GetAllMappedAsync<TModel>(Func<Participant, TModel> mapper)
        {
            await foreach (var participant in GetAllAsyncEnumerable())
            {
                yield return mapper(participant);
            }
        }

        private async IAsyncEnumerable<Participant> GetAllAsyncEnumerable()
        {
            var search = _context.ScanAsync<Participant>(null, _config);

            while (!search.IsDone)
            {
                var page = await search.GetNextSetAsync();
                foreach (var item in page)
                {
                    yield return item;
                }
            }
        }


        public async Task<Participant> GetParticipantAsync(string participantId)
        {
            var participant = await _context.LoadAsync<Participant>(participantId, _config);
            participant.SelectedLocale = new CultureInfo(participant.SelectedLocale).TwoLetterISOLanguageName;
            return participant;
        }
    }
}