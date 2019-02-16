using Core.Common.Contracts;
using EchoDesertTrips.Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Business.Common
{
    public interface IReservationEngine : IBusinessEngine
    {
        void PrepareReservationsForTransmition(Reservation[] Reservations);
        Reservation[] GetReservationsByIds(List<int> idList);
    }
}
