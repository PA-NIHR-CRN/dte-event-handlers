using System;
using System.Collections.Generic;
using System.IO;
using Adapter.Contracts;
using Evento;
using Evento.Repository;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Adapter
{
    public class DomainRepositoryBuilder : IDomainRepositoryBuilder
    {
        private readonly Settings _settings;

        public DomainRepositoryBuilder()
        {
            // TODO find a way to inject settings
            _settings = new Settings
            {
                EventStore_ProcessorLink = "tcp://localhost:1113", 
                EventStore_Username = "admin",
                EventStore_Password = "changeit"
            };
        }
        
        public IDomainRepository Build()
        {
            var conn = BuilderForDomain("domain-dte", _settings,
                ConnectionBuilder.BuildConnectionSettings(
                    new UserCredentials(_settings.EventStore_Username, _settings.EventStore_Password), _settings.CertificateFqdn)).Build();
            return new EventStoreDomainRepository("dte", conn);
        }

        private static IConnectionBuilder BuilderForDomain(string connectionName, Settings settings,
            ConnectionSettings connSettings)
        {
            var builderForDomain = new ConnectionBuilder(new Uri(settings.EventStore_ProcessorLink), connSettings,
                connectionName, new UserCredentials(settings.EventStore_Username, settings.EventStore_Password), new Logger<ConnectionBuilder>(new LoggerFactory()));
            return builderForDomain;
        }
        
        private static IConfigurationRoot BuildConfig()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "dev";
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();
            return builder.Build();
        }
    }
}