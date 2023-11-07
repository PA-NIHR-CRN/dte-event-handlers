using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Dte.Common.Lambda;
using Dte.Common.Lambda.Events;
using Dte.Common.Lambda.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CognitoCustomMessageProcessor
{
    public class CognitoCustomMessageFunction : EventFunctionBase<CognitoCustomMessageEvent>
    {
        private static readonly HashSet<string> MfaTriggers = new()
        { 
            "CustomMessage_VerifyUserAttribute",
            "CustomMessage_Authentication"
        };
        protected override void Configure(IConfigurationBuilder builder) => builder.AddConfiguration();
        protected override void ConfigureLogging(ILoggingBuilder logging, IConfiguration configuration, IExecutionEnvironment executionEnvironment) => logging.AddLambdaLogger(configuration, "Logging");
        protected override void ConfigureServices(IServiceCollection services, IExecutionEnvironment executionEnvironment) => DependencyRegistration.RegisterServices(services, executionEnvironment, Configuration);

        // Needed to be able to run
        public async Task<JsonElement> FunctionHandler(CognitoCustomMessageEvent input, ILambdaContext context)
        {
            if (MfaTriggers.Contains(input.TriggerSource))
            {
                Logger.LogInformation("Inside the MFA trigger");
                return JsonSerializer.SerializeToElement(input);
            }
            // Needs to be verification set to Code
            Logger.LogInformation($"{nameof(CognitoCustomMessageFunction)}:FunctionHandler called for event: {input.GetType().Name}");
            Logger.LogInformation($"LambdaContext: {JsonConvert.SerializeObject(context)}");

            var serialize = JsonSerializer.Serialize(input);
            Logger.LogInformation(serialize);
            
            await FunctionHandlerAsync(input);
            Logger.LogInformation("**** Response: {Response}", JsonConvert.SerializeObject(input));
            
            return JsonSerializer.SerializeToElement(input);
        }
    }
}
