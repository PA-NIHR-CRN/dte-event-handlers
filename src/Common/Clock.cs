using System;
using Common.Interfaces;

namespace Common
{
    public class Clock : IClock
    {
        public DateTime UtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}