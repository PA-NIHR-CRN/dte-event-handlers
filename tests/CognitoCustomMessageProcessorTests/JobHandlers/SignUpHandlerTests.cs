using System;
using System.Collections.Generic;
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

public class SignUpHandlerTests
{
    private Mock<ILinkBuilder> _mockLinkBuilder;
    private Mock<IContentfulService> _mockContentfulService;
    private AppSettings _appSettings;
    private ContentfulSettings _contentfulSettings;
    private SignUpHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _mockLinkBuilder = new Mock<ILinkBuilder>();
        _mockContentfulService = new Mock<IContentfulService>();
        _appSettings = new AppSettings { WebAppBaseUrl = "http://example.com/" };
        _contentfulSettings = new ContentfulSettings
        {
            EmailTemplates = new EmailTemplates { SignUp = "SignUpTemplate" }
        };

        _handler = new SignUpHandler(
            _mockLinkBuilder.Object,
            _appSettings,
            _mockContentfulService.Object,
            _contentfulSettings
        );
    }

    [Test]
    public async Task HandleAsync_ShouldGenerateCorrectLinkAndFetchContent()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var customMessageSignUp = new CustomMessageSignUp
        {
            Request = new Request
            {
                CodeParameter = "signupcode123",
                UserAttributes = new UserAttributes { Sub = guid },
                ClientMetadata = new Dictionary<string, string> { { "selectedLocale", "en-GB" } }
            },
            Response = new Response()
        };

        var expectedLink = $"{_appSettings.WebAppBaseUrl}verify?code=signupcode123&user={guid}";
        _mockLinkBuilder.Setup(lb => lb.AddLink(null, It.Is<string>(url => url.StartsWith(_appSettings.WebAppBaseUrl)),
                "signupcode123", guid.ToString()))
            .Returns(_mockLinkBuilder.Object);
        _mockLinkBuilder.Setup(lb => lb.Build()).Returns(expectedLink);

        var emailContent = new ContentfulEmailResponse
        {
            EmailSubject = "Welcome to our service!",
            EmailBody = $"Please use the following link to verify your account: {expectedLink}"
        };
        _mockContentfulService.Setup(cs =>
                cs.GetEmailContentAsync(It.Is<EmailContentRequest>(req =>
                    req.Link == expectedLink && req.SelectedLocale.ToString() == "en-GB")))
            .ReturnsAsync(emailContent);

        // Act
        var result = await _handler.HandleAsync(customMessageSignUp);

        // Assert
        Assert.IsNotNull(result.Response.EmailMessage, "The EmailMessage should not be null.");
        Assert.IsInstanceOf<string>(result.Response.EmailMessage, "The EmailMessage should be a string.");
        StringAssert.Contains(expectedLink, result.Response.EmailMessage as string,
            "The EmailMessage should contain the verification link.");
        Assert.AreEqual(emailContent.EmailSubject, result.Response.EmailSubject);
        Assert.AreEqual(emailContent.EmailBody, result.Response.EmailMessage);
        _mockLinkBuilder.VerifyAll();
        _mockContentfulService.VerifyAll();
    }

}
