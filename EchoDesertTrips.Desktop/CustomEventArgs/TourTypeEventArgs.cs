using EchoDesertTrips.Client.Entities;
using System;

namespace EchoDesertTrips.Desktop.CustomEventArgs
{
    public class TourTypeEventArgs : EventArgs
    {
        public TourTypeEventArgs(TourType tourType, bool isNew)
        {
            TourType = tourType;
            IsNew = isNew;
        }
        public TourType TourType { get; set; }
        public bool IsNew { get; set; }
    }
}
