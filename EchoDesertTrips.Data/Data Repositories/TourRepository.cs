using EchoDesertTrips.Business.Entities;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Data.Entity;

namespace EchoDesertTrips.Data.Data_Repositories
{
    [Export(typeof(ITourRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class TourRepository : DataRepositoryBase<Tour>, ITourRepository
    {
        protected override Tour AddEntity(EchoDesertTripsContext entityContext, Tour entity)
        {
            entityContext.TourSet.Add(entity);
            entityContext.TourTypeSet.Attach(entity.TourType);
            //foreach(var tourTypeDestination in entity.TourType.TourTypeDestinations)
            //    entityContext.TourDestinationSet.Attach(tourTypeDestination.TourDestination);

            return entity;
        }

        protected override Tour UpdateEntity(EchoDesertTripsContext entityContext, Tour entity)
        {
            //foreach (var tourOptional in entity.TourOptionals)
            //    entityContext.OptionalSet.Attach(tourOptional.Optional);

            return (from e in entityContext.TourSet where e.TourId == entity.TourId select e).FirstOrDefault();
        }

        protected override IEnumerable<Tour> GetEntities(EchoDesertTripsContext entityContext)
        {
            return entityContext.TourSet
                .Include(t => t.TourType)
                //.Include(t=>t.TourType.TourTypeDestinations)
                .Include(t => t.TourOptionals).ToList();
        }

        protected override IEnumerable<Tour> GetEntities(EchoDesertTripsContext entityContext, int id)
        {
            return null;
        }

        protected override Tour GetEntity(EchoDesertTripsContext entityContext, int id)
        {
            var query = (from e in entityContext.TourSet where e.TourId == id select e);
            var results = query.FirstOrDefault();

            return results;

        }

        //protected override void LoadNavigationProperties(EchoDesertTripsContext entityContext, Tour existingEntity, Tour entity)
        //{
        //    entityContext.Entry(existingEntity).Reference(n => n.TourType).Load();
        //}

        public void RemoveTour(int tourId)
        {
            using (EchoDesertTripsContext entityContext = new EchoDesertTripsContext())
            {
                var tour = (from t in entityContext.TourSet
                            where t.TourId == tourId
                            select t)
                   .Include(t => t.TourOptionals)
                   //.Include(t => t.TourHotelRoomTypes)
                   .Include(t => t.TourHotels)
                   .FirstOrDefault();
                if (tour.TourOptionals != null)
                {
                    var tourOptional = tour.TourOptionals.FirstOrDefault();
                    while (tourOptional != null)
                    {
                        entityContext.TourOptionalSet.Remove(tourOptional);
                        tourOptional = tour.TourOptionals.FirstOrDefault();
                    }
                }
                if (tour.TourHotels != null)
                {
                    tour.TourHotels.ForEach((tourHotel) =>
                        {
                            var tourHotelRoomType = tourHotel.TourHotelRoomTypes.FirstOrDefault();
                            while (tourHotelRoomType != null)
                            {
                                entityContext.TourHotelRoomTypesSet.Remove(tourHotelRoomType);
                                tourHotelRoomType = tourHotel.TourHotelRoomTypes.FirstOrDefault();
                            }
                        });
                }

                entityContext.Entry(tour).State = EntityState.Deleted;
                entityContext.SaveChanges();
            }
        }
    }
}
