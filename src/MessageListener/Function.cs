using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Adapter;
using Adapter.Contracts;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.SecretsManager.Model;
using Common;
using Common.Interfaces;
using Common.Settings;
using Domain.Contracts;
using Evento;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Services.Stubs;
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
            var awsSettings = Configuration.GetSection(AwsSettings.SectionName).Get<AwsSettings>();
            
            if (awsSettings == null)
            {
                throw new Exception("Could not bind the aws settings, please check configuration");
            }
            
            services.AddSingleton(awsSettings);
            
            // Handlers
            services.UseSqsHandler<CloudEvent, CloudEventHandler>();
            
            // Others
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<IAmazonDynamoDB>(ServiceLifetime.Scoped);
            services.AddTransient<IClock, Clock>();
            services.AddTransient<IDomainRepository, InMemoryDomainRepository>();
            services.AddTransient<IStudyRegistrationRepository, StudyRegistrationDynamoDbRepository>();
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
