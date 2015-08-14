using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public interface IHandleCommand<T> where T : Command
    {
        void Handle(T command);
    }
}
