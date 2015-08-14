using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public interface ICommandBus
    {
        void SendCommand<T>(T command) where T : Command;
    }
}
