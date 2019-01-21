using Core.Common.Core;
using System;

namespace EchoDesertTrips.Client.Entities
{
    public class HotelRoomType : ObjectBase
    {
        public HotelRoomType()
        {
            StartDaysRange = DateTime.Today;
            EndDaysRange = DateTime.Today;
        }
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
        private DateTime _startDaysRange;
        public DateTime StartDaysRange
        {
            get
            {
                return _startDaysRange;
            }
            set
            {
                _startDaysRange = value;
                OnPropertyChanged(() => StartDaysRange);
            }
        }

        public DateTime _endDaysRange;
        public DateTime EndDaysRange
        {
            get
            {
                return _endDaysRange;
            }
            set
            {
                _endDaysRange = value;
                OnPropertyChanged(() => EndDaysRange);
            }
        }

        private float _pricePerPerson;

        public float PricePerPerson
        {
            get
            {
                return _pricePerPerson;
            }
            set
            {
                if (_pricePerPerson != value)
                {
                    _pricePerPerson = value;
                    OnPropertyChanged(() => PricePerPerson);
                }
            }
        }
    }
}
