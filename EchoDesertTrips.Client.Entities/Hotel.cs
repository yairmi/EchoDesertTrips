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

        public string HotelAddress { get; set; }

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
