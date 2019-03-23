using Core.Common.Core;
using Core.Common.UI.PubSubEvent;
using EchoDesertTrips.Client.Entities;
using Microsoft.Practices.Prism.PubSubEvents;
using System.Linq;
using Core.Common.UI.CustomEventArgs;
using System;

namespace Core.Common.UI.Core
{
    public class InventoriesSingle : ObjectBase
    {
        private static readonly InventoriesSingle INSTANCE = new InventoriesSingle()
        {
            _hotels = new RangeObservableCollection<Hotel>(),
            _tourTypes = new RangeObservableCollection<TourType>(),
            _optionals = new RangeObservableCollection<Optional>(),
            _roomTypes = new RangeObservableCollection<RoomType>(),
            _agencies = new RangeObservableCollection<Agency>(),
            _operators = new RangeObservableCollection<Operator>(),
        };

        private InventoriesSingle() {}

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
            var existingHotel = Hotels.FirstOrDefault(item => item.HotelId == hotel.HotelId);
            if (existingHotel != null)
            {
                var index = Hotels.IndexOf(existingHotel);
                Hotels[index].HotelName = hotel.HotelName;
                Hotels[index].HotelAddress = hotel.HotelAddress;
                Hotels[index].HotelRoomTypes = hotel.HotelRoomTypes;
            }
            else
            {
                Hotels.Add(hotel);
            }
        }

        public void Update(RoomType roomType)
        {
            var existingRoomType = RoomTypes.FirstOrDefault(item => item.RoomTypeId == roomType.RoomTypeId);
            if (existingRoomType != null)
            {
                var index = RoomTypes.IndexOf(existingRoomType);
                RoomTypes[index].RoomTypeName = roomType.RoomTypeName;
            }
            else
            {
                RoomTypes.Add(roomType);
            }
        }

        public void Update(Operator oper)
        {
            var existingOperator = Operators.FirstOrDefault(item => item.OperatorId == oper.OperatorId);
            if (existingOperator != null)
            {
                var index = Operators.IndexOf(existingOperator);
                Operators[index].OperatorFullName = oper.OperatorFullName;
                Operators[index].OperatorName = oper.OperatorName;
                Operators[index].Password = oper.Password;
            }
            else
            {
                Operators.Add(oper);
            }
        }

        public void Update(Optional optional)
        {
            var existingOptional = Optionals.FirstOrDefault(item => item.OptionalId == optional.OptionalId);
            if (existingOptional != null)
            {
                var index = Optionals.IndexOf(existingOptional);
                Optionals[index].OptionalDescription = optional.OptionalDescription;
                Optionals[index].PriceInclusive = optional.PriceInclusive;
                Optionals[index].PricePerPerson = optional.PricePerPerson;
            }
            else
            {
                Optionals.Add(optional);
            }
        }

        public void Update(TourType tourType)
        {
            var existingTourType = TourTypes.FirstOrDefault(item => item.TourTypeId == tourType.TourTypeId);
            if (existingTourType != null)
            {
                var index = TourTypes.IndexOf(existingTourType);
                TourTypes[index].TourTypeName = tourType.TourTypeName;
                TourTypes[index].TourTypeDescriptions = tourType.TourTypeDescriptions;
                TourTypes[index].AdultPrices = tourType.AdultPrices;
                TourTypes[index].ChildPrices = tourType.ChildPrices;
                TourTypes[index].InfantPrices = tourType.InfantPrices;
                TourTypes[index].Days = tourType.Days;
                TourTypes[index].Destinations = tourType.Destinations;
                TourTypes[index].IncramentExternalId = tourType.IncramentExternalId;
                TourTypes[index].Private = tourType.Private;
            }
            else
            {
                TourTypes.Add(tourType);
            }
        }

        public void Update(Agency agency)
        {
            var existingAgency = Agencies.FirstOrDefault(item => item.AgencyId == agency.AgencyId);
            if (existingAgency != null)
            {
                var index = Agencies.IndexOf(existingAgency);
                Agencies[index].AgencyName = agency.AgencyName;
                Agencies[index].AgencyAddress = agency.AgencyAddress;
                Agencies[index].Phone1 = agency.Phone1;
                Agencies[index].Phone2 = agency.Phone2;
                Agencies[index].Agents = agency.Agents;
            }
            else
            {
                Agencies.Add(agency);
            }
        }
    }
}
