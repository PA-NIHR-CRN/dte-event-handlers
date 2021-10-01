using System;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace Adapter.Contracts
{
    public interface IConnectionBuilder
    {
        string ConnectionName { get; }
        Uri ConnectionString { get; }
        ConnectionSettings ConnectionSettings { get; }
        UserCredentials Credentials { get; }
        IEventStoreConnection Build(bool openConnection = true);
    }
}