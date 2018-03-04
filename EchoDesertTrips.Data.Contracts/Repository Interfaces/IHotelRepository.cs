using Core.Common.Contracts;
using EchoDesertTrips.Business.Entities;
using EchoDesertTrips.Data.Contracts.DTOs;
using System.Collections.Generic;

namespace EchoDesertTrips.Data.Contracts.Repository_Interfaces
{
    public interface IHotelRepository : IDataRepository<Hotel>
    {
        IEnumerable<DTOHotelRoomTypesInfo> GetHotelsAndRoomTypes();
        IEnumerable<HotelRoomType> GetHotelRoomTypes();
        Hotel UpdateHotel(Hotel hotel);
    }
}
