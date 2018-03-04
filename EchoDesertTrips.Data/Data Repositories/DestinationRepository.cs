using EchoDesertTrips.Business.Entities;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace EchoDesertTrips.Data.Data_Repositories
{
    [Export(typeof(IDestinationRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DestinationRepository : DataRepositoryBase<Destination>, IDestinationRepository
    {
        protected override Destination AddEntity(EchoDesertTripsContext entityContext, Destination entity)
        {
            return entityContext.DestinationSet.Add(entity);
        }

        protected override Destination UpdateEntity(EchoDesertTripsContext entityContext, Destination entity)
        {
            return (from e in entityContext.DestinationSet where e.DestinationId == entity.DestinationId select e).FirstOrDefault();
        }

        protected override IEnumerable<Destination> GetEntities(EchoDesertTripsContext entityContext)
        {
            return (from e in entityContext.DestinationSet select e);
        }

        protected override IEnumerable<Destination> GetEntities(EchoDesertTripsContext entityContext, int id)
        {
            return null;
        }

        protected override Destination GetEntity(EchoDesertTripsContext entityContext, int id)
        {
            var query = (from e in entityContext.DestinationSet where e.DestinationId == id select e);
            var results = query.FirstOrDefault();

            return results;

        }

        protected override void LoadNavigationProperties(EchoDesertTripsContext entityContext, Destination existingEntity, Destination entity)
        {
        }
    }
}

