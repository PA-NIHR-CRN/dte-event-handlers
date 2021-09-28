using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Evento;

namespace Infrastructure.Services.Stubs
{
    public class InMemoryDomainRepository : IDomainRepository
    {
        private readonly Dictionary<string, List<Event>> _eventStore = new Dictionary<string, List<Event>>();
        
        public IEnumerable<Event> Save<TAggregate>(TAggregate aggregate) where TAggregate : IAggregate
        {
            var uncommittedEvents = aggregate.UncommitedEvents().ToList();
            if (!_eventStore.ContainsKey(aggregate.AggregateId))
            {
                _eventStore.Add(aggregate.AggregateId, aggregate.UncommitedEvents().ToList());
            }
            else
            {
                _eventStore[aggregate.AggregateId].AddRange(aggregate.UncommitedEvents().ToList());
            }
            aggregate.ClearUncommitedEvents();
            return uncommittedEvents;
        }

        public async Task<IEnumerable<Event>> SaveAsync<TAggregate>(TAggregate aggregate) where TAggregate : IAggregate
        {
            var uncommittedEvents = aggregate.UncommitedEvents().ToList();
            
            if (!_eventStore.ContainsKey(aggregate.AggregateId))
            {
                _eventStore.Add(aggregate.AggregateId, aggregate.UncommitedEvents().ToList());
            }
            else
            {
                _eventStore[aggregate.AggregateId].AddRange(aggregate.UncommitedEvents().ToList());
            }
            
            aggregate.ClearUncommitedEvents();
            
            return await Task.FromResult(uncommittedEvents.AsEnumerable());
        }

        public TResult GetById<TResult>(string correlationId) where TResult : IAggregate, new()
        {
            throw new AggregateNotFoundException("inmemory");
        }

        public TResult GetById<TResult>(string correlationId, int eventsToLoad) where TResult : IAggregate, new()
        {
            throw new AggregateNotFoundException("inmemory");
        }

        public void DeleteAggregate<TAggregate>(string correlationId, bool hard)
        {
            
        }
    }
}