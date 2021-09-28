using System;

namespace Common.Interfaces
{
    public interface IClock
    {
        DateTime UtcNow();
    }
}