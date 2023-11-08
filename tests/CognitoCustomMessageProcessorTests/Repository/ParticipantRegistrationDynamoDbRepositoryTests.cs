using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using CognitoCustomMessageProcessor.Repository;
using CognitoCustomMessageProcessor.Settings;
using Moq;
using NUnit.Framework;

namespace CognitoCustomMessageProcessorTests.Repository;

public class ParticipantRegistrationDynamoDbRepositoryTests
{
    private Mock<IAmazonDynamoDB> _mockDynamoDbClient;
    private Mock<IDynamoDBContext> _mockDynamoDbContext;
    private AwsSettings _awsSettings;
    private ParticipantRegistrationDynamoDbRepository _repository;

    [SetUp]
    public void SetUp()
    {
        _mockDynamoDbClient = new Mock<IAmazonDynamoDB>();
        _mockDynamoDbContext = new Mock<IDynamoDBContext>();
        _awsSettings = new AwsSettings { ParticipantRegistrationDynamoDbTableName = "ParticipantRegistrationTable" };

        _repository = new ParticipantRegistrationDynamoDbRepository(
            _mockDynamoDbClient.Object, _mockDynamoDbContext.Object, _awsSettings);
    }

    [Test]
    public async Task GetParticipantLocaleAsync_ReturnsLocale_WhenParticipantExists()
    {
        // Arrange
        var participantId = Guid.NewGuid().ToString();
        var expectedLocale = "en-GB";
        var queryResponse = new QueryResponse
        {
            Items = new List<Dictionary<string, AttributeValue>>
            {
                new Dictionary<string, AttributeValue>
                {
                    { "SelectedLocale", new AttributeValue { S = expectedLocale } }
                }
            }
        };

        _mockDynamoDbClient.Setup(client => client.QueryAsync(It.IsAny<QueryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryResponse);

        // Act
        var locale = await _repository.GetParticipantLocaleAsync(participantId);

        // Assert
        Assert.AreEqual(expectedLocale, locale);
        _mockDynamoDbClient.Verify(client => client.QueryAsync(It.Is<QueryRequest>(req =>
            req.TableName == _awsSettings.ParticipantRegistrationDynamoDbTableName &&
            req.KeyConditionExpression == "PK = :participantId" &&
            req.ExpressionAttributeValues[":participantId"].S == $"PARTICIPANT#{participantId}"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetParticipantLocaleAsync_ReturnsDefaultLocale_WhenParticipantDoesNotExist()
    {
        // Arrange
        var participantId = "nonexistent-participant-id";
        var queryResponse = new QueryResponse
        {
            Items = new List<Dictionary<string, AttributeValue>>()
        };

        _mockDynamoDbClient.Setup(client => client.QueryAsync(It.IsAny<QueryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryResponse);

        // Act
        var locale = await _repository.GetParticipantLocaleAsync(participantId);

        // Assert
        Assert.AreEqual("en-GB", locale);
        _mockDynamoDbClient.Verify(client => client.QueryAsync(It.Is<QueryRequest>(req =>
            req.TableName == _awsSettings.ParticipantRegistrationDynamoDbTableName &&
            req.KeyConditionExpression == "PK = :participantId" &&
            req.ExpressionAttributeValues[":participantId"].S == $"PARTICIPANT#{participantId}"), It.IsAny<CancellationToken>()), Times.Once);
    }
}
