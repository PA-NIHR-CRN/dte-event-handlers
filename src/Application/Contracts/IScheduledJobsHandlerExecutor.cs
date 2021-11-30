using System.Threading.Tasks;
using Amazon.Lambda.CloudWatchEvents.ScheduledEvents;

namespace Application.Contracts
{
    public interface IScheduledJobsHandlerExecutor
    {
        Task<(string, bool)> ExecuteHandlerAsync(ScheduledEvent @event);
    }
}