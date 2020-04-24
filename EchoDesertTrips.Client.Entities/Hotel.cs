using Core.Common.Contracts;
using Core.Common.Core;
using FluentValidation;
using System.Collections.Generic;

namespace EchoDesertTrips.Client.Entities
{
    public class Hotel : ObjectBase
    {
        public Hotel()
        {
            _hotelName = string.Empty;
            _hotelRoomTypes = new List<HotelRoomType>();
        }
        public int HotelId { get; set; }

        private string _hotelName;

        public string HotelName
        {
            get { return _hotelName; }
            set
            {
                if (value != null)
                {
                    _hotelName = value;
                    OnPropertyChanged(()=> HotelName);
                }
            }
        }

        private string _hotelAddress;

        public string HotelAddress
        {
            get
            {
                return _hotelAddress;
            }
            set
            {
                _hotelAddress = value;
                OnPropertyChanged(() => HotelAddress);
            }
        }

        private List<HotelRoomType> _hotelRoomTypes;

        public List<HotelRoomType> HotelRoomTypes
        {
            get
            {
                return _hotelRoomTypes;
            }
            set
            {
                _hotelRoomTypes = value;
                OnPropertyChanged(() => HotelRoomTypes);
            }
        }

        public override int EntityId
        {
            get
            {
                return HotelId;
            }

            set
            {
                HotelId = value;
            }
        }

        class HotelValidator : AbstractValidator<Hotel>
        {
            public HotelValidator()
            {
                RuleFor(obj => obj.HotelName).NotEmpty();
                RuleFor(obj => obj.HotelRoomTypes.Capacity).NotEqual(0);
            }
        }

        protected override IValidator GetValidator()
        {
            return new HotelValidator();
        }
    }
}
