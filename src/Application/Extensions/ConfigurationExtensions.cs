using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Amazon.Extensions.NETCore.Setup;
using Amazon.SecretsManager.Model;
using Amazon.SQS;
using Common;
using Common.Interfaces;
using Common.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application.Extensions
{
    public static class ConfigurationExtensions
    {
        private const string AwsSecretManagerSecretName = "AWS_SECRET_MANAGER_SECRET_NAME";
        
        public static void AddConfiguration(this IConfigurationBuilder builder)
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
                        var logger = new LoggerFactory().AddLambdaLogger().CreateLogger<FunctionBase>();
                        logger.LogError($"The {AwsSecretManagerSecretName} environment variable has not been set");
                    }
                    
                    var allowedSecretNames = new[] { awsSecretsName };
                    
                    opts.SecretFilter = entry => HasValue(allowedSecretNames, entry);
                    opts.KeyGenerator = (entry, key) => GenerateKey(allowedSecretNames, key);
                });
        }

        public static void ConfigureServices(this IServiceCollection services, IExecutionEnvironment executionEnvironment, IConfigurationRoot configuration)
        {
            // Configuration
            var appSettings = configuration.GetSection(AppSettings.SectionName).Get<AppSettings>();
            if (appSettings == null)
            {
                throw new Exception("Could not bind the app settings, please check configuration");
            }
            
            var awsSettings = configuration.GetSection(AwsSettings.SectionName).Get<AwsSettings>();
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

            services.Configure<ParallelSqsExecutionOptions>(option => option.MaxDegreeOfParallelism = 5);

            // Others
            services.AddDefaultAWSOptions(configuration.GetAWSOptions());
            services.AddTransient<IClock, Clock>();
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