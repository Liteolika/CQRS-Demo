using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public interface IDomainRepository<IAggregate>
    {
        IAggregate GetById(Guid guid);

        void Store(IAggregate device);
    }
}
