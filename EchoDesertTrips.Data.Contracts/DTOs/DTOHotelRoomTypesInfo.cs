using EchoDesertTrips.Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Data.Contracts.DTOs
{
    public class DTOHotelRoomTypesInfo
    {
        public Hotel Hotel { get; set; }
        public HotelRoomType HotelRoomType { get; set; }
        public RoomType RoomType { get; set; }
    }
}
