using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;
using CognitoCustomMessageProcessor;
using CognitoCustomMessageProcessor.CustomMessages;
using CognitoCustomMessageProcessor.Settings;
using Dte.Common;
using Dte.Common.Lambda;
using Dte.Common.Lambda.Contracts;
using Dte.Common.Lambda.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace CognitoCustomMessageProcessorTests;

[TestFixture]
public class DependencyRegistrationTests
{
    private IServiceCollection _services;
    private Mock<IExecutionEnvironment> _executionEnvironmentMock;
    private Mock<IConfigurationRoot> _configurationRootMock;

    [SetUp]
    public void SetUp()
    {
        _services = new ServiceCollection();
        _executionEnvironmentMock = new Mock<IExecutionEnvironment>();
        _configurationRootMock = new Mock<IConfigurationRoot>();

        // Create a dictionary to represent the configuration key-value pairs
        var inMemorySettings = new Dictionary<string, string>
        {
            { "AwsSettings:ServiceUrl", "https://service.url" },
            { "AwsSettings:ParticipantRegistrationDynamoDbTableName", "DynamoDbTable" },
            { "AppSettings:WebAppBaseUrl", "http://yourwebappbaseurl.com" },
            { "ContentfulSettings:DeliveryApiKey", "your-delivery-api-key" },
            { "ContentfulSettings:PreviewApiKey", "your-preview-api-key" },
            { "ContentfulSettings:SpaceId", "your-space-id" },
            { "ContentfulSettings:UsePreviewApi", "false" },
            { "ContentfulSettings:BaseUrl", "http://yourbaseurl.com" },
            { "ContentfulSettings:EmailTemplates:ForgotPassword", "forgot-password-template-id" },
            { "ContentfulSettings:EmailTemplates:ResendCode", "resend-code-template-id" },
            { "ContentfulSettings:EmailTemplates:SignUp", "sign-up-template-id" },
            { "ContentfulSettings:EmailTemplates:UpdateUserAttribute", "update-user-attribute-template-id" },
            { "ContentfulSettings:EmailTemplates:NhsAccountExists", "nhs-account-exists-template-id" },
            { "ContentfulSettings:EmailTemplates:NhsPasswordReset", "nhs-password-reset-template-id" },
            { "ContentfulSettings:EmailTemplates:EmailAccountExists", "email-account-exists-template-id" },
            { "ContentfulSettings:EmailTemplates:NhsSignUp", "nhs-sign-up-template-id" },
            { "ContentfulSettings:EmailTemplates:MfaEmailConfirmation", "mfa-email-confirmation-template-id" },
            {
                "ContentfulSettings:EmailTemplates:MfaMobileNumberVerification",
                "mfa-mobile-number-verification-template-id"
            },
            { "ContentfulSettings:EmailTemplates:DeleteAccount", "delete-account-template-id" },
            { "ContentfulSettings:EmailTemplates:NewAccount", "new-account-template-id" },
        };

        // Create a configuration with the in-memory settings
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        // Set up the IConfigurationRoot to return the configuration instance
        _configurationRootMock.Setup(c => c.GetSection(It.IsAny<string>()))
            .Returns<string>(key => configuration.GetSection(key));
    }

    [Test]
    public void Configuration_CanRetrieveContentfulSettings()
    {
        // Act
        var contentfulSettings =
            _configurationRootMock.Object.GetSection("ContentfulSettings").Get<ContentfulSettings>();

        // Assert
        Assert.IsNotNull(contentfulSettings, "ContentfulSettings should not be null.");
        Assert.IsNotNull(contentfulSettings.EmailTemplates, "EmailTemplates should not be null.");
        Assert.AreEqual("forgot-password-template-id", contentfulSettings.EmailTemplates.ForgotPassword);
        Assert.AreEqual("resend-code-template-id", contentfulSettings.EmailTemplates.ResendCode);
        Assert.AreEqual("sign-up-template-id", contentfulSettings.EmailTemplates.SignUp);
        Assert.AreEqual("update-user-attribute-template-id", contentfulSettings.EmailTemplates.UpdateUserAttribute);
        Assert.AreEqual("nhs-account-exists-template-id", contentfulSettings.EmailTemplates.NhsAccountExists);
        Assert.AreEqual("nhs-password-reset-template-id", contentfulSettings.EmailTemplates.NhsPasswordReset);
        Assert.AreEqual("email-account-exists-template-id", contentfulSettings.EmailTemplates.EmailAccountExists);
        Assert.AreEqual("nhs-sign-up-template-id", contentfulSettings.EmailTemplates.NhsSignUp);
        Assert.AreEqual("mfa-email-confirmation-template-id", contentfulSettings.EmailTemplates.MfaEmailConfirmation);
        Assert.AreEqual("mfa-mobile-number-verification-template-id",
            contentfulSettings.EmailTemplates.MfaMobileNumberVerification);
        Assert.AreEqual("delete-account-template-id", contentfulSettings.EmailTemplates.DeleteAccount);
        Assert.AreEqual("new-account-template-id", contentfulSettings.EmailTemplates.NewAccount);
    }

    [Test]
    public void Configuration_CanRetrieveAwsSettings()
    {
        var awsSettings = _configurationRootMock.Object.GetSection("AwsSettings").Get<AwsSettings>();
        Assert.IsNotNull(awsSettings, "AwsSettings should not be null.");
        Assert.AreEqual("https://service.url", awsSettings.ServiceUrl);
        Assert.AreEqual("DynamoDbTable", awsSettings.ParticipantRegistrationDynamoDbTableName);
    }

    [Test]
    public void Configuration_CanRetrieveAppSettings()
    {
        var appSettings = _configurationRootMock.Object.GetSection("AppSettings").Get<AppSettings>();
        Assert.IsNotNull(appSettings, "AppSettings should not be null.");
    }

    [Test]
    public void RegisterServices_AddsAwsSettings_WhenCalled()
    {
        // Act
        DependencyRegistration.RegisterServices(_services, _executionEnvironmentMock.Object,
            _configurationRootMock.Object);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        var awsSettings = serviceProvider.GetService<AwsSettings>();
        Assert.IsNotNull(awsSettings, "AwsSettings should not be null.");
        Assert.AreEqual("https://service.url", awsSettings.ServiceUrl);
        Assert.AreEqual("DynamoDbTable", awsSettings.ParticipantRegistrationDynamoDbTableName);
    }

    [Test]
    public void RegisterServices_AddsAppSettings_WhenCalled()
    {
        // Act
        DependencyRegistration.RegisterServices(_services, _executionEnvironmentMock.Object,
            _configurationRootMock.Object);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetService<AppSettings>());
    }

    [Test]
    public void RegisterServices_AddsContentfulSettings_WhenCalled()
    {
        // Act
        DependencyRegistration.RegisterServices(_services, _executionEnvironmentMock.Object,
            _configurationRootMock.Object);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetService<ContentfulSettings>());
    }

    [Test]
    public void RegisterServices_AddsDynamoDBContext_WhenCalled()
    {
        // Act
        DependencyRegistration.RegisterServices(_services, _executionEnvironmentMock.Object,
            _configurationRootMock.Object);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetService<IDynamoDBContext>());
    }

    [Test]
    public void RegisterServices_AddsCognitoCustomMessageEventLambdaHandler_WhenCalled()
    {
        // Act
        DependencyRegistration.RegisterServices(_services, _executionEnvironmentMock.Object,
            _configurationRootMock.Object);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetService<ILambdaEventHandler<CognitoCustomMessageEvent>>());
    }

    [Test]
    public void RegisterServices_AddsHandlers_WhenCalled()
    {
        // Act
        DependencyRegistration.RegisterServices(_services, _executionEnvironmentMock.Object,
            _configurationRootMock.Object);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        Assert.That(serviceProvider.GetServices<IHandler<CustomMessageForgotPassword, CognitoCustomMessageEvent>>(),
            Is.Not.Empty);
        Assert.That(serviceProvider.GetServices<IHandler<CustomMessageResendCode, CognitoCustomMessageEvent>>(),
            Is.Not.Empty);
        Assert.That(serviceProvider.GetServices<IHandler<CustomMessageSignUp, CognitoCustomMessageEvent>>(),
            Is.Not.Empty);
        Assert.That(
            serviceProvider.GetServices<IHandler<CustomMessageUpdateUserAttribute, CognitoCustomMessageEvent>>(),
            Is.Not.Empty);
    }

    [Test]
    public void AllRegisteredServices_CanBeResolved()
    {
        // Arrange
        DependencyRegistration.RegisterServices(_services, _executionEnvironmentMock.Object,
            _configurationRootMock.Object);
        var serviceProvider = _services.BuildServiceProvider();

        // Act & Assert
        foreach (var serviceDescriptor in _services)
        {
            Assert.DoesNotThrow(() =>
                {
                    if (serviceDescriptor.ServiceType.IsGenericTypeDefinition)
                    {
                        return;
                    }

                    var service = serviceProvider.GetService(serviceDescriptor.ServiceType);
                    Assert.IsNotNull(service,
                        $"Service of type {serviceDescriptor.ServiceType} could not be resolved.");
                }, $"Service of type {serviceDescriptor.ServiceType} threw an exception when being resolved.");
        }
    }
}
