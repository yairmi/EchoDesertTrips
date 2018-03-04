using System;
using EchoDesertTrips.Client.Entities;

namespace EchoDesertTrips.Desktop.Support
{
    public class TourEventArgs : EventArgs
    {
        public TourEventArgs(TourWrapper tour, bool isNew)
        {
            Tour = tour;
            IsNew = isNew;
        }

        public TourWrapper Tour { get; set; }
        public bool IsNew { get; set; }
    }
}
