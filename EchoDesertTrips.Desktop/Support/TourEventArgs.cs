using System;
using EchoDesertTrips.Client.Entities;

namespace EchoDesertTrips.Desktop.Support
{
    public class TourEventArgs : EventArgs
    {
        public TourEventArgs(Tour tour, int removedItems, bool isNew)
        {
            Tour = tour;
            RemovedItems = removedItems;
            IsNew = isNew;
        }
        public Tour Tour { get; set; }
        public int RemovedItems { get; set; }
        public bool IsNew { get; set; }
    }
}
