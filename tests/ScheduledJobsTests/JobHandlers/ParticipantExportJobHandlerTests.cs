using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using ScheduledJobs.Contracts;
using ScheduledJobs.Domain;
using ScheduledJobs.JobHandlers;
using ScheduledJobs.Models;
using ScheduledJobs.Settings;

namespace ScheduledJobsTests.JobHandlers;

[TestFixture]
public class ParticipantExportJobHandlerTests
{
    private Mock<IS3Service> _mockS3Service;
    private Mock<ICsvUtilities> _mockCsvUtilities;
    private Mock<IParticipantRegistrationDynamoDbRepository> _mockRepository;
    private Mock<ILogger<ParticipantExportJobHandler>> _mockLogger;
    private ParticipantExportSettings _participantExportSettings;
    private AwsSettings _awsSettings;
    private ParticipantExportJobHandler _handler;

    [SetUp]
    public void Setup()
    {
        _mockS3Service = new Mock<IS3Service>();
        _mockCsvUtilities = new Mock<ICsvUtilities>();
        _mockRepository = new Mock<IParticipantRegistrationDynamoDbRepository>();
        _mockLogger = new Mock<ILogger<ParticipantExportJobHandler>>();
        _participantExportSettings = new ParticipantExportSettings { S3BucketName = "test-bucket" };
        _awsSettings = new AwsSettings { ParticipantRegistrationDynamoDbTableName = "test-table" };

        _handler = new ParticipantExportJobHandler(
            _mockS3Service.Object,
            _mockCsvUtilities.Object,
            _mockRepository.Object,
            _awsSettings,
            _participantExportSettings,
            _mockLogger.Object);
    }

    [Test]
    public async Task HandleAsync_SuccessfulExport_InvokesAllDependenciesAndReturnsTrue()
    {
        // Arrange
        var cancellationToken = new CancellationToken(false);
        var fakeStream = new MemoryStream();
        _mockRepository.Setup(repo => repo.GetAllAsync(cancellationToken)).Returns(GetFakeParticipants());
        _mockCsvUtilities
            .Setup(csv => csv.WriteCsvToStreamAsync(It.IsAny<IAsyncEnumerable<ParticipantExportModel>>(), fakeStream, cancellationToken))
            .Returns(Task.CompletedTask);
        _mockS3Service
            .Setup(s3 => s3.SaveStreamContentAsync(_participantExportSettings.S3BucketName, It.IsAny<string>(), fakeStream, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(new ParticipantExport());

        // Assert
        Assert.IsTrue(result);
        VerifyAllDependenciesCalledOnce(cancellationToken);
    }

    [Test]
    public async Task HandleAsync_RepositoryThrowsException_LogsErrorAndReturnsFalse()
    {
        // Arrange
        var cancellationToken = new CancellationToken(false);
        _mockRepository.Setup(repo => repo.GetAllAsync(cancellationToken)).Throws(new Exception("Test exception"));

        // Act
        var result = await _handler.HandleAsync(new ParticipantExport());

        // Assert
        Assert.IsFalse(result);
    }

    private void VerifyAllDependenciesCalledOnce(CancellationToken cancellationToken)
    {
        _mockRepository.Verify(repo => repo.GetAllAsync(cancellationToken), Times.Once);
        _mockCsvUtilities.Verify(
            csv => csv.WriteCsvToStreamAsync(It.IsAny<IAsyncEnumerable<ParticipantExportModel>>(), It.IsAny<Stream>(), cancellationToken),
            Times.Once);
        _mockS3Service.Verify(
            s3 => s3.SaveStreamContentAsync(
                _participantExportSettings.S3BucketName,
                It.Is<string>(fileName => fileName.StartsWith("participant-export-")),
                It.IsAny<Stream>(),
                cancellationToken),
            Times.Once);
    }

    private IAsyncEnumerable<Participant> GetFakeParticipants()
    {
        var participants = new List<Participant>
        {
            new()
            {
                ParticipantId = "1",
                Email = "test1@example.com",
                Firstname = "John",
                Lastname = "Doe",
                ConsentRegistration = true,
                ConsentRegistrationAtUtc = DateTime.UtcNow.AddDays(-10),
                RemovalOfConsentRegistrationAtUtc = null,
                MobileNumber = "1234567890",
                LandlineNumber = "0987654321",
                Address = new ParticipantAddressModel
                {
                    AddressLine1 = "123 Main St",
                    AddressLine2 = "Apt 4",
                    AddressLine3 = "Business District",
                    AddressLine4 = "Suite 500",
                    Town = "Anytown",
                    Postcode = "12345"
                },
                DateOfBirth = new DateTime(1980, 1, 1),
                SexRegisteredAtBirth = "M",
                GenderIsSameAsSexRegisteredAtBirth = true,
                EthnicGroup = "GroupA",
                EthnicBackground = "BackgroundA",
                Disability = false,
                DisabilityDescription = "None",
                HealthConditionInterests = new List<string> { "ConditionA", "ConditionB" },
                CreatedAtUtc = DateTime.UtcNow.AddDays(-20),
                UpdatedAtUtc = DateTime.UtcNow.AddDays(-5)
            },
        };

        return AsyncEnumerable(participants);

        async IAsyncEnumerable<Participant> AsyncEnumerable(IEnumerable<Participant> participants)
        {
            foreach (var participant in participants)
            {
                yield return participant;
            }
        }
    }
}
