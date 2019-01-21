using EchoDesertTrips.Business.Entities;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Data.Entity;
using System.Runtime.InteropServices;
using System.Linq.Expressions;
using System;

namespace EchoDesertTrips.Data.Data_Repositories
{
    [Export(typeof(ICustomerRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CustomerRepository : DataRepositoryBase<Customer>, ICustomerRepository
    {
        protected override DbSet<Customer> DbSet(EchoDesertTripsContext entityContext)
        {
            return entityContext.CustomerSet;
        }

        protected override Expression<Func<Customer, bool>> IdentifierPredicate(EchoDesertTripsContext entityContext, int id)
        {
            return (e => e.CustomerId == id);
        }

        protected override IEnumerable<Customer> GetEntities(EchoDesertTripsContext entityContext, int id)
        {
            return null;
        }
    }
}
