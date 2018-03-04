using EchoDesertTrips.Business.Entities;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System.Linq;

namespace EchoDesertTrips.Data.Data_Repositories
{
    [Export(typeof(ITourHotelRoomTypeRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class TourHotelRoomTypeRepository : DataRepositoryBase<TourHotelRoomType>, ITourHotelRoomTypeRepository
    {
        protected override TourHotelRoomType AddEntity(EchoDesertTripsContext entityContext, TourHotelRoomType entity)
        {
            entityContext.TourHotelRoomTypesSet.Add(entity);
            entityContext.HotelRoomTypeSet.Attach(entity.HotelRoomType);
            entityContext.RoomTypeSet.Attach(entity.HotelRoomType.RoomType);
            return entity;
        }

        protected override TourHotelRoomType UpdateEntity(EchoDesertTripsContext entityContext, TourHotelRoomType entity)
        {
            return (from e in entityContext.TourHotelRoomTypesSet where 
                    e.HotelRoomTypeId == entity.HotelRoomTypeId &&
                    e.TourId == entity.TourId select e).FirstOrDefault();
        }

        protected override IEnumerable<TourHotelRoomType> GetEntities(EchoDesertTripsContext entityContext)
        {
            return (from e in entityContext.TourHotelRoomTypesSet select e);
        }

        protected override IEnumerable<TourHotelRoomType> GetEntities(EchoDesertTripsContext entityContext, int id)
        {
            return (from e in entityContext.TourHotelRoomTypesSet where e.TourId == id select e);
        }

        protected override TourHotelRoomType GetEntity(EchoDesertTripsContext entityContext, int id)
        {
            //var query = (from e in entityContext.TourHotelRoomTypesSet where e.HotelRoomTypeId == id select e);
            //var results = query.FirstOrDefault();

            return null;

        }
    }
}
