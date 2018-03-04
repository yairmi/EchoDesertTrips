using EchoDesertTrips.Business.Contracts;
using EchoDesertTrips.Business.Entities;
using EchoDesertTrips.Data.Contracts.DTOs;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;

namespace EchoDesertTrips.Data.Data_Repositories
{
    [Export(typeof(IHotelRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class HotelRepository : DataRepositoryBase<Hotel>, IHotelRepository
    {
        protected override Hotel AddEntity(EchoDesertTripsContext entityContext, Hotel entity)
        {
            foreach(var hotelRoomType in entity.HotelRoomTypes)
            {
                entityContext.HotelRoomTypeSet.Add(hotelRoomType);
                entityContext.RoomTypeSet.Attach(hotelRoomType.RoomType);
            }

            return entityContext.HotelSet.Add(entity);
        }

        public Hotel UpdateHotel(Hotel hotel)
        {
            using (EchoDesertTripsContext entityContext = new EchoDesertTripsContext())
            {
                var exsitingHotel = (from e in entityContext.HotelSet where e.HotelId == hotel.HotelId select e)
                             .Include(h => h.HotelRoomTypes.Select(r => r.RoomType))
                             .FirstOrDefault();
                entityContext.Entry(exsitingHotel).CurrentValues.SetValues(hotel);
                //Hotel Room Types
                if (hotel.HotelRoomTypes != null)
                    foreach (var hotelRoomType in hotel.HotelRoomTypes)
                    {
                        var existingHotelRoomType = (from e in entityContext.HotelRoomTypeSet
                                                    where 
                                                    e.HotelId == hotelRoomType.HotelId &&
                                                    e.RoomTypeId == hotelRoomType.RoomTypeId
                                                     select e).FirstOrDefault();
                        if (existingHotelRoomType != null)
                        {
                            entityContext.Entry(existingHotelRoomType).CurrentValues.SetValues(hotelRoomType);
                        }
                        else //new HotelRoomType
                        {
                            entityContext.HotelRoomTypeSet.Add(hotelRoomType);
                            entityContext.RoomTypeSet.Attach(hotelRoomType.RoomType);
                        }
                    }
                entityContext.SaveChanges();

                return exsitingHotel;
            }
        }

        protected override Hotel UpdateEntity(EchoDesertTripsContext entityContext, Hotel entity)
        {
            return (from e in entityContext.HotelSet where e.HotelId == entity.HotelId select e).FirstOrDefault();
        }

        protected override IEnumerable<Hotel> GetEntities(EchoDesertTripsContext entityContext)
        {
            var query = (from e in entityContext.HotelSet select e)
                .Include(h => h.HotelRoomTypes.Select(r => r.RoomType)).ToList();
            return query;
        }

        protected override IEnumerable<Hotel> GetEntities(EchoDesertTripsContext entityContext, int id)
        {
            var query = (from e in entityContext.HotelSet select e)
                .Where(h => h.HotelId == id)
                .Include(h => h.HotelRoomTypes.Select(r => r.RoomType)).ToList();
            return query;

        }

        protected override Hotel GetEntity(EchoDesertTripsContext entityContext, int id)
        {
            var query = (from e in entityContext.HotelSet where e.HotelId == id select e);
            var results = query.FirstOrDefault();

            return results;

        }

        public IEnumerable<HotelRoomType> GetHotelRoomTypes()
        {
            return null;
        }

        public IEnumerable<DTOHotelRoomTypesInfo> GetHotelsAndRoomTypes()
        {
            using (EchoDesertTripsContext entityContext = new EchoDesertTripsContext())
            {
                var query = from hotel in entityContext.HotelSet
                            join hotelRoomType in entityContext.HotelRoomTypeSet on hotel.HotelId equals hotelRoomType.HotelId
                            join roomType in entityContext.RoomTypeSet on hotelRoomType.RoomTypeId equals roomType.RoomTypeId
                            select new DTOHotelRoomTypesInfo()
                            {
                                Hotel = hotel,
                                HotelRoomType = hotelRoomType,
                                RoomType = roomType
                            };

                return query.ToList().ToArray();
            }
        }
    }
}
