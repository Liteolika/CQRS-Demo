using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public interface IAggregate
    {
        Guid AggregateId { get; }
        void ApplyChange(Event @event);
        IEnumerable<Event> UncommittedEvents();
        void ClearUncommittedEvents();
        void LoadHistory(IEnumerable<Event> events);
    }
}
