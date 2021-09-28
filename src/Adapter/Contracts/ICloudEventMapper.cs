using System;
using Evento;

namespace Adapter.Contracts
{
    public interface ICloudEventMapper
    {
        Uri Schema { get; }
        Command Map(CloudEvent request);
    }
}