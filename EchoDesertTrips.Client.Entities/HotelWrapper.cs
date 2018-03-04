using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Core;
using FluentValidation;
using System.Collections.ObjectModel;

namespace EchoDesertTrips.Client.Entities
{
    public class HotelWrapper : ObjectBase
    {

        public HotelWrapper()
        {
            HotelRoomTypes = new ObservableCollection<HotelRoomType>();
        }

        private int _hotelId;

        public int HotelId
        {
            get
            {
                return _hotelId;
            }

            set
            {
                if (_hotelId != value)
                {
                    bool dirty = _hotelId != 0;
                    _hotelId = value;
                    OnPropertyChanged(() => HotelId, dirty);
                }
            }
        }

        private string _hotelName;

        public string HotelName
        {
            get
            {
                return _hotelName;
            }

            set
            {
                if (_hotelName != value)
                {
                    _hotelName = value;
                    OnPropertyChanged(() => HotelName, true);
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
                if (_hotelAddress != value)
                {
                    _hotelAddress = value;
                    OnPropertyChanged(() => HotelAddress, true);
                }
            }
        }

        private ObservableCollection<HotelRoomType> _hotelRoomTypes;

        public ObservableCollection<HotelRoomType> HotelRoomTypes
        {
            get
            {
                return _hotelRoomTypes;
            }

            set
            {
                _hotelRoomTypes = value;
                OnPropertyChanged(() => HotelRoomTypes, true);
            }
        }

        class NationalityValidator : AbstractValidator<HotelWrapper>
        {
            public NationalityValidator()
            {
                RuleFor(obj => obj.HotelName).NotEmpty();
            }
        }

        protected override IValidator GetValidator()
        {
            return new NationalityValidator();
        }
    }
}
