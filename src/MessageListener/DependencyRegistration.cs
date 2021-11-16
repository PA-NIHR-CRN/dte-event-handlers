using System;
using System.Reflection;
using Amazon.Lambda.SQSEvents;
using Application;
using Application.Contracts;
using Application.EventHandlers.Sqs;
using Application.Executors;
using Application.Extensions;
using Application.Resolvers;
using Common.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MessageListener
{
    public static class DependencyRegistration
    {
        public static IServiceCollection RegisterServices(IServiceCollection services, IExecutionEnvironment executionEnvironment, IConfigurationRoot configuration)
        {
            services.ConfigureServices(executionEnvironment, configuration);
            
            var appSettings = services.BuildServiceProvider().GetService<AppSettings>();

            if (appSettings == null)
            {
                throw new Exception("Can not find AppSettings in ServiceCollection");
            }

            if (executionEnvironment.RunAsQueueListener)
            {
                services.AddTransient<ILambdaEventHandler<SQSEvent>, ManualMessageLambdaEventHandler>();
            }
            else
            {
                if (appSettings.RunInParallel)
                    services.AddTransient<ILambdaEventHandler<SQSEvent>, ParallelSqsLambdaEventHandler>();
                else
                    services.AddTransient<ILambdaEventHandler<SQSEvent>, SqsLambdaEventHandler>();
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