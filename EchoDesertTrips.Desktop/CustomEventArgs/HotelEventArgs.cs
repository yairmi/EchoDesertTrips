using EchoDesertTrips.Client.Entities;
using System;

namespace EchoDesertTrips.Desktop.CustomEventArgs
{
    public class HotelEventArgs : EventArgs
    {
        public HotelEventArgs(Hotel hotel, bool isNew)
        {
            Hotel = hotel;
            IsNew = isNew;
        }
        public Hotel Hotel { get; set; }
        public bool IsNew { get; set; }
    }
}
