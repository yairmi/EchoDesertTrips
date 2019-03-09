using EchoDesertTrips.Client.Entities;
using System.Linq;

namespace Core.Common.UI.Core
{
    public class InventoriesSingle : ViewModelBase
    {
        private static readonly InventoriesSingle INSTANCE = new InventoriesSingle()
        {
            _hotels = new RangeObservableCollection<Hotel>(),
            _tourTypes = new RangeObservableCollection<TourType>(),
            _optionals = new RangeObservableCollection<Optional>(),
            _roomTypes = new RangeObservableCollection<RoomType>(),
            _agencies = new RangeObservableCollection<Agency>(),
            _operators = new RangeObservableCollection<Operator>()
        };
        private InventoriesSingle() { }
        public static InventoriesSingle Instance
        {
            get
            {
                return INSTANCE;
            }
        }

        private RangeObservableCollection<Hotel> _hotels;

        public RangeObservableCollection<Hotel> Hotels
        {
            get
            {
                return _hotels;
            }

            set
            {
                _hotels = value;
                OnPropertyChanged(() => Hotels, false);
            }
        }

        private RangeObservableCollection<TourType> _tourTypes;

        public RangeObservableCollection<TourType> TourTypes
        {
            get
            {
                return _tourTypes;
            }

            set
            {
                _tourTypes = value;
                OnPropertyChanged(() => TourTypes, false);
            }
        }

        private RangeObservableCollection<Optional> _optionals;

        public RangeObservableCollection<Optional> Optionals
        {
            get
            {
                return _optionals;
            }

            set
            {
                _optionals = value;
                OnPropertyChanged(() => Optionals, false);
            }
        }

        private RangeObservableCollection<RoomType> _roomTypes;

        public RangeObservableCollection<RoomType> RoomTypes
        {
            get
            {
                return _roomTypes;
            }
            set
            {
                _roomTypes = value;
                OnPropertyChanged(() => RoomTypes);
            }
        }

        private RangeObservableCollection<Agency> _agencies;

        public RangeObservableCollection<Agency> Agencies
        {
            get
            {
                return _agencies;
            }

            set
            {
                _agencies = value;
                OnPropertyChanged(() => Agencies, false);
            }
        }

        private RangeObservableCollection<Operator> _operators;

        public RangeObservableCollection<Operator> Operators
        {
            get
            {
                return _operators;
            }

            set
            {
                _operators = value;
                OnPropertyChanged(() => Operators, false);
            }
        }

        public void UpdateHotels(RoomType roomType)
        {
            foreach (var hotel in Hotels)
            {
                foreach (var hotelRoomType in hotel.HotelRoomTypes)
                {
                    if (hotelRoomType.RoomType.RoomTypeId == roomType.RoomTypeId)
                    {
                        hotelRoomType.RoomType.RoomTypeName = roomType.RoomTypeName;
                    }
                }
            }
        }

        public void Update(Hotel hotel)
        {
            var existingHotel = Inventories.Hotels.FirstOrDefault(item => item.HotelId == hotel.HotelId);
            if (existingHotel != null)
            {
                var index = Inventories.Hotels.IndexOf(existingHotel);
                Inventories.Hotels[index] = hotel;
            }
            else
            {
                Inventories.Hotels.Add(hotel);
            }
        }

        public void Update(RoomType roomType)
        {
            var existingRoomType = Inventories.RoomTypes.FirstOrDefault(item => item.RoomTypeId == roomType.RoomTypeId);
            if (existingRoomType != null)
            {
                var index = Inventories.RoomTypes.IndexOf(existingRoomType);
                Inventories.RoomTypes[index] = roomType;
            }
            else
            {
                Inventories.RoomTypes.Add(roomType);
            }
        }

        public void Update(Operator oper)
        {
            var existingOperator = Inventories.Operators.FirstOrDefault(item => item.OperatorId == oper.OperatorId);
            if (existingOperator != null)
            {
                var index = Inventories.Operators.IndexOf(existingOperator);
                Inventories.Operators[index] = oper;
            }
            else
            {
                Inventories.Operators.Add(oper);
            }
        }

        public void Update(Optional optional)
        {
            var existingOptional = Inventories.Optionals.FirstOrDefault(item => item.OptionalId == optional.OptionalId);
            if (existingOptional != null)
            {
                var index = Inventories.Optionals.IndexOf(existingOptional);
                Inventories.Optionals[index] = optional;
            }
            else
            {
                Inventories.Optionals.Add(optional);
            }
        }

        public void Update(TourType tourType)
        {
            var existingTourType = Inventories.TourTypes.FirstOrDefault(item => item.TourTypeId == tourType.TourTypeId);
            if (existingTourType != null)
            {
                var index = Inventories.TourTypes.IndexOf(existingTourType);
                Inventories.TourTypes[index] = tourType;
            }
            else
            {
                Inventories.TourTypes.Add(tourType);
            }
        }

        public void Update(Agency agency)
        {
            var existingAgency = Inventories.Agencies.FirstOrDefault(item => item.AgencyId == agency.AgencyId);
            if (existingAgency != null)
            {
                var index = Inventories.Agencies.IndexOf(existingAgency);
                Inventories.Agencies[index] = agency;
            }
            else
            {
                Inventories.Agencies.Add(agency);
            }
        }
    }
}
