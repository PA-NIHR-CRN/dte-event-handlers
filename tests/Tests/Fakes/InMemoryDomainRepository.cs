using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Evento;

namespace Tests.Fakes
{
    public class InMemoryDomainRepository : IDomainRepository
    {
        private readonly Dictionary<string, List<Event>> _eventStore = new Dictionary<string, List<Event>>();
        private IAggregate _aggregate;

        public Dictionary<string, List<Event>> EventStore => _eventStore;
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
            _aggregate = aggregate;
            return uncommittedEvents;
        }

        public Task<IEnumerable<Event>> SaveAsync<TAggregate>(TAggregate aggregate) where TAggregate : IAggregate
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
            _aggregate = aggregate;
            return new Task<IEnumerable<Event>>(uncommittedEvents.AsEnumerable);
        }

        public TResult GetById<TResult>(string correlationId) where TResult : IAggregate, new()
        {
            if (_aggregate != null && _aggregate.AggregateId.EndsWith(correlationId))
                return (TResult) _aggregate;
            throw new AggregateNotFoundException("inmemory");
        }

        public TResult GetById<TResult>(string correlationId, int eventsToLoad) where TResult : IAggregate, new()
        {
            if (_aggregate != null && _aggregate.AggregateId.EndsWith(correlationId))
                return (TResult)_aggregate;
            throw new AggregateNotFoundException("inmemory");
        }

        public void DeleteAggregate<TAggregate>(string correlationId, bool hard)
        {

        }
    }
}