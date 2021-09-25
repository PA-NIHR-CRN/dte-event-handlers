using System.IO;
using System.Reflection;
using Adapter;
using Adapter.Fakes;
using Adapter.Mappers;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Domain.Services;
using Evento;
using MessageListener.Base;
using MessageListener.Extensions;
using MessageListener.Handlers;
using MessageListener.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace MessageListener
{
    public class Function : EventFunctionBase<SQSEvent>
    {
        protected override void Configure(IConfigurationBuilder builder)
        {
            builder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
        }

        protected override void ConfigureLogging(ILoggingBuilder logging, IExecutionEnvironment executionEnvironment)
        {
            logging.AddLambdaLogger();
        }

        protected override void ConfigureServices(IServiceCollection services, IExecutionEnvironment executionEnvironment)
        {
            // Configuration
            services.AddSingleton(Configuration.GetSection(AppSettings.SectionName).Get<AppSettings>());
            
            // Handlers
            services.UseSqsHandler<CloudEvent, CloudEventHandler>();
            
            // Others
            services.AddTransient<IDomainRepository, InMemoryDomainRepository>();
            services.AddTransient<IStudyService, FakeStudyService>();
            services.AddTransient<IWorker, Worker>();
            
            // Mappers
            services.Scan(s => s
                .FromAssemblies(Assembly.Load("Adapter"))
                .AddClasses(c => c.AssignableTo(typeof(ICloudEventMapper)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());
        }
    }
}
