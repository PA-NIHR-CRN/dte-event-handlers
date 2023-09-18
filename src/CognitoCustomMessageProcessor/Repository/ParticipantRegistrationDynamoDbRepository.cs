using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using CognitoCustomMessageProcessor.Contracts;
using CognitoCustomMessageProcessor.Settings;
using Dte.Common.Persistence;

namespace CognitoCustomMessageProcessor.Repository
{
    public class ParticipantRegistrationDynamoDbRepository : BaseDynamoDbRepository,
        IParticipantRegistrationDynamoDbRepository
    {
        private readonly IAmazonDynamoDB _client;
        private readonly DynamoDBOperationConfig _config;

        public ParticipantRegistrationDynamoDbRepository(IAmazonDynamoDB client, IDynamoDBContext context,
            AwsSettings awsSettings)
            : base(client, context)
        {
            _client = client;
            _config = new DynamoDBOperationConfig
                { OverrideTableName = awsSettings.ParticipantRegistrationDynamoDbTableName };
        }

        public async Task<string> GetParticipantLocaleAsync(string participantId, CancellationToken cancellationToken = default)
        {
            // query dynamodb for participant locale
            var request = new QueryRequest
            {
                TableName = _config.OverrideTableName,
                KeyConditionExpression = "PK = :participantId",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":participantId", new AttributeValue { S = $"PARTICIPANT#{participantId}" } }
                },
                ProjectionExpression = "SelectedLocale"
            };

            var response = await _client.QueryAsync(request, cancellationToken);

            // Check if the key 'SelectedLocale' exists in the dictionary before accessing it
            return response?.Items?.FirstOrDefault()?.GetValueOrDefault("SelectedLocale")?.S ?? "en-GB";
        }
    }
}