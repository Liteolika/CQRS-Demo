using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public abstract class AggregateRoot : IAggregate
    {
        protected Guid _AggregateId { get; set; }
        protected int _AggregateVersion { get; set; }

        public Guid AggregateId { get { return _AggregateId; } }
        public int AggregateVersion { get { return _AggregateVersion; } }

        private List<Event> _UncommitedEvents = new List<Event>();

        public void ApplyChange(Event @event)
        {
            var eventType = @event.GetType();
            if (_routes.ContainsKey(eventType))
            {
                _routes[eventType](@event);
                _UncommitedEvents.Add(@event);
                _AggregateVersion++;
            }
        }

        public void LoadHistory(IEnumerable<Event> events)
        {
            foreach(var @event in events)
            {
                var eventType = @event.GetType();
                if (_routes.ContainsKey(eventType))
                    _routes[eventType](@event);
                _AggregateVersion++;
            }
        }

        public IEnumerable<Event> UncommittedEvents()
        {
            return _UncommitedEvents;
        }

        public void ClearUncommittedEvents()
        {
            _UncommitedEvents.Clear();
        }

        private Dictionary<Type, Action<Event>> _routes = new Dictionary<Type, Action<Event>>();
        protected void RegisterTransition<T>(Action<T> transition) where T : class
        {
            _routes.Add(typeof(T), o => transition(o as T));
        }

    }
}
