using System;
using EchoDesertTrips.Client.Entities;

namespace EchoDesertTrips.Desktop.Support
{
    public class TourEventArgs : EventArgs
    {
        public TourEventArgs(Tour tour, bool isNew)
        {
            Tour = tour;
            IsNew = isNew;
        }

        public Tour Tour { get; set; }
        public bool IsNew { get; set; }
    }
}
