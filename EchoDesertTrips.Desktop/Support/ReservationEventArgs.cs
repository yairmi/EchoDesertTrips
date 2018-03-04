using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Desktop.Support
{
    public class ReservationEventArgs : EventArgs
    {
        public ReservationEventArgs(ReservationWrapper reservation, bool isNew)
        {
            Reservation = reservation;
            IsNew = isNew;
        }
        public ReservationWrapper Reservation { get; set; }
        public bool IsNew { get; set; }
    }
}
