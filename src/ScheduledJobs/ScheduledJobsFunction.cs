using System.Threading.Tasks;
using Amazon.Lambda.CloudWatchEvents.ScheduledEvents;
using Amazon.Lambda.Core;
using Dte.Common.Lambda;
using Dte.Common.Lambda.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace ScheduledJobs
{
    public class ScheduledJobsFunction : EventFunctionBase<ScheduledEvent>
    {
        protected override void Configure(IConfigurationBuilder builder) => builder.AddConfiguration();
        protected override void ConfigureLogging(ILoggingBuilder logging, IExecutionEnvironment executionEnvironment) => logging.AddLambdaLogger();
        protected override void ConfigureServices(IServiceCollection services, IExecutionEnvironment executionEnvironment) => DependencyRegistration.RegisterServices(services, executionEnvironment, Configuration);

        // Needed to be able to run
        public async Task FunctionHandler(ScheduledEvent input, ILambdaContext context)
        {
            Logger.LogInformation($"FunctionHandler called for event: {input.GetType().Name}");
            Logger.LogInformation($"LambdaContext: {JsonConvert.SerializeObject(context)}");
            Logger.LogInformation(JsonConvert.SerializeObject(input, Formatting.Indented));
            
            await FunctionHandlerAsync(input);
        }
    }
}
