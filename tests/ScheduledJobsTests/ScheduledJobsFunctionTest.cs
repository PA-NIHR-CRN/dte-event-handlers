using System.Collections.Generic;
using Amazon.Lambda.CloudWatchEvents.ScheduledEvents;
using Amazon.Lambda.TestUtilities;
using NUnit.Framework;
using ScheduledJobs;

namespace ScheduledJobsTests
{
    [TestFixture]
    public class ScheduledJobsFunctionTest
    {
        [Test]
        public void Function_Executes_Correct_Handler_From_Incoming_Event()
        {
            var scheduledEvent = new ScheduledEvent();
            var function = new ScheduledJobsFunction();

            scheduledEvent.Resources = new List<string> { "CpmsImport"};
            Assert.DoesNotThrowAsync(() => function.FunctionHandler(scheduledEvent, new TestLambdaContext()), $"No job handler for job type: {scheduledEvent.Resources[0]}");
        }
    }
}
