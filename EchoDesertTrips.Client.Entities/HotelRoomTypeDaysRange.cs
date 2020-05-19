using Core.Common.Core;
using System;

namespace EchoDesertTrips.Client.Entities
{
    public class HotelRoomTypeDaysRange : ObjectBase
    {
        public HotelRoomTypeDaysRange()
        {
            _startDaysRange = DateTime.Now;
            _endDaysRange = DateTime.Now;
        }
        private int _hotelRoomTypeDaysRangeId;
        public int HotelRoomTypeDaysRangeId
        {
            get
            {
                return _hotelRoomTypeDaysRangeId;
            }
            set
            {
                _hotelRoomTypeDaysRangeId = value;
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
                OnPropertyChanged(() => StartDaysRange, true);
            }
        }

        private DateTime _endDaysRange;
        public DateTime EndDaysRange
        {
            get
            {
                return _endDaysRange;
            }
            set
            {
                _endDaysRange = value;
                OnPropertyChanged(() => EndDaysRange, true);
            }
        }

        private decimal _pricePerPerson;
        public decimal PricePerPerson
        {
            get
            {
                return _pricePerPerson;
            }
            set
            {
                _pricePerPerson = value;
                OnPropertyChanged(() => PricePerPerson, true);
            }
        }
    }
}
