using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using ScheduledJobs.Contracts;
using ScheduledJobs.Models;
using ScheduledJobs.Settings;

namespace ScheduledJobs.Repositories
{
    public class CpmsStudyDynamoDbRepository : ICpmsStudyDynamoDbRepository
    {
        private readonly IAmazonDynamoDB _client;
        private readonly IDynamoDBContext _context;
        private readonly DynamoDBOperationConfig _config;

        public CpmsStudyDynamoDbRepository(IAmazonDynamoDB client, IDynamoDBContext context, AwsSettings awsSettings)
        {
            _client = client;
            _context = context;
            _config = new DynamoDBOperationConfig { OverrideTableName = awsSettings.CpmsStudyDynamoDbTableName };
        }

        public async Task BatchInsertCpmsStudyAsync(IEnumerable<CpmsStudy> entities)
        {
            var aggBatchWriter = _context.CreateBatchWrite<CpmsStudy>(_config);
            aggBatchWriter.AddPutItems(entities);
            await aggBatchWriter.ExecuteAsync();
        }
    }
}