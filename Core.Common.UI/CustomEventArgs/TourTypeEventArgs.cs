using EchoDesertTrips.Client.Entities;
using System;

namespace Core.Common.UI.CustomEventArgs
{
    public class TourTypeEventArgs : EventArgs
    {
        public TourTypeEventArgs(TourType tourType, bool isNew, bool sendUpdateToClients = true)
        {
            TourType = tourType;
            bIsNew = isNew;
            bSendUpdateToClients = sendUpdateToClients;
        }
        public TourType TourType { get; set; }
        public bool bIsNew { get; set; }
        public bool bSendUpdateToClients { get; set; }
    }
}
