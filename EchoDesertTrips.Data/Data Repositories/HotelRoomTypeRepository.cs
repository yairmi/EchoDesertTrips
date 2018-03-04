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
    [Export(typeof(IHotelRoomTypeRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class HotelRoomTypeRepository : DataRepositoryBase<HotelRoomType>, IHotelRoomTypeRepository
    {
        protected override HotelRoomType AddEntity(EchoDesertTripsContext entityContext, HotelRoomType entity)
        {
            return entityContext.HotelRoomTypeSet.Add(entity);
        }

        protected override HotelRoomType UpdateEntity(EchoDesertTripsContext entityContext, HotelRoomType entity)
        {
            return (from e in entityContext.HotelRoomTypeSet where e.HotelId == entity.HotelId
                    && e.RoomTypeId == entity.RoomTypeId select e).FirstOrDefault();
        }

        protected override IEnumerable<HotelRoomType> GetEntities(EchoDesertTripsContext entityContext)
        {
            var query = (from e in entityContext.HotelRoomTypeSet select e)
                /*.Include(h => h.Hotel)*/
                .Include(h => h.RoomType)
                .ToList();
                
            return query;
        }

        protected override IEnumerable<HotelRoomType> GetEntities(EchoDesertTripsContext entityContext, int id)
        {
            return null;
        }

        protected override HotelRoomType GetEntity(EchoDesertTripsContext entityContext, int id)
        {
            return null;
        }

        public HotelRoomType GetEntity(int hotelId, int RoomTypeId)
        {
            using (EchoDesertTripsContext entityContext = new EchoDesertTripsContext())
            {
                return (from e in entityContext.HotelRoomTypeSet
                        where e.HotelId == hotelId
                        && e.RoomTypeId == RoomTypeId
                        select e).FirstOrDefault();
            }
        }
    }
}
