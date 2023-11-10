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
    public class CpmsStudyDynamoDbRepository : BaseDynamoDbRepository, ICpmsStudyDynamoDbRepository
    {
        private readonly IAmazonDynamoDB _client;
        private readonly IDynamoDBContext _context;
        private readonly DynamoDBOperationConfig _config;

        public CpmsStudyDynamoDbRepository(IAmazonDynamoDB client, IDynamoDBContext context, AwsSettings awsSettings)
            : base(client, context)
        {
            _client = client;
            _context = context;
            _config = new DynamoDBOperationConfig { OverrideTableName = awsSettings.CpmsStudyDynamoDbTableName };
        }

        public async Task BatchInsertCpmsStudyAsync(IEnumerable<CpmsStudy> entities)
        {
            await BatchInsertAsync(entities, _config);
        }
    }
}