using Core.Common.Core;
using System;
using System.Collections.Generic;

namespace EchoDesertTrips.Client.Entities
{
    public class HotelRoomType : ObjectBase
    {
        private int _hotelRoomTypeId;

        public int HotelRoomTypeId
        {
            get
            {
                return _hotelRoomTypeId;
            }
            set
            {
                if (_hotelRoomTypeId!= value)
                {
                    _hotelRoomTypeId = value;
                    OnPropertyChanged(() => HotelRoomTypeId, true);
                }
            }
        }

        public int HotelId { get; set; }

        private string _hotelName;

        public string HotelName
        {
            get
            {
                return _hotelName;
            }
            set
            {
                _hotelName = value;
                OnPropertyChanged(() => HotelName, true);
            }
        }

        public int RoomTypeId { get; set; }

        private RoomType _roomType;

        public RoomType RoomType
        {
            get
            {
                return _roomType;
            }
            set
            {
                if (_roomType != value)
                {
                    _roomType = value;
                    OnPropertyChanged(() => RoomType);
                }
            }
        }

        private List<HotelRoomTypeDaysRange> _hotelRoomTypeDaysRanges;
        public List<HotelRoomTypeDaysRange> HotelRoomTypeDaysRanges
        {
            get
            {
                return _hotelRoomTypeDaysRanges;
            }
            set
            {
                _hotelRoomTypeDaysRanges = value;
                OnPropertyChanged(() => HotelRoomTypeDaysRanges, true);
            }
        }
    }
}
