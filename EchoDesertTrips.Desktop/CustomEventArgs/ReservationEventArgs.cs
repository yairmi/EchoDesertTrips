using EchoDesertTrips.Client.Entities;
using System;

namespace EchoDesertTrips.Desktop.CustomEventArgs
{
    public class ReservationEventArgs : EventArgs
    {
        public ReservationEventArgs(Reservation reservation, bool isNew, bool isDbWon)
        {
            Reservation = reservation;
            IsNew = isNew;
            IsDbWon = isDbWon;
        }
        public Reservation Reservation { get; set; }
        public bool IsNew { get; set; }
        public bool IsDbWon { get; set; }
    }
}
