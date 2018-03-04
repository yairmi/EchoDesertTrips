using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Business.DbEntities
{
    public class TourHotelRoomType
    {
        public int HotelRoomTypeId { get; set; }
        public HotelRoomType HotelRoomType { get; set; }
        public int TourId { get; set; }
        public int Capacity { get; set; }
        public int Persons { get; set; }
    }
}
