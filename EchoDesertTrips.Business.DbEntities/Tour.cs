using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Business.DbEntities
{
    public class Tour
    {
        public int TourId { get; set; }
        public TourType TourType { get; set; }
        public int TourTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PickupAddress { get; set; }
        public List<TourOptional> TourOptionals { get; set; }
        public List<TourHotelRoomType> TourHotelRoomTypes { get; set; }
    }
}
