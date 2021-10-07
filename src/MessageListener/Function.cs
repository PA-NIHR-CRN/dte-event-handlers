using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Adapter;
using Adapter.Contracts;
using Adapter.Handlers;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.SecretsManager.Model;
using Common;
using Common.Interfaces;
using Common.Settings;
using Domain.Commands;
using Evento;
using MessageListener.Base;
using MessageListener.Extensions;
using MessageListener.Handlers;
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
            
            var eventStoreSettings = Configuration.GetSection(EventStoreSettings.SectionName).Get<EventStoreSettings>();
            if (eventStoreSettings == null)
            {
                throw new Exception("Could not bind the event store settings, please check configuration");
            }
            
            services.AddSingleton(appSettings);
            services.AddSingleton(eventStoreSettings);
            
            // Factories
            services.AddTransient<ICommandExecutor, CommandExecutor>();
            services.AddTransient<IHandle<SubmitStudyForApproval>, SubmitStudyForApprovalHandler>();
            
            // Handlers
            services.UseSqsHandler<CloudEvent, CloudEventHandler>();
            services.Scan(s => s
                .FromAssemblies(Assembly.Load("Domain"))
                .AddClasses(c => c.AssignableTo(typeof(IHandle<>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());
            
            // Others
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddTransient<IClock, Clock>();
            services.AddTransient<IDomainRepositoryBuilder, InMemoryDomainRepositoryBuilder>();
            services.AddTransient<IDomainRepository, InMemoryDomainRepository>();
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
