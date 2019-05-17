using Core.Common.Contracts;
using EchoDesertTrips.Business.Entities;
using System.Collections.Generic;

namespace EchoDesertTrips.Business.Common
{
    public interface IReservationEngine : IBusinessEngine
    {
        //void PrepareReservationsForTransmition(Reservation[] Reservations);
        Reservation[] GetReservationsByIds(List<int> idList, int customersAmount);
    }
}
