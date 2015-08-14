using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Storage
{
    public class InMemoryEventStore : IEventStore
    {

        private class EventData
        {
            public Guid AggregateId { get; set; }
            public Event Event { get; set; }
        }

        private static ConcurrentDictionary<Guid, List<EventData>> _eventStore = new ConcurrentDictionary<Guid, List<EventData>>();

        private readonly IEventBus _eventBus;

        public InMemoryEventStore(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public bool HasAggregate(Guid guid)
        {
            return _eventStore.ContainsKey(guid);
        }

        public IEnumerable<Event> GetEventsForAggregate(Guid guid)
        {
            List<EventData> storedEvents;
            if(!_eventStore.TryGetValue(guid, out storedEvents))
            {
                throw new Exception("AggregateId not found");
            }
            return storedEvents.Select(x => x.Event);
        }

        public void SaveEventsForAggregate(Guid guid, IEnumerable<Event> events)
        {
            List<EventData> storedEvents;
            if (!_eventStore.TryGetValue(guid, out storedEvents))
            {
                storedEvents = new List<EventData>();
                //_eventStore.Add(guid, storedEvents);
                _eventStore.TryAdd(guid, storedEvents);
            }
            
            foreach(var @event in events)
            {
                storedEvents.Add(new EventData() {
                    AggregateId = guid,
                    Event = @event
                });
            }

            _eventBus.PublishEvents(events);

        }
    }
}
