using System;
using System.Reflection;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Lambda.SQSEvents;
using Amazon.SQS;
using Application;
using Application.Contracts;
using Application.EventHandlers.Sqs;
using Application.Executors;
using Application.Extensions;
using Application.Resolvers;
using Application.Settings;
using MessageListener.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MessageListener
{
    public static class DependencyRegistration
    {
        public static IServiceCollection RegisterServices(IServiceCollection services, IExecutionEnvironment executionEnvironment, IConfigurationRoot configuration)
        {
            var requiredSettings = new SettingsBase []{ new AppSettings(), new AwsSettings() };
            services.ConfigureServices(executionEnvironment, configuration, requiredSettings);
            
            var appSettings = services.BuildServiceProvider().GetService<AppSettings>();
            if (appSettings == null) throw new Exception("Can not find AppSettings in ServiceCollection");
            var awsSettings = services.BuildServiceProvider().GetService<AwsSettings>();
            if (awsSettings == null) throw new Exception("Can not find AwsSettings in ServiceCollection");

            // AWS
            var awsOptions = new AWSOptions();
            if (!string.IsNullOrWhiteSpace(awsSettings.ServiceUrl))
            {
                awsOptions.DefaultClientConfig.ServiceURL = awsSettings.ServiceUrl;
            }
            services.AddAWSService<IAmazonSQS>(awsOptions);

            if (executionEnvironment.RunAsQueueListener)
            {
                services.AddTransient<ILambdaEventHandler<SQSEvent>, ManualSqsEventLambdaHandler>();
            }
            else
            {
                if (appSettings.RunInParallel)
                    services.AddTransient<ILambdaEventHandler<SQSEvent>, ParallelSqsEventLambdaHandler>();
                else
                    services.AddTransient<ILambdaEventHandler<SQSEvent>, SqsEventLambdaHandler>();
            }
            
            // Handlers
            services.AddTransient<IHandlerResolver>(_ => new HandlerResolver(services.BuildServiceProvider(), Assembly.GetExecutingAssembly()));
            services.AddTransient<ISqsMessageHandlerExecutor, SqsMessageHandlerExecutor>();
            
            services.Scan(s => s
                .FromAssemblies(Assembly.GetExecutingAssembly())
                .AddClasses(c => c.AssignableTo(typeof(IHandler<,>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());

            return services;
        }
    }
}