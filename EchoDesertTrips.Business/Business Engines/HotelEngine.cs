using Core.Common.Contracts;
using EchoDesertTrips.Business.Common;
using EchoDesertTrips.Business.Entities;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Business.Business_Engines
{
    [Export(typeof(IHotelEngine))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class HotelEngine : IHotelEngine
    {
        [ImportingConstructor]
        public HotelEngine(IDataRepositoryFactory dataRepositoryFactory)
        {
            _DataRepositoryFactory = dataRepositoryFactory;
        }

        IDataRepositoryFactory _DataRepositoryFactory;

        public IEnumerable<Hotel> GetHotelsData()
        {
            IHotelRepository HotelRepository =
                  _DataRepositoryFactory.GetDataRepository<IHotelRepository>();

            IRoomTypeRepository RoomTypeRepository =
                  _DataRepositoryFactory.GetDataRepository<IRoomTypeRepository>();

            IHotelRoomTypeRepository HotelRoomTypeRepository =
                  _DataRepositoryFactory.GetDataRepository<IHotelRoomTypeRepository>();


            var hotels = HotelRepository.Get();
            var hotelRoomTypes = HotelRoomTypeRepository.Get();
            var roomTypes = RoomTypeRepository.Get();


            foreach (var hotel in hotels)
            {
                hotel.HotelRoomTypes = null;
                var hrt = hotelRoomTypes.ToList().FindAll(h => h.HotelId == hotel.HotelId);
                hrt.ForEach((hotelRoomType) =>
                {
                    hotelRoomType.RoomType = roomTypes.FirstOrDefault(r => r.RoomTypeId == hotelRoomType.RoomTypeId);
                    //hotelRoomType.Hotel = null;
                });
                hotel.HotelRoomTypes = hrt;
            }

            return hotels;
        }
    }
}
