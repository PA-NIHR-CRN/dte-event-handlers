using System.Threading.Tasks;
using Amazon.Lambda.CloudWatchEvents.ScheduledEvents;

namespace Dte.Common.Lambda.Contracts
{
    public interface IScheduledJobsHandlerExecutor
    {
        Task<(string, bool)> ExecuteHandlerAsync(ScheduledEvent @event);
    }
}