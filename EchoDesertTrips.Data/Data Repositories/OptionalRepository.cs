using EchoDesertTrips.Business.Entities;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Data.Data_Repositories
{
    [Export(typeof(IOptionalRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class OptionalRepository : DataRepositoryBase<Optional>, IOptionalRepository
    {
        protected override Optional AddEntity(EchoDesertTripsContext entityContext, Optional entity)
        {
            return entityContext.OptionalSet.Add(entity);
        }

        protected override Optional UpdateEntity(EchoDesertTripsContext entityContext, Optional entity)
        {
            return (from e in entityContext.OptionalSet where e.OptionalId == entity.OptionalId select e).FirstOrDefault();
        }

        protected override IEnumerable<Optional> GetEntities(EchoDesertTripsContext entityContext)
        {
            return (from e in entityContext.OptionalSet select e);
        }

        protected override IEnumerable<Optional> GetEntities(EchoDesertTripsContext entityContext, int id)
        {
            return null;
        }

        protected override Optional GetEntity(EchoDesertTripsContext entityContext, int id)
        {
            var query = (from e in entityContext.OptionalSet where e.OptionalId == id select e);
            var results = query.FirstOrDefault();

            return results;

        }
    }
}
