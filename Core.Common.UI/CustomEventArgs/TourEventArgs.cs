using System;
using EchoDesertTrips.Client.Entities;

namespace Core.Common.UI.CustomEventArgs
{
    public class TourEventArgs : EventArgs
    {
        public TourEventArgs(Tour tour/*, bool isDirty*/, bool isNew)
        {
            Tour = tour;
            //IsDirty = isDirty;
            IsNew = isNew;
        }
        public Tour Tour { get; set; }
        //public bool  IsDirty { get; set; }
        public bool IsNew { get; set; }
    }
}
