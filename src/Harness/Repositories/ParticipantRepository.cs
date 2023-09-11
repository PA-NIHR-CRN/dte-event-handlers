using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Dte.Common.Persistence;
using Harness.Contracts;
using ScheduledJobs.Domain;
using ScheduledJobs.Settings;

namespace Harness.Repositories;

public class ParticipantRepository : BaseDynamoDbRepository, IParticipantRepository

{
    private readonly IAmazonDynamoDB _client;
    private readonly IDynamoDBContext _context;
    private readonly ILogger<ParticipantRepository> _logger;
    private readonly DynamoDBOperationConfig _config;

    public ParticipantRepository(IAmazonDynamoDB client, IDynamoDBContext context, AwsSettings awsSettings,
        ILogger<ParticipantRepository> logger) : base(
        client, context)
    {
        _client = client;
        _context = context;
        _logger = logger;
        _config = new DynamoDBOperationConfig
            { OverrideTableName = awsSettings.ParticipantRegistrationDynamoDbTableName };
    }


    public async Task InsertAllAsync(IEnumerable<Participant> participants)
    {
        const int batchSize = 25;
        var batch = new List<Participant>(batchSize);
    
        foreach (var participant in participants)
        {
            batch.Add(participant);
        
            if (batch.Count == batchSize)
            {
                await WriteBatchAsync(batch);
                batch.Clear();
            }
        }

        if (batch.Any())
        {
            await WriteBatchAsync(batch);
        }
    }

    private async Task WriteBatchAsync(List<Participant> batch)
    {
        var batchWrite = _context.CreateBatchWrite<Participant>(_config);
    
        foreach (var participant in batch)
        {
            batchWrite.AddPutItem(participant);
        }

        try
        {
            await batchWrite.ExecuteAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while writing batch");
        }
    }


    public Task<int> GetTotalParticipants()
    {
        var scan = _context.ScanAsync<Participant>(new List<ScanCondition>(), _config);
        return scan.GetRemainingAsync().ContinueWith(x => x.Result.Count);
    }
}