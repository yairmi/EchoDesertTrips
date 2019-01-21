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
    [Export(typeof(ITourOptionalRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class TourOptionalRepository : DataRepositoryBase<TourOptional>, ITourOptionalRepository
    {
        protected override DbSet<TourOptional> DbSet(EchoDesertTripsContext entityContext)
        {
            return entityContext.TourOptionalSet;
        }

        protected override Expression<Func<TourOptional, bool>> IdentifierPredicate(EchoDesertTripsContext entityContext, int id)
        {
            return null;
        }

        protected override TourOptional AddEntity(EchoDesertTripsContext entityContext, TourOptional entity)
        {
            entityContext.TourOptionalSet.Add(entity);
            entityContext.OptionalSet.Attach(entity.Optional);
            return entity;
        }

        protected override TourOptional UpdateEntity(EchoDesertTripsContext entityContext, TourOptional entity)
        {
            return null;
        }

        protected override IEnumerable<TourOptional> GetEntities(EchoDesertTripsContext entityContext)
        {
            return null;
        }

        protected override IEnumerable<TourOptional> GetEntities(EchoDesertTripsContext entityContext, int id)
        {
            return entityContext.TourOptionalSet.Where(t => t.TourId == id);
        }

        protected override TourOptional GetEntity(EchoDesertTripsContext entityContext, int id)
        {
            return null;
        }

        public IEnumerable<TourOptional> GetTourOptionalsByTourId(int tourId)
        {
            using (EchoDesertTripsContext entityContext = new EchoDesertTripsContext())
            {
                return (from e in entityContext.TourOptionalSet where e.TourId == tourId select e).Include(o => o.Optional).ToList();
            }
        }
    }
}
