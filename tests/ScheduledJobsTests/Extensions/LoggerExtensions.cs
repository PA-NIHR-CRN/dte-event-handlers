using System;
using Microsoft.Extensions.Logging;
using Moq;

namespace ScheduledJobsTests.Extensions;

public static class LoggerExtensions
{
    public static void VerifyMessageLogged<T>(this Mock<ILogger<T>> logger,
        LogLevel logLevel,
        Predicate<string> matcher,
        Func<Times> times = null,
        Exception ex = null)
    {
        logger.Verify(
            x => x.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => matcher(o.ToString())),
                ex ?? It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            times ?? Times.AtLeastOnce);
    }
}
