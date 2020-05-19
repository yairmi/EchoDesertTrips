using EchoDesertTrips.Client.Entities;
using System;

namespace Core.Common.UI.CustomEventArgs
{
    public class HotelEventArgs : EventArgs
    {
        public HotelEventArgs(Hotel hotel, bool isNew, bool sendUpdateToClients = true)
        {
            Hotel = hotel;
            bIsNew = isNew;
            bSendUpdateToClients = sendUpdateToClients;
        }
        public Hotel Hotel { get; private set; }
        public bool bIsNew { get; private set; }
        public bool bSendUpdateToClients { get; private set; }
    }
}
