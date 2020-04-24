using Core.Common.Contracts;
using EchoDesertTrips.Business.Entities;

namespace EchoDesertTrips.Data.Contracts.Repository_Interfaces
{
    public interface IHotelRepository : IDataRepository<Hotel>
    {
        Hotel AddHotel(Hotel hotel);
        Hotel UpdateHotel(Hotel hotel);
    }
}
