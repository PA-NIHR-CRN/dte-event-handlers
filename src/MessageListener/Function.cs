using System.IO;
using Adapter;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
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
    public class Function : EventFunction<SQSEvent>
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
        }
    }
}
