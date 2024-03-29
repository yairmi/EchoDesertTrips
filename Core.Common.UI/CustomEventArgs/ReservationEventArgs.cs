﻿using EchoDesertTrips.Client.Entities;
using System;

namespace Core.Common.UI.CustomEventArgs
{
    public class ReservationEventArgs : EventArgs
    {
        public ReservationEventArgs(Reservation reservation, bool isNew, bool isDbWon)
        {
            Reservation = reservation;
            IsNew = isNew;
            IsDbWon = isDbWon;
        }
        public Reservation Reservation { get; private set; }
        public bool IsNew { get; private set; }
        public bool IsDbWon { get; private set; }
    }
}
