using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Application;
using Application.Events;
using Application.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using JsonSerializer = System.Text.Json.JsonSerializer;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CognitoCustomMessageProcessor
{
    public class CognitoCustomMessageFunction : EventFunctionBase<CognitoCustomMessageEvent>
    {
        protected override void Configure(IConfigurationBuilder builder) => builder.AddConfiguration();
        protected override void ConfigureLogging(ILoggingBuilder logging, IExecutionEnvironment executionEnvironment) => logging.AddLambdaLogger();
        protected override void ConfigureServices(IServiceCollection services, IExecutionEnvironment executionEnvironment) => DependencyRegistration.RegisterServices(services, executionEnvironment, Configuration);

        // Needed to be able to run
        public async Task<JsonElement> FunctionHandler(CognitoCustomMessageEvent input, ILambdaContext context)
        {
            // Needs to be verification set to Code
            Logger.LogInformation($"FunctionHandler called for event: {input.GetType().Name}");

            var serialize = JsonSerializer.Serialize(input);
            Logger.LogInformation(serialize);
            
            await FunctionHandlerAsync(input, context);
            
            return JsonDocument.Parse(JsonSerializer.Serialize(input)).RootElement.Clone();
        }
    }
}
