using System;
using System.IO;
using Adapter.Contracts;
using Common.Settings;
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
        private readonly AppSettings _appSettings;
        private readonly EventStoreSettings _eventStoreSettings;

        public DomainRepositoryBuilder(AppSettings appSettings, EventStoreSettings eventStoreSettings)
        {
            _appSettings = appSettings;
            _eventStoreSettings = eventStoreSettings;
        }
        
        public IDomainRepository Build()
        {
            var conn = BuilderForDomain("domain-dte", _eventStoreSettings,
                ConnectionBuilder.BuildConnectionSettings(
                    new UserCredentials(_eventStoreSettings.Username, _eventStoreSettings.Password), _appSettings.CertificateFqdn)).Build();
            return new EventStoreDomainRepository("dte", conn);
        }

        private static IConnectionBuilder BuilderForDomain(string connectionName, EventStoreSettings eventStoreSettings,
            ConnectionSettings connSettings)
        {
            var builderForDomain = new ConnectionBuilder(new Uri(eventStoreSettings.ProcessorLink), connSettings,
                connectionName, new UserCredentials(eventStoreSettings.Username, eventStoreSettings.Password), new Logger<ConnectionBuilder>(new LoggerFactory()));
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