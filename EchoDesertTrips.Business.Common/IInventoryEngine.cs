using Core.Common.Contracts;
using Core.Common.Core;
using EchoDesertTrips.Business.Entities;

namespace EchoDesertTrips.Business.Common
{
    public interface IInventoryEngine : IBusinessEngine
    {
        Hotel GetHotelById(int id);
        RoomType GetRoomTypeById(int id);
        Optional GetOptionalById(int id);
        TourType GetTourTypeById(int id);
        Operator GetOperatorById(int id);
        Agency GetAgencyById(int id);
    }
}
