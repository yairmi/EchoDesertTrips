using EchoDesertTrips.Client.Entities;
using System;

namespace Core.Common.UI.CustomEventArgs
{
    public class EditReservationEventArgs : EventArgs
    {
        public EditReservationEventArgs(Reservation reservation, bool viewMode, bool isContinual)
        {
            Reservation = reservation;
            ViewMode = viewMode;
            IsContinual = isContinual;
        }
        public Reservation Reservation { get; private set; }
        public bool ViewMode { get; private set; }
        public bool IsContinual { get; private set; }
    }
}
