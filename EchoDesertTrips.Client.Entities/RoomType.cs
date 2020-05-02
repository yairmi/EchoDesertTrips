using Core.Common.Core;
using FluentValidation;

namespace EchoDesertTrips.Client.Entities
{
    public class RoomType : ObjectBase
    {
        public RoomType()
        {
            RoomTypeName = string.Empty;
        }

        public int RoomTypeId { get; set; }
        private string _roomTypeName;

        public string RoomTypeName
        {
            get
            {
                return _roomTypeName;
            }
            set
            {
                if (_roomTypeName != value)
                {
                    _roomTypeName = value;
                    OnPropertyChanged(() => RoomTypeName, true);
                }
            }
        }

        public override int EntityId
        {
            get
            {
                return RoomTypeId;
            }

            set
            {
                RoomTypeId = value;
            }
        }

        class RoomTypeValidator : AbstractValidator<RoomType>
        {
            public RoomTypeValidator()
            {
                RuleFor(obj => obj.RoomTypeName).NotEmpty().MaximumLength(50);
            }
        }

        protected override IValidator GetValidator()
        {
            return new RoomTypeValidator();
        }
    }

    public class TourRoomType : RoomType
    {
        private HotelRoomType _hotelRoomType;

        public HotelRoomType HotelRoomType
        {
            get
            {
                return _hotelRoomType;
            }
            set
            {
                _hotelRoomType = value;
                OnPropertyChanged(() => HotelRoomType, true);
            }
        }

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
