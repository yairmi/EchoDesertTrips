using Core.Common.Core;
using System;
using System.Collections.ObjectModel;

namespace EchoDesertTrips.Client.Entities
{
    public class TourHotel : ObjectBase
    {
        public TourHotel()
        {
            TourHotelRoomTypes = new ObservableCollection<TourHotelRoomType>();
            HotelStartDay = DateTime.Today;
        }

        private int _tourHotelId;

        public int TourHotelId
        {
            get
            {
                return _tourHotelId;
            }
            set
            {
                if (_tourHotelId != value)
                {
                    _tourHotelId = value;
                    OnPropertyChanged(() => TourHotelId, true);
                }
            }
        }

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

        private int? _hotelId;

        public int? HotelId
        {
            get
            {
                return _hotelId;
            }
            set
            {
                if (_hotelId != value)
                {
                    _hotelId = value;
                    OnPropertyChanged(() => HotelId, true);
                }
            }
        }

        public ObservableCollection<TourHotelRoomType> TourHotelRoomTypes { get; set; }

        private DateTime _hotelStartDay;

        public DateTime HotelStartDay
        {
            get
            {
                return _hotelStartDay;
            }
            set
            {
                _hotelStartDay = value;
                OnPropertyChanged(() => HotelStartDay, true);
            }
        }

        public DateTime HotelEndDay { get; set; }   
    }
}
