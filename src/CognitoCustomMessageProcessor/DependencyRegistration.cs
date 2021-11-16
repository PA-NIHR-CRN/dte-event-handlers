using System.Reflection;
using Application;
using Application.Contracts;
using Application.EventHandlers.Cognito;
using Application.Events;
using Application.Executors;
using Application.Extensions;
using Application.Resolvers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CognitoCustomMessageProcessor
{
    public static class DependencyRegistration
    {
        public static IServiceCollection RegisterServices(IServiceCollection services, IExecutionEnvironment executionEnvironment, IConfigurationRoot configuration)
        {
            services.ConfigureServices(executionEnvironment, configuration);

            services.AddTransient<ILambdaEventHandler<CognitoCustomMessageEvent>, CognitoCustomMessageLambdaEventHandler>();

            // Handlers
            services.AddTransient<IHandlerResolver>(_ => new HandlerResolver(services.BuildServiceProvider(), Assembly.GetExecutingAssembly()));
            services.AddTransient<ICognitoMessageHandlerExecutor, CognitoMessageHandlerExecutor>();

            services.Scan(s => s
                .FromAssemblies(Assembly.GetExecutingAssembly())
                .AddClasses(c => c.AssignableTo(typeof(IHandler<,>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());

            return services;
        }
    }
}