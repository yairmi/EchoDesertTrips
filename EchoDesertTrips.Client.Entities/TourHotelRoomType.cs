using Core.Common.Core;
using FluentValidation;
using System.Windows;

namespace EchoDesertTrips.Client.Entities
{
    public class TourHotelRoomType : ObjectBase
    {
        public int TourHotelRoomTypeId { get; set; }

        public int HotelRoomTypeId { get; set; }

        public HotelRoomType HotelRoomType { get; set; }

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

        class TourHotelRoomTypeValidator : AbstractValidator<TourHotelRoomType>
        {
            public TourHotelRoomTypeValidator()
            {
                RuleFor(obj => obj.Capacity).GreaterThanOrEqualTo(0).WithMessage("Room capacity must be greater then 0");
                RuleFor(obj => obj.Persons).GreaterThanOrEqualTo(0).WithMessage("Number of pax must be greater then 0");
            }
        }

        protected override IValidator GetValidator()
        {
            return new TourHotelRoomTypeValidator();
        }
    }
}
