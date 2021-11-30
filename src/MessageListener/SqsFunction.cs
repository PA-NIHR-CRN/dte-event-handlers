using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Application;
using Application.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace MessageListener
{
    public class SqsFunction : EventFunctionBase<SQSEvent>
    {
        protected override void Configure(IConfigurationBuilder builder) => builder.AddConfiguration();
        protected override void ConfigureLogging(ILoggingBuilder logging, IExecutionEnvironment executionEnvironment) => logging.AddLambdaLogger();
        protected override void ConfigureServices(IServiceCollection services, IExecutionEnvironment executionEnvironment) => DependencyRegistration.RegisterServices(services, executionEnvironment, Configuration);

        // Needed to be able to run
        public async Task FunctionHandler(SQSEvent input, ILambdaContext context)
        {
            Logger.LogInformation($"FunctionHandler called for event: {input.GetType().Name}");
            Logger.LogInformation($"LambdaContext: {JsonConvert.SerializeObject(context)}");
            
            await FunctionHandlerAsync(input);
        }
    }
}
