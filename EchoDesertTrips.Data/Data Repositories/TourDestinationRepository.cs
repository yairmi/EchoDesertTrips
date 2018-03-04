using EchoDesertTrips.Business.Entities;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace EchoDesertTrips.Data.Data_Repositories
{
    [Export(typeof(ITourDestinationRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class TourDestinationRepository : DataRepositoryBase<TourDestination>, ITourDestinationRepository
    {
        protected override TourDestination AddEntity(EchoDesertTripsContext entityContext, TourDestination entity)
        {
            return entityContext.TourDestinationSet.Add(entity);
        }

        protected override TourDestination UpdateEntity(EchoDesertTripsContext entityContext, TourDestination entity)
        {
            return (from e in entityContext.TourDestinationSet where e.TourDestinationId == entity.TourDestinationId select e).FirstOrDefault();
        }

        protected override IEnumerable<TourDestination> GetEntities(EchoDesertTripsContext entityContext)
        {
            return (from e in entityContext.TourDestinationSet select e);
        }

        protected override IEnumerable<TourDestination> GetEntities(EchoDesertTripsContext entityContext, int id)
        {
            return null;
        }

        protected override TourDestination GetEntity(EchoDesertTripsContext entityContext, int id)
        {
            var query = (from e in entityContext.TourDestinationSet where e.TourDestinationId == id select e);
            var results = query.FirstOrDefault();

            return results;

        }

        protected override void LoadNavigationProperties(EchoDesertTripsContext entityContext, TourDestination existingEntity, TourDestination entity)
        {
        }
    }
}

