using System;
using System.Reflection;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Lambda.CloudWatchEvents.ScheduledEvents;
using Amazon.S3;
using Amazon.SQS;
using Application;
using Application.Contracts;
using Application.EventHandlers.ScheduledEvents;
using Application.EventHandlers.Sqs;
using Application.Executors;
using Application.Extensions;
using Application.Resolvers;
using Application.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScheduledJobs.Contracts;
using ScheduledJobs.Repositories;
using ScheduledJobs.Settings;

namespace ScheduledJobs
{
    public static class DependencyRegistration
    {
        public static IServiceCollection RegisterServices(IServiceCollection services, IExecutionEnvironment executionEnvironment, IConfigurationRoot configuration)
        {
            var requiredSettings = new SettingsBase []{ new CpmsImportSettings(), new AwsSettings() };
            services.ConfigureServices(executionEnvironment, configuration, requiredSettings);
            
            var awsSettings = services.BuildServiceProvider().GetService<AwsSettings>();
            if (awsSettings == null) throw new Exception("Can not find AwsSettings in ServiceCollection");

            // AWS
            var awsOptions = new AWSOptions();
            var amazonDynamoDbConfig = new AmazonDynamoDBConfig();
            if (!string.IsNullOrWhiteSpace(awsSettings.ServiceUrl))
            {
                awsOptions.DefaultClientConfig.ServiceURL = awsSettings.ServiceUrl;
                amazonDynamoDbConfig.ServiceURL = awsSettings.ServiceUrl;
            }
            services.AddAWSService<IAmazonS3>(awsOptions);
            services.AddScoped<IAmazonDynamoDB>(_ => new AmazonDynamoDBClient(amazonDynamoDbConfig));
            services.AddScoped<IDynamoDBContext>(_ => new DynamoDBContext(new AmazonDynamoDBClient(amazonDynamoDbConfig)));

            // Lambda Event Handlers
            services.AddTransient<ILambdaEventHandler<ScheduledEvent>, ScheduledEventLambdaHandler>();
            
            // Handlers
            services.AddTransient<IHandlerResolver>(_ => new HandlerResolver(services.BuildServiceProvider(), Assembly.GetExecutingAssembly()));
            services.AddTransient<IScheduledJobsHandlerExecutor, ScheduledJobsHandlerExecutor>();
            
            services.Scan(s => s
                .FromAssemblies(Assembly.GetExecutingAssembly())
                .AddClasses(c => c.AssignableTo(typeof(IHandler<,>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());
            
            // Others
            services.AddTransient<ICpmsStudyDynamoDbRepository, CpmsStudyDynamoDbRepository>();

            return services;
        }
    }
}