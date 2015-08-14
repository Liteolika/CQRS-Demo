using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    

    public interface IHandleEvent<T> where T : Event
    {
        void Handle(T @event);
    }

}
