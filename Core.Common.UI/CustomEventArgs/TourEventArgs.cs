using System;
using EchoDesertTrips.Client.Entities;

namespace Core.Common.UI.CustomEventArgs
{
    public class TourEventArgs : EventArgs
    {
        public TourEventArgs(Tour tour, bool isNew)
        {
            Tour = tour;
            IsNew = isNew;
        }
        public Tour Tour { get; private set; }
        public bool IsNew { get; private set; }
    }
}
