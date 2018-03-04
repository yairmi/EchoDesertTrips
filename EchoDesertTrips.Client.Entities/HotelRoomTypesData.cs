using Core.Common.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Client.Entities
{
    public class HotelRoomTypesData : NotificationObject
    {
        private RoomType _hotel;

        public RoomType Hotel
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
                    OnPropertyChanged(() => Hotel);
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
                    OnPropertyChanged(() => RoomType);
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
                    OnPropertyChanged(() => PricePerChild);
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
                    OnPropertyChanged(() => PricePerAdult);
                }
            }
        }
    }
}
