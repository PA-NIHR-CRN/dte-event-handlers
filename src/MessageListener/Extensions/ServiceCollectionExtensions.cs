using Adapter;
using Adapter.Fakes;
using Amazon.Lambda.SQSEvents;
using Domain.Services;
using Evento;
using MessageListener.Base;
using Microsoft.Extensions.DependencyInjection;

namespace MessageListener.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureSnsParallelExecution(this IServiceCollection services, int maxDegreeOfParallelism)
        {
            services.Configure<ParallelSqsExecutionOptions>(option => option.MaxDegreeOfParallelism = maxDegreeOfParallelism);

            return services;
        }

        public static IServiceCollection UseSqsHandler<TMessage, THandler>(this IServiceCollection services, bool enableParallelExecution = false)
            where TMessage : class
            where THandler : class, IMessageHandler<TMessage>
        {
            services.AddOptions();

            if (enableParallelExecution)
            {
                services.AddTransient<IEventHandler<SQSEvent>, ParallelSqsEventHandler<TMessage>>();
            }
            else
            {
                services.AddTransient<IEventHandler<SQSEvent>, SqsEventHandler<TMessage>>();
            }

            services.AddTransient<IMessageHandler<TMessage>, THandler>();
            services.AddTransient<IDomainRepository, InMemoryDomainRepository>();
            services.AddTransient<IStudyService, FakeStudyService>();

            return services;
        }
    }
}