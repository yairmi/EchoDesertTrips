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
        public EditReservationEventArgs(Reservation reservation, bool viewMode, bool isContinual)
        {
            Reservation = reservation;
            ViewMode = viewMode;
            IsContinual = isContinual;
        }
        public Reservation Reservation { get; set; }
        public bool ViewMode { get; set; }
        public bool IsContinual { get; set; }
    }
}
