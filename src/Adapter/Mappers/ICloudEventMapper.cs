using System;
using Evento;

namespace Adapter.Mappers
{
    public interface ICloudEventMapper
    {
        Uri Schema { get; }
        Command Map(CloudEvent request);
    }
}