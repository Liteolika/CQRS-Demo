using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class InMemoryDomainRepository<T> : IDomainRepository<T> where T : IAggregate
    {
        private static Dictionary<Guid, T> _store = new Dictionary<Guid, T>();

        public InMemoryDomainRepository()
        {
            
        }

        public T GetById(Guid aggregateId)
        {
            T aggregate;

            if (!_store.TryGetValue(aggregateId, out aggregate))
                return default(T);
            return aggregate;
        }

        public void Store(T aggregate)
        {
            T storedAggregate;
            if (!_store.TryGetValue(aggregate.AggregateId, out storedAggregate))
                _store.Add(aggregate.AggregateId, aggregate);
            else
                storedAggregate = aggregate;



            Console.WriteLine("Stored aggregate {0}", aggregate.AggregateId);

        }

    }

    

}
