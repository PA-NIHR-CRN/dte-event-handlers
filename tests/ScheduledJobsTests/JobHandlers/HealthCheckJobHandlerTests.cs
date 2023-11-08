using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using ScheduledJobs.Clients;
using ScheduledJobs.JobHandlers;

namespace ScheduledJobsTests.JobHandlers;

[TestFixture]
    public class HealthCheckJobHandlerTests
    {
        private Mock<IStudyServiceClient> _mockStudyServiceClient;
        private Mock<ILogger<HealthCheckJobHandler>> _mockLogger;
        private HealthCheckJobHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _mockStudyServiceClient = new Mock<IStudyServiceClient>();
            _mockLogger = new Mock<ILogger<HealthCheckJobHandler>>();
            _handler = new HealthCheckJobHandler(_mockStudyServiceClient.Object, _mockLogger.Object);
        }

        [Test]
        public async Task HandleAsync_WhenHealthCheckSucceeds_ReturnsTrue()
        {
            // Arrange
            var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK);
            _mockStudyServiceClient.Setup(client => client.GetHealthReadyAsync()).ReturnsAsync(fakeResponse);

            // Act
            var result = await _handler.HandleAsync(new HealthCheck());

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task HandleAsync_WhenHealthCheckFails_ReturnsFalse()
        {
            // Arrange
            var fakeResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            _mockStudyServiceClient.Setup(client => client.GetHealthReadyAsync()).ReturnsAsync(fakeResponse);

            // Act
            var result = await _handler.HandleAsync(new HealthCheck());

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task HandleAsync_WhenHttpRequestExceptionThrown_ReturnsFalse()
        {
            // Arrange
            _mockStudyServiceClient.Setup(client => client.GetHealthReadyAsync()).ThrowsAsync(new HttpRequestException());

            // Act
            var result = await _handler.HandleAsync(new HealthCheck());

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task HandleAsync_WhenExceptionThrown_ReturnsFalse()
        {
            // Arrange
            _mockStudyServiceClient.Setup(client => client.GetHealthReadyAsync()).ThrowsAsync(new Exception());

            // Act
            var result = await _handler.HandleAsync(new HealthCheck());

            // Assert
            Assert.IsFalse(result);
        }
    }
