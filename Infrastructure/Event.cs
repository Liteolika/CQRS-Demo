using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class Event : IEvent
    {
        public Guid EventId { get; set; }

        public Event()
        {
            this.EventId = Guid.NewGuid();
        }

    }
}
