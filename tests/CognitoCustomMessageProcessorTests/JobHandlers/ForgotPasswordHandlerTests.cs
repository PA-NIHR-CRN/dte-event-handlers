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

[TestFixture]
public class ForgotPasswordHandlerTests
{
    private Mock<ILinkBuilder> _mockLinkBuilder;
    private Mock<IContentfulService> _mockContentfulService;
    private Mock<IParticipantRegistrationDynamoDbRepository> _mockRepository;
    private AppSettings _appSettings;
    private ContentfulSettings _contentfulSettings;
    private ForgotPasswordHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _mockLinkBuilder = new Mock<ILinkBuilder>();
        _mockContentfulService = new Mock<IContentfulService>();
        _mockRepository = new Mock<IParticipantRegistrationDynamoDbRepository>();
        _appSettings = new AppSettings { WebAppBaseUrl = "http://example.com/" };
        _contentfulSettings = new ContentfulSettings
        {
            EmailTemplates = new EmailTemplates { ForgotPassword = "ForgotPasswordTemplate" }
        };

        _handler = new ForgotPasswordHandler(
            _mockLinkBuilder.Object,
            _appSettings,
            _mockContentfulService.Object,
            _mockRepository.Object,
            _contentfulSettings
        );
    }

    [Test]
    public async Task HandleAsync_ShouldGenerateCorrectLinkAndFetchContent()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var customMessageForgotPassword = new CustomMessageForgotPassword
        {
            Request = new Request
            {
                CodeParameter = "code123",
                UserAttributes = new UserAttributes { Sub = guid }
            },
            Response = new Response()
        };

        var expectedLink = $"{_appSettings.WebAppBaseUrl}resetpassword?code=code123&user={guid}";
        _mockLinkBuilder.Setup(lb => lb.AddLink(null, It.Is<string>(url => url.StartsWith(_appSettings.WebAppBaseUrl)),
                "code123", guid.ToString()))
            .Returns(_mockLinkBuilder.Object);
        _mockLinkBuilder.Setup(lb => lb.Build()).Returns(expectedLink);

        var expectedLocale = "en-GB";
        _mockRepository.Setup(repo => repo.GetParticipantLocaleAsync(guid.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedLocale);

        var emailContent = new ContentfulEmailResponse
        {
            EmailSubject = "Reset your password",
            EmailBody = $"Please use the following link to reset your password: {expectedLink}"
        };
        _mockContentfulService.Setup(cs =>
                cs.GetEmailContentAsync(It.Is<EmailContentRequest>(req =>
                    req.Link == expectedLink && req.SelectedLocale.ToString() == expectedLocale)))
            .ReturnsAsync(emailContent);

        // Act
        var result = await _handler.HandleAsync(customMessageForgotPassword);

        // Assert
        Assert.IsNotNull(result.Response.EmailMessage, "The EmailMessage should not be null.");
        Assert.IsInstanceOf<string>(result.Response.EmailMessage, "The EmailMessage should be a string.");
        StringAssert.Contains(expectedLink, result.Response.EmailMessage as string,
            "The EmailMessage should contain the link.");
        Assert.AreEqual(emailContent.EmailSubject, result.Response.EmailSubject);
        Assert.AreEqual(emailContent.EmailBody, result.Response.EmailMessage);
        _mockLinkBuilder.VerifyAll();
        _mockContentfulService.VerifyAll();
        _mockRepository.VerifyAll();
    }
}
