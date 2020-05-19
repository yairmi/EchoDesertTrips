using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Business.DbEntities
{
    public class HotelRoomType
    {
        [Key]
        public int HotelRoomTypeId { get; set; }
        public int HotelId { get; set; }
        public int RoomTypeId { get; set; }
        public RoomType RoomType { get; set; }
        public decimal PricePerPerson { get; set; }
        public bool Visible { get; set; }
    }
}
