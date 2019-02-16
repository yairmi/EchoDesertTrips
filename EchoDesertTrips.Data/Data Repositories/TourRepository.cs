using EchoDesertTrips.Business.Entities;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;
using System;
using Z.EntityFramework.Plus;

namespace EchoDesertTrips.Data.Data_Repositories
{
    [Export(typeof(ITourRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class TourRepository : DataRepositoryBase<Tour>, ITourRepository
    {
        protected override DbSet<Tour> DbSet(EchoDesertTripsContext entityContext)
        {
            return entityContext.TourSet;
        }

        protected override Expression<Func<Tour, bool>> IdentifierPredicate(EchoDesertTripsContext entityContext, int id)
        {
            return (e => e.TourId == id);
        }

        protected override Tour AddEntity(EchoDesertTripsContext entityContext, Tour entity)
        {
            entityContext.TourSet.Add(entity);
            entityContext.TourTypeSet.Attach(entity.TourType);
            return entity;
        }

        protected override IEnumerable<Tour> GetEntities(EchoDesertTripsContext entityContext, int id)
        {
            return null;
        }

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

        protected override IQueryable<Tour> IncludeNavigationProperties(IQueryable<Tour> query)
        {
            return query
                .IncludeOptimized(t => t.TourType)
                .IncludeOptimized(t => t.TourOptionals);
        }
    }
}
