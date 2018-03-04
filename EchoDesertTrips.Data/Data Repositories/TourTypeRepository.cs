using EchoDesertTrips.Business.Entities;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Data.Entity;

namespace EchoDesertTrips.Data.Data_Repositories
{
    [Export(typeof(ITourTypeRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class TourTypeRepository : DataRepositoryBase<TourType>, ITourTypeRepository
    {
        protected override TourType AddEntity(EchoDesertTripsContext entityContext, TourType entity)
        {
            return entityContext.TourTypeSet.Add(entity);
        }

        protected override TourType UpdateEntity(EchoDesertTripsContext entityContext, TourType entity)
        {
            return (from e in entityContext.TourTypeSet where e.TourTypeId == entity.TourTypeId select e)
                .FirstOrDefault();
        }

        protected override IEnumerable<TourType> GetEntities(EchoDesertTripsContext entityContext)
        {
            return (from e in entityContext.TourTypeSet select e).Include(t => t.TourTypeDescriptions);
        }

        protected override IEnumerable<TourType> GetEntities(EchoDesertTripsContext entityContext, int id)
        {
            return null;
        }

        protected override TourType GetEntity(EchoDesertTripsContext entityContext, int id)
        {
            var query = (from e in entityContext.TourTypeSet where e.TourTypeId == id select e)
                .Include(t => t.TourTypeDescriptions);
            var results = query.FirstOrDefault();

            return results;

        }

        public TourType UpdateTourType(TourType tourType)
        {
            using (EchoDesertTripsContext entityContext = new EchoDesertTripsContext())
            {
                var exsitingTourType = (from e in entityContext.TourTypeSet where e.TourTypeId == tourType.TourTypeId select e)
                             .Include(h => h.TourTypeDescriptions).FirstOrDefault();
                entityContext.Entry(exsitingTourType).CurrentValues.SetValues(tourType);
                //Hotel Room Types
                if (tourType.TourTypeDescriptions != null)
                    foreach (var tourTypeDescription in tourType.TourTypeDescriptions)
                    {
                        var existingTourDescription = (from e in entityContext.TourTypeDescriptionSet
                                                     where
                                                     e.TourTypeDescriptionId == tourTypeDescription.TourTypeDescriptionId //&&
                                                     select e).FirstOrDefault();
                        if (existingTourDescription != null)
                        {
                            entityContext.Entry(existingTourDescription).CurrentValues.SetValues(tourTypeDescription);
                        }
                        else //new 
                        {
                            exsitingTourType.TourTypeDescriptions.Add(tourTypeDescription);
                        }
                    }
                entityContext.SaveChanges();

                return exsitingTourType;
            }
        }
    }
}
