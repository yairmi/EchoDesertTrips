using Core.Common.Contracts;
using EchoDesertTrips.Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
