using EchoDesertTrips.Client.Entities;

namespace EchoDesertTrips.Desktop.Support
{
    public class TourTypeEventArgs
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
