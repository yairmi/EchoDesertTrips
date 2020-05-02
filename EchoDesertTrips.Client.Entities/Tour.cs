using AutoMapper;
using Core.Common.Core;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EchoDesertTrips.Client.Entities
{
    public class Tour : ObjectBase
    {
        public Tour()
        {
            TourOptionals = new ObservableCollection<TourOptional>();
            TourHotels = new ObservableCollection<TourHotel>();
            SubTours = new ObservableCollection<SubTour>();
            _tourType = new TourType();
            _startDate = DateTime.Today;
            _endDate = (_tourType == null ||_tourType.Days == 0) ? _endDate : StartDate.AddDays(_tourType.Days - 1);
            _pickupAddress = string.Empty;
            bInEdit = false;
        }

        private int _tourId;

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

        private TourType _tourType;

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

        private DateTime _startDate;

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
                    //EndDate = _tourType.Days == 0 ? EndDate : StartDate.AddDays(_tourType.Days - 1);
                    OnPropertyChanged(() => StartDate, true);
                }
            }
        }

        private DateTime _endDate;

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

        private string _pickupAddress;

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

        public ObservableCollection<TourOptional> TourOptionals { get; set; }

        public ObservableCollection<TourHotel> TourHotels { get; set; }

        public ObservableCollection<SubTour> SubTours { get; set; }

        private string _tourTypePrice;
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

        public bool bInEdit { get; set; }

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
