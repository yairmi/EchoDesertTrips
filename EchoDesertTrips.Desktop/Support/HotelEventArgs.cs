using EchoDesertTrips.Client.Entities;

namespace EchoDesertTrips.Desktop.Support
{
    public class HotelEventArgs
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
