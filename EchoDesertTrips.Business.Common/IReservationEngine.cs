using Core.Common.Contracts;
using EchoDesertTrips.Business.Entities;
using System.Collections.Generic;

namespace EchoDesertTrips.Business.Common
{
    public interface IReservationEngine : IBusinessEngine
    {
        ReservationDTO[] GetReservationsByIds(List<int> idList);
    }
}
