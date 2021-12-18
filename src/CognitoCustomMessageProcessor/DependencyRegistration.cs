using System;
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
using CognitoCustomMessageProcessor.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CognitoCustomMessageProcessor
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

            services.AddTransient<ILambdaEventHandler<CognitoCustomMessageEvent>, CognitoCustomMessageEventLambdaHandler>();

            // Handlers
            services.AddTransient<IHandlerResolver>(_ => new HandlerResolver(services.BuildServiceProvider(), Assembly.GetExecutingAssembly()));
            services.AddTransient<ICognitoMessageHandlerExecutor, CognitoMessageHandlerExecutor>();
            services.AddTransient<ILinkBuilder, LinkBuilder>();

            services.Scan(s => s
                .FromAssemblies(Assembly.GetExecutingAssembly())
                .AddClasses(c => c.AssignableTo(typeof(IHandler<,>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());

            return services;
        }
    }
}