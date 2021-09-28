using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Adapter;
using Adapter.Contracts;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Common;
using Common.Interfaces;
using Domain.Contracts;
using Evento;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Services.Fakes;
using Infrastructure.Services.Stubs;
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
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<IAmazonDynamoDB>(ServiceLifetime.Scoped);
            services.AddTransient<IClock, Clock>();
            services.AddTransient<IDomainRepository, InMemoryDomainRepository>();
            services.AddTransient<IStudyRepository, StudyDynamoDbRepository>();
            services.AddTransient<IStudyService, StudyService>();
            services.AddTransient<IWorker, Worker>();

            // Mappers
            services.Scan(s => s
                .FromAssemblies(Assembly.Load("Adapter"))
                .AddClasses(c => c.AssignableTo(typeof(ICloudEventMapper)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());
        }

        // Needed to be able to run
        public async Task FunctionHandler(SQSEvent input, ILambdaContext context)
        {
            await FunctionHandlerAsync(input, context);
        }
    }
}
