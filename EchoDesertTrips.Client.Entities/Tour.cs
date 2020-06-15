using AutoMapper;
using Core.Common.Core;
using FluentValidation;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace EchoDesertTrips.Client.Entities
{
    [Export]
    public class Tour : ObjectBase
    {
        private int _tourId;
        [DefaultValue(0)]
        public int TourId
        {
            get
            {
                return _tourId;
            }

            set
            {
                if (_tourId != value)
                {
                    _tourId = value;
                    OnPropertyChanged(() => TourId, true);
                }
            }
        }

        private TourType _tourType = new TourType();
        [DefaultValue(typeof(ObjectBase))]
        public TourType TourType
        {
            get
            {
                return _tourType;
            }

            set
            {
                if (_tourType != value)
                {
                    _tourType = value;
                    OnPropertyChanged(() => TourType, true);
                }
            }
        }

        private int _tourTypeId;
        [DefaultValue(0)]
        public int TourTypeId
        {
            get
            {
                return _tourTypeId;
            }

            set
            {
                if (_tourTypeId != value)
                {
                    _tourTypeId = value;
                    OnPropertyChanged(() => TourTypeId, true);
                }
            }
        }

        private DateTime _startDate = DateTime.Today;

        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }

            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    OnPropertyChanged(() => StartDate, true);
                }
            }
        }

        private DateTime _endDate = DateTime.Today;

        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }

            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    OnPropertyChanged(() => EndDate, true);
                }
            }
        }

        private string _pickupAddress = string.Empty;
        [DefaultValue("")]
        public string PickupAddress
        {
            get
            {
                return _pickupAddress;
            }

            set
            {
                if (_pickupAddress != value)
                {
                    _pickupAddress = value;
                    OnPropertyChanged(() => PickupAddress, true);
                }
            }
        }

        private ObservableCollection<TourOptional> _tourOptionals = new ObservableCollection<TourOptional>();
        [DefaultValue(typeof(ObjectBase))]
        public ObservableCollection<TourOptional> TourOptionals
        {
            get
            {
                return _tourOptionals; 
            }
            set
            {
                _tourOptionals = value;
            }
        }

        private ObservableCollection<TourHotel> _tourHotels = new ObservableCollection<TourHotel>();
        [DefaultValue(typeof(ObjectBase))]
        public ObservableCollection<TourHotel> TourHotels
        {
            get
            {
                return _tourHotels;
            }
            set
            {
                _tourHotels = value;
            }
        }

        private ObservableCollection<SubTour> _subTours = new ObservableCollection<SubTour>();
        [DefaultValue(typeof(ObjectBase))]
        public ObservableCollection<SubTour> SubTours
        {
            get
            {
                return _subTours;
            }
            set
            {
                _subTours = value;
            }
        }

        private string _tourTypePrice;
        [DefaultValue("")]
        public string TourTypePrice
        {
            get
            {
                return _tourTypePrice;
            }
            set
            {
                _tourTypePrice = value;
            }
        }

        private bool _private;
        [DefaultValue(false)]
        public bool Private
        {
            get
            {
                return _private;
            }
            set
            {
                _private = value;
                OnPropertyChanged(() => Private);
            }
        }

        private bool _bInEdit;
        [DefaultValue(false)]
        public bool bInEdit
        {
            get { return _bInEdit; }
            set { _bInEdit = value; }
        }

        class TourValidator : AbstractValidator<Tour>
        {
            public TourValidator()
            {
                RuleFor(obj => obj.TourType.TourTypeName).NotEmpty();
                RuleFor(obj => obj.PickupAddress).NotEmpty().MaximumLength(100);
                RuleFor(obj => obj.StartDate).NotEqual(default(DateTime));
                RuleFor(obj => obj.EndDate).GreaterThanOrEqualTo(p => p.StartDate);
            }
        }

        protected override IValidator GetValidator()
        {
            return new TourValidator();
        }
    }

    public class TourHelper
    {
        public static Tour CloneTour(Tour tour)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Tour, Tour>();
                cfg.CreateMap<TourHotel, TourHotel>();
                cfg.CreateMap<TourOptional, TourOptional>();
                cfg.CreateMap<TourHotelRoomType, TourHotelRoomType>();
            });

            IMapper iMapper = config.CreateMapper();
            var _tour = iMapper.Map<Tour, Tour>(tour);
            return _tour;
        }
    }
}
