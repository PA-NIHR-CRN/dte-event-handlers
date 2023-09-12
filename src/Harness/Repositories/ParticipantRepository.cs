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
        _logger.LogInformation("Starting insertion of participants");

        const int batchSize = 25;
        var batch = new List<Participant>(batchSize);
        int counter = 0;

        foreach (var participant in participants)
        {
            batch.Add(participant);

            if (batch.Count == batchSize)
            {
                await WriteBatchAsync(batch, counter);
                counter += batchSize;
                batch.Clear();
            }
        }

        if (batch.Any())
        {
            await WriteBatchAsync(batch, counter);
        }

        _logger.LogInformation("Finished insertion of participants");
    }


    private async Task WriteBatchAsync(List<Participant> batch, int startIndex)
    {
        _logger.LogInformation("Writing batch {BatchNumber} of participants {StartIndex} to {EndIndex}",
            (startIndex / batch.Count) + 1,
            startIndex + 1,
            startIndex + batch.Count);

        var batchWrite = _context.CreateBatchWrite<Participant>(_config);

        foreach (var participant in batch)
        {
            batchWrite.AddPutItem(participant);
        }

        try
        {
            await batchWrite.ExecuteAsync();
            _logger.LogInformation("Successfully written a batch of {BatchCount} participants", batch.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while writing batch of {BatchCount} participants", batch.Count);
        }
    }

    public async Task<int> GetTotalParticipants()
    {
        _logger.LogInformation("Fetching total participants count...");

        var search = _context.ScanAsync<Participant>(null, _config);
        int totalCount = 0;

        do
        {
            var batch = await search.GetNextSetAsync();
            totalCount += batch.Count;
        } while (!search.IsDone);

        _logger.LogInformation("Total participants count: {TotalCount}", totalCount);
        return totalCount;
    }
}