using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.SecretsManager.Model;
using Amazon.SQS;
using Common;
using Common.Interfaces;
using Common.Settings;
using MessageListenerBase;
using MessageListenerBase.EventHandlers;
using MessageListenerBase.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace MessageListener
{
    public class Function : EventFunctionBase<SQSEvent>
    {
        private const string AwsSecretManagerSecretName = "AWS_SECRET_MANAGER_SECRET_NAME";
        
        protected override void Configure(IConfigurationBuilder builder)
        {
            builder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddSecretsManager(configurator: opts =>
                {
                    var awsSecretsName = Environment.GetEnvironmentVariable(AwsSecretManagerSecretName);

                    if (string.IsNullOrWhiteSpace(awsSecretsName))
                    {
                        Logger.LogWarning($"The {AwsSecretManagerSecretName} environment variable has not been set");
                    }
                    
                    var allowedSecretNames = new[] { awsSecretsName };
                    
                    opts.SecretFilter = entry => HasValue(allowedSecretNames, entry);
                    opts.KeyGenerator = (entry, key) => GenerateKey(allowedSecretNames, key);
                });;
        }

        protected override void ConfigureLogging(ILoggingBuilder logging, IExecutionEnvironment executionEnvironment)
        {
            logging.AddLambdaLogger();
        }

        protected override void ConfigureServices(IServiceCollection services, IExecutionEnvironment executionEnvironment)
        {
            // Configuration
            var appSettings = Configuration.GetSection(AppSettings.SectionName).Get<AppSettings>();
            if (appSettings == null)
            {
                throw new Exception("Could not bind the app settings, please check configuration");
            }
            
            var awsSettings = Configuration.GetSection(AwsSettings.SectionName).Get<AwsSettings>();
            if (awsSettings == null)
            {
                throw new Exception("Could not bind the aws settings, please check configuration");
            }
            
            services.AddSingleton(appSettings);
            services.AddSingleton(awsSettings);
            
            // AWS
            var awsOptions = new AWSOptions();
            
            if (!string.IsNullOrWhiteSpace(awsSettings.ServiceUrl))
            {
                awsOptions.DefaultClientConfig.ServiceURL = awsSettings.ServiceUrl;
            }
            
            services.AddAWSService<IAmazonSQS>(awsOptions);

            // IEventHandlers
            services.Configure<ParallelSqsExecutionOptions>(option => option.MaxDegreeOfParallelism = 5);
            
            if (executionEnvironment.RunAsQueueListener)
            {
                services.AddTransient<IEventHandler<SQSEvent>, ManualMessageEventHandler>();
            }
            else
            {
                if (appSettings.RunInParallel)
                    services.AddTransient<IEventHandler<SQSEvent>, ParallelSqsEventHandler>();
                else
                    services.AddTransient<IEventHandler<SQSEvent>, SqsEventHandler>();
            }
            
            // Handlers
            services.AddTransient<IHandlerExecutor, HandlerExecutor>(_ => new HandlerExecutor(ServiceProvider, Assembly.GetExecutingAssembly()));
            
            services.Scan(s => s
                .FromAssemblies(Assembly.GetExecutingAssembly())
                .AddClasses(c => c.AssignableTo(typeof(IHandler<,>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());
            
            // Others
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddTransient<IClock, Clock>();
        }

        // Needed to be able to run
        public async Task FunctionHandler(SQSEvent input, ILambdaContext context)
        {
            await FunctionHandlerAsync(input, context);
        }
        
        // Only load entries that start with any of the allowed prefixes
        private static bool HasValue(IEnumerable<string> allowedSecretNames, SecretListEntry entry)
        {
            return allowedSecretNames.Any(prefix => string.Equals(prefix, entry.Name, StringComparison.CurrentCultureIgnoreCase));
        }

        // Strip the prefix and replace '__' with ':'
        private static string GenerateKey(IEnumerable<string> allowedSecretNames, string entryName)
        {
            return entryName[(allowedSecretNames.First(x => entryName.StartsWith(x, StringComparison.CurrentCultureIgnoreCase)).Length + 1)..].Replace("__", ":");
        }
    }
}
