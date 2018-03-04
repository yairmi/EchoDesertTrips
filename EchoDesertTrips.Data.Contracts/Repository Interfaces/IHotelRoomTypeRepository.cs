using Core.Common.Contracts;
using EchoDesertTrips.Business.Entities;

namespace EchoDesertTrips.Data.Contracts.Repository_Interfaces
{
    public interface IHotelRoomTypeRepository : IDataRepository<HotelRoomType>
    {
        HotelRoomType GetEntity(int hotelId, int RoomTypeId);
    }
}
