using System;
using System.Threading;
using System.Threading.Tasks;
using CognitoCustomMessageProcessor.Contracts;
using CognitoCustomMessageProcessor.CustomMessageHandlers;
using CognitoCustomMessageProcessor.CustomMessages;
using Dte.Common;
using Dte.Common.Contracts;
using Dte.Common.Lambda.Events;
using Dte.Common.Models;
using Moq;
using NUnit.Framework;

namespace CognitoCustomMessageProcessorTests.JobHandlers;

public class UpdateUserAttributeHandlerTests
{
    private Mock<IContentfulService> _mockContentfulService;
    private Mock<IParticipantRegistrationDynamoDbRepository> _mockRepository;
    private ContentfulSettings _contentfulSettings;
    private UpdateUserAttributeHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _mockContentfulService = new Mock<IContentfulService>();
        _mockRepository = new Mock<IParticipantRegistrationDynamoDbRepository>();
        _contentfulSettings = new ContentfulSettings
        {
            EmailTemplates = new EmailTemplates { UpdateUserAttribute = "UpdateUserAttributeTemplate" }
        };

        _handler = new UpdateUserAttributeHandler(
            _mockContentfulService.Object,
            _mockRepository.Object,
            _contentfulSettings
        );
    }

    [Test]
    public async Task HandleAsync_ShouldFetchContentBasedOnLocaleAndSetEmailDetails()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var customMessageUpdateUserAttribute = new CustomMessageUpdateUserAttribute
        {
            Request = new Request
            {
                CodeParameter = "updatecode123",
                UserAttributes = new UserAttributes { Sub = guid }
            },
            Response = new Response()
        };

        var expectedLocale = "en-US";
        _mockRepository.Setup(repo => repo.GetParticipantLocaleAsync(guid.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedLocale);

        var emailContent = new ContentfulEmailResponse
        {
            EmailSubject = "Your account details have been updated",
            EmailBody = "Your account details have been successfully updated."
        };
        _mockContentfulService.Setup(cs =>
                cs.GetEmailContentAsync(It.Is<EmailContentRequest>(req =>
                    req.Code == "updatecode123" && req.SelectedLocale.ToString() == expectedLocale)))
            .ReturnsAsync(emailContent);

        // Act
        var result = await _handler.HandleAsync(customMessageUpdateUserAttribute);

        // Assert
        Assert.IsNotNull(result.Response.EmailMessage, "The EmailMessage should not be null.");
        Assert.IsInstanceOf<string>(result.Response.EmailMessage, "The EmailMessage should be a string.");
        Assert.AreEqual(emailContent.EmailSubject, result.Response.EmailSubject,
            "The EmailSubject should match the contentful service response.");
        Assert.AreEqual(emailContent.EmailBody, result.Response.EmailMessage,
            "The EmailMessage should match the contentful service response.");
        _mockRepository.VerifyAll();
        _mockContentfulService.VerifyAll();
    }
}
