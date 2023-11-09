using System;
using Microsoft.Extensions.Logging;
using Moq;

namespace ScheduledJobsTests.Extensions;

public static class LoggerExtensions
{
    public static void VerifyMessageLogged<T>(this Mock<ILogger<T>> logger,
        LogLevel logLevel,
        string matcher,
        Func<Times> times = null)
    {
        logger.Verify(l => l.Log(
                It.Is<LogLevel>(ll => ll == logLevel),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString().Contains(matcher)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            times ?? Times.AtLeastOnce);
    }
}
