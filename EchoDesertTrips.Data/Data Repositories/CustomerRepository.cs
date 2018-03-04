using EchoDesertTrips.Business.Entities;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Data.Entity;
using System.Runtime.InteropServices;

namespace EchoDesertTrips.Data.Data_Repositories
{
    [Export(typeof(ICustomerRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CustomerRepository : DataRepositoryBase<Customer>, ICustomerRepository
    {
        protected override Customer AddEntity(EchoDesertTripsContext entityContext, Customer entity)
        {
            return entityContext.CustomerSet.Add(entity);
        }

        protected override Customer UpdateEntity(EchoDesertTripsContext entityContext, Customer entity)
        {
            return (from e in entityContext.CustomerSet where e.CustomerId == entity.CustomerId select e).FirstOrDefault();
        }

        protected override IEnumerable<Customer> GetEntities(EchoDesertTripsContext entityContext)
        {
            var query = (from e in entityContext.CustomerSet select e);//.Include(n => n.Nationality);
                //.Include(c => c.Reservations.Select(t => t.Tours));

            return query;
        }

        protected override IEnumerable<Customer> GetEntities(EchoDesertTripsContext entityContext, int id)
        {
            return null;
        }

        protected override Customer GetEntity(EchoDesertTripsContext entityContext, int id)
        {
            var query = (from e in entityContext.CustomerSet where e.CustomerId == id select e);
            var results = query.FirstOrDefault();

            return results;

        }
    }
}
