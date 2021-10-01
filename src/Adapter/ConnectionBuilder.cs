using System;
using Adapter.Contracts;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Microsoft.Extensions.Logging;

namespace Adapter
{
    public class ConnectionBuilder : IConnectionBuilder
    {
        private readonly ILogger<ConnectionBuilder> _log;
        public Uri ConnectionString { get; }
        public ConnectionSettings ConnectionSettings { get; }
        public string ConnectionName { get; }
        public UserCredentials Credentials { get; }
        public IEventStoreConnection Build(bool openConnection = true)
        {
            var conn = EventStoreConnection.Create(ConnectionSettings, ConnectionString, ConnectionName);
            conn.Disconnected += Conn_Disconnected;
            conn.Reconnecting += Conn_Reconnecting;
            conn.Connected += Conn_Connected;
            if (openConnection)
                conn.ConnectAsync().Wait();

            return conn;
        }

        private void Conn_Connected(object sender, ClientConnectionEventArgs e)
        {
            _log.LogDebug($"Connected to EventStore RemoteEndPoint:'{e.RemoteEndPoint}';ConnectionName:'{e.Connection.ConnectionName}'");
        }

        private void Conn_Reconnecting(object sender, ClientReconnectingEventArgs e)
        {
            _log.LogDebug($"Reconnecting to EventStore ConnectionName:'{e.Connection.ConnectionName}'");
        }

        private void Conn_Disconnected(object sender, ClientConnectionEventArgs e)
        {
            _log.LogError($"Disconnected from EventStore RemoteEndPoint:'{e.RemoteEndPoint}';ConnectionName:'{e.Connection.ConnectionName}'");
        }

        public ConnectionBuilder(Uri connectionString, ConnectionSettings connectionSettings, string connectionName, UserCredentials credentials, ILogger<ConnectionBuilder> logger)
        {
            ConnectionString = connectionString;
            ConnectionSettings = connectionSettings;
            ConnectionName = connectionName;
            Credentials = credentials;
            _log = logger;
        }

        public static ConnectionSettings BuildConnectionSettings(UserCredentials userCredentials, string certificateFqdn)
        {
            var connectionSettingsBuilder = string.IsNullOrWhiteSpace(certificateFqdn)
                ? ConnectionSettings.Create()
                    .SetDefaultUserCredentials(userCredentials)
                    .KeepReconnecting().KeepRetrying()
                : ConnectionSettings.Create()
                    .UseSslConnection(certificateFqdn, true)
                    .SetDefaultUserCredentials(userCredentials)
                    .KeepReconnecting().KeepRetrying();

            return connectionSettingsBuilder.Build();
        }
    }
}