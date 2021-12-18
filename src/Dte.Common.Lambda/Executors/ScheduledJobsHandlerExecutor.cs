using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.CloudWatchEvents.ScheduledEvents;
using Dte.Common.Lambda.Contracts;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Dte.Common.Lambda.Executors
{
    public class ScheduledJobsHandlerExecutor : IScheduledJobsHandlerExecutor
    {
        private readonly IHandlerResolver _handlerResolver;

        public ScheduledJobsHandlerExecutor(IHandlerResolver handlerResolver)
        {
            _handlerResolver = handlerResolver;
        }
        
        public async Task<(string, bool)> ExecuteHandlerAsync(ScheduledEvent @event)
        {
            if (@event == null)
            {
                throw new Exception("the event is null");
            }

            if (@event.Resources == null || @event.Resources.Count == 0)
            {
                throw new Exception("Resources is null or empty, dont know which handler to call");
            }
            
            if (@event.Resources.Count > 1)
            {
                throw new Exception("Resources count is more than 1, dont know which handler to call");
            }
            
            var (handlerImpl, invoke) = _handlerResolver.ResolveHandler(@event.Resources[0].Split("/").Last(), JsonSerializer.Serialize(@event));

            return (handlerImpl.GetType().Name, await (Task<bool>)invoke);
        }
    }
}