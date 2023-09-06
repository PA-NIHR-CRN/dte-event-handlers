using System;
using System.Net.Http;
using System.Reflection;
using Dte.Common.Lambda;
using Dte.Common.Lambda.Contracts;
using Dte.Common.Lambda.EventHandlers.Cognito;
using Dte.Common.Lambda.Events;
using Dte.Common.Lambda.Executors;
using Dte.Common.Lambda.Extensions;
using Dte.Common.Lambda.Resolvers;
using Dte.Common.Lambda.Settings;
using CognitoCustomMessageProcessor.Builders;
using CognitoCustomMessageProcessor.Contracts;
using CognitoCustomMessageProcessor.Services;
using CognitoCustomMessageProcessor.Settings;
using Contentful.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScheduledJobs.Contracts;
using ScheduledJobs.Repositories;

namespace CognitoCustomMessageProcessor
{
    public static class DependencyRegistration
    {
        public static IServiceCollection RegisterServices(IServiceCollection services,
            IExecutionEnvironment executionEnvironment, IConfigurationRoot configuration)
        {
            var requiredSettings = new SettingsBase[]
                { new AppSettings(), new AwsSettings(), new ContentfulSettings() };
            services.ConfigureServices(executionEnvironment, configuration, requiredSettings);

            var serviceProvider = services.BuildServiceProvider();
            var appSettings = serviceProvider.GetService<AppSettings>();
            if (appSettings == null) throw new Exception("Can not find AppSettings in ServiceCollection");
            var awsSettings = serviceProvider.GetService<AwsSettings>();
            if (awsSettings == null) throw new Exception("Can not find AwsSettings in ServiceCollection");
            var contentfulEmailTemplateSettings = serviceProvider.GetService<ContentfulSettings>();
            if (contentfulEmailTemplateSettings == null)
                throw new Exception("Can not find ContentfulSettings in ServiceCollection");

            services
                .AddTransient<ILambdaEventHandler<CognitoCustomMessageEvent>, CognitoCustomMessageEventLambdaHandler>();

            // Contentful set up
            services.AddHttpClient();
            var contentfulSettings =
                configuration.GetSection(ContentfulSettings.ConfigSectionName).Get<ContentfulSettings>();
            services.AddSingleton<IContentfulClient>(sp =>
            {
                var httpClient = sp.GetRequiredService<HttpClient>();
                return new ContentfulClient(httpClient, contentfulSettings.DeliveryApiKey,
                    contentfulSettings.PreviewApiKey, contentfulSettings.SpaceId, contentfulSettings.UsePreviewApi);
            });

            // Handlers
            services.AddTransient<IHandlerResolver>(_ =>
                new HandlerResolver(services.BuildServiceProvider(), Assembly.GetExecutingAssembly()));
            services.AddTransient<ICognitoMessageHandlerExecutor, CognitoMessageHandlerExecutor>();
            services.AddTransient<ILinkBuilder, LinkBuilder>();
            services.AddTransient<IContentfulService, ContentfulService>();
            services.AddTransient<IRichTextToHtmlService, RichTextToHtmlService>();
            services
                .AddTransient<IParticipantRegistrationDynamoDbRepository, ParticipantRegistrationDynamoDbRepository>();

            services.Scan(s => s
                .FromAssemblies(Assembly.GetExecutingAssembly())
                .AddClasses(c => c.AssignableTo(typeof(IHandler<,>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());

            return services;
        }
    }
}