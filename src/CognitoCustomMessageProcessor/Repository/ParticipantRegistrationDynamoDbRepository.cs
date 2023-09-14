using System.Collections.Generic;
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
        private readonly IDynamoDBContext _context;
        private readonly DynamoDBOperationConfig _config;

        public ParticipantRegistrationDynamoDbRepository(IAmazonDynamoDB client, IDynamoDBContext context,
            AwsSettings awsSettings)
            : base(client, context)
        {
            _client = client;
            _context = context;
            _config = new DynamoDBOperationConfig
                { OverrideTableName = awsSettings.ParticipantRegistrationDynamoDbTableName };
        }

        public async Task<string> GetParticipantLocaleAsync(string participantId)
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

            var response = await _client.QueryAsync(request);

            // return participant locale if found otherwise return default locale
            return response.Items.Count > 0 ? response.Items[0]["SelectedLocale"].S : "en-GB";
        }
    }
}