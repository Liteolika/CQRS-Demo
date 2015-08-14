using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class EventStoreDomainRepository<T> : IDomainRepository<T> where T : IAggregate, new()
    {
        private readonly IEventStore _store;

        public EventStoreDomainRepository(IEventStore store)
        {
            _store = store;
        }

        public T GetById(Guid guid)
        {
            if (!_store.HasAggregate(guid))
                return default(T);

            var events = _store.GetEventsForAggregate(guid);
            var aggregate = BuildAggregate(events);
            return aggregate;
                 
        }

        private T BuildAggregate(IEnumerable<Event> events)
        {
            var aggregate = new T();
            aggregate.LoadHistory(events);
            return aggregate;
        }

        public void Store(T device)
        {
            var events = device.UncommittedEvents();
            _store.SaveEventsForAggregate(device.AggregateId, events);
            device.ClearUncommittedEvents();
        }
    }
}
