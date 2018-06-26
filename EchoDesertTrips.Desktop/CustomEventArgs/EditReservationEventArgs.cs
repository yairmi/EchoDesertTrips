using Core.Common.Contracts;
using EchoDesertTrips.Client.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Desktop.CustomEventArgs
{
    public class EditReservationEventArgs : EventArgs
    {
        public EditReservationEventArgs(ReservationWrapper reservation)
        {
            _reservation = reservation;
        }
        public ReservationWrapper _reservation { get; set; }
    }
}
