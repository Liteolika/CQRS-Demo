using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public abstract class Command : ICommand
    {
        public Guid CommandId { get; set; }

        public Command()
        {
            this.CommandId = Guid.NewGuid();
        }
    }
}
