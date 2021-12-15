using System;
using System.Threading.Tasks;
using Amazon.Lambda.CloudWatchEvents.ScheduledEvents;
using Application.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application.EventHandlers.ScheduledJobs
{
    public class ScheduledEventLambdaHandler : ILambdaEventHandler<ScheduledEvent>
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public ScheduledEventLambdaHandler(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory?.CreateLogger("SqsEventHandler") ?? throw new ArgumentNullException(nameof(loggerFactory));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }
        
        public async Task HandleLambdaEventAsync(ScheduledEvent @event)
        {
            using var scope = _serviceProvider.CreateScope();
            var handlerExecutor = _serviceProvider.GetService<IScheduledJobsHandlerExecutor>();
                
            if (handlerExecutor == null)
            {
                throw new Exception("No IHandlerExecutor found");
            }

            var (handlerName, success) = await handlerExecutor.ExecuteHandlerAsync(@event);
            _logger.LogInformation($"**** Handle {(success ? "SUCCESS" : "FAILURE")} for handler: {handlerName}");
        }
    }
}