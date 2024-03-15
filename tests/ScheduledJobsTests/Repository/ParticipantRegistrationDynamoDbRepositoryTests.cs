using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Moq;
using NUnit.Framework;
using ScheduledJobs.Repositories;
using ScheduledJobs.Settings;

namespace ScheduledJobsTests.Repository;

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

}
