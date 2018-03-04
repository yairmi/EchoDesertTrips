using EchoDesertTrips.Business.Entities;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Data.Data_Repositories
{
    [Export(typeof(ITourOptionalRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class TourOptionalRepository : DataRepositoryBase<TourOptional>, ITourOptionalRepository
    {
        protected override TourOptional AddEntity(EchoDesertTripsContext entityContext, TourOptional entity)
        {
            entityContext.TourOptionalSet.Add(entity);
            entityContext.OptionalSet.Attach(entity.Optional);
            return entity;
        }

        protected override TourOptional UpdateEntity(EchoDesertTripsContext entityContext, TourOptional entity)
        {
            //return (from e in entityContext.TourOptional where e.TourId == entity.TourId select e).FirstOrDefault();
            return null;
        }

        protected override IEnumerable<TourOptional> GetEntities(EchoDesertTripsContext entityContext)
        {
            //return entityContext.TourSet
            //    .Include(n => n.LodggingPlace)
            //    .Include(t => t.TourType)
            //    .Include(t => t.TourType.TourDestination).ToList();
            return null;
        }

        protected override IEnumerable<TourOptional> GetEntities(EchoDesertTripsContext entityContext, int id)
        {
            return entityContext.TourOptionalSet.Where(t => t.TourId == id).ToList();
        }

        protected override TourOptional GetEntity(EchoDesertTripsContext entityContext, int id)
        {
            //var query = (from e in entityContext.TourSet where e.TourId == id select e);
            //var results = query.FirstOrDefault();

            //return results;

            return null;

        }

        public IEnumerable<TourOptional> GetTourOptionalsByTourId(int tourId)
        {
            using (EchoDesertTripsContext entityContext = new EchoDesertTripsContext())
            {
                return (from e in entityContext.TourOptionalSet where e.TourId == tourId select e)
                    //.Include(t => t.Tour)
                    .Include(o => o.Optional).ToList();
            }
        }
    }
}
