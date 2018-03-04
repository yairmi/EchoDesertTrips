using Core.Common.Core;

namespace EchoDesertTrips.Client.Entities
{
    public class TourHotelRoomType : ObjectBase
    {
        public int TourHotelRoomTypeId { get; set; }

        public int HotelRoomTypeId { get; set; }

        public HotelRoomType HotelRoomType { get; set; }

        //public int TourId { get; set; }

        private int _capacity;

        public int Capacity
        {
            get
            {
                return _capacity;
            }
            set
            {
                if (_capacity != value)
                {
                    _capacity = value;
                    OnPropertyChanged(() => Capacity, true);
                }
            }
        }

        private int _persons;

        public int Persons
        {
            get
            {
                return _persons;
            }

            set
            {
                _persons = value;
                OnPropertyChanged(() => Persons, true);
            }
        }
    }
}
