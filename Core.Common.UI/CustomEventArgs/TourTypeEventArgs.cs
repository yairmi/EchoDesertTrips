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
        public TourType TourType { get; private set; }
        public bool bIsNew { get; private set; }
        public bool bSendUpdateToClients { get; private set; }
    }
}
