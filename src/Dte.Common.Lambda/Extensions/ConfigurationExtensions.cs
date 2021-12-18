using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Amazon.SecretsManager.Model;
using Dte.Common.Lambda.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dte.Common.Lambda.Extensions
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

        public static void ConfigureServices(this IServiceCollection services, IExecutionEnvironment executionEnvironment, IConfigurationRoot configuration, params SettingsBase[] settings)
        {
            // Configuration
            foreach (var setting in settings)
            {
                var baseSetting = configuration.GetSection(setting.SectionName).Get(setting.GetType());
                if (baseSetting == null)
                {
                    throw new Exception($"Could not bind the {setting.GetType().Name}, please check configuration");
                }
                
                services.AddSingleton(baseSetting.GetType(), baseSetting);
            }

            services.Configure<ParallelSqsExecutionOptions>(option => option.MaxDegreeOfParallelism = 5);

            // Others
            services.AddDefaultAWSOptions(configuration.GetAWSOptions());
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