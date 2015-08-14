using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public interface IEventStore
    {
        bool HasAggregate(Guid guid);

        IEnumerable<Event> GetEventsForAggregate(Guid guid);

        void SaveEventsForAggregate(Guid guid, IEnumerable<Event> events);
    }
}
