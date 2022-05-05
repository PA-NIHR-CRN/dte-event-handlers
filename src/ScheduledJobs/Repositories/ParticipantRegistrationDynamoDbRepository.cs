using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Dte.Common.Persistence;
using ScheduledJobs.Contracts;
using ScheduledJobs.Domain;
using ScheduledJobs.Models;
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

        public async Task<IEnumerable<Participant>> GetAllAsync()
        {
            return await _context.ScanAsync<Participant>(null, _config).GetRemainingAsync();
        }
    }
}