using EchoDesertTrips.Client.Entities;
using System;

namespace EchoDesertTrips.Desktop.CustomEventArgs
{
    public class TourTypeEventArgs : EventArgs
    {
        public TourTypeEventArgs(TourTypeWrapper tourType, bool isNew)
        {
            TourType = tourType;
            IsNew = isNew;
        }
        public TourTypeWrapper TourType { get; set; }
        public bool IsNew { get; set; }
    }
}
