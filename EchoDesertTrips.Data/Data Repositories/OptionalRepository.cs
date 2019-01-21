using EchoDesertTrips.Business.Entities;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Data.Data_Repositories
{
    [Export(typeof(IOptionalRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class OptionalRepository : DataRepositoryBase<Optional>, IOptionalRepository
    {
        protected override DbSet<Optional> DbSet(EchoDesertTripsContext entityContext)
        {
            return entityContext.OptionalSet;
        }

        protected override Expression<Func<Optional, bool>> IdentifierPredicate(EchoDesertTripsContext entityContext, int id)
        {
            return (e => e.OptionalId == id);
        }

        protected override IEnumerable<Optional> GetEntities(EchoDesertTripsContext entityContext, int id)
        {
            return null;
        }
    }
}
