using Core.Common.Core;

namespace EchoDesertTrips.Client.Entities
{
    public class HotelRoomTypeWrapper : ObjectBase
    {
        public int HotelId { get; set; }
        public int RoomTypeId { get; set; }

        private Hotel _hotel;

        public Hotel Hotel
        {
            get
            {
                return _hotel;
            }
            set
            {
                if (_hotel != value)
                {
                    _hotel = value;
                    OnPropertyChanged(() => Hotel, true);
                }
            }
        }

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
                    OnPropertyChanged(() => RoomType, true);
                }
            }
        }

        private float _pricePerChild;

        public float PricePerChild
        {
            get
            {
                return _pricePerChild;
            }
            set
            {
                if (_pricePerChild != value)
                {
                    _pricePerChild = value;
                    OnPropertyChanged(() => PricePerChild, true);
                }
            }
        }

        private float _pricePerAdult;

        public float PricePerAdult
        {
            get
            {
                return _pricePerAdult;
            }
            set
            {
                if (_pricePerAdult != value)
                {
                    _pricePerAdult = value;
                    OnPropertyChanged(() => PricePerAdult, true);
                }
            }
        }
    }
}
