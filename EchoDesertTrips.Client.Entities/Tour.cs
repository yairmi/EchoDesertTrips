using AutoMapper;
using Core.Common.Core;
using Core.Common.Utils;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EchoDesertTrips.Client.Entities
{
    public class Tour : ObjectBase
    {
        public Tour()
        {
            _tourOptionals = new ObservableCollection<TourOptional>();
            _tourHotels = new ObservableCollection<TourHotel>();
            _subTours = new ObservableCollection<SubTour>();
            TourType = new TourType();
            _startDate = DateTime.Today;
            _endDate = DateTime.Today;
            bInEdit = false;

            _pickupAddress = string.Empty;
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
                if ((_tourType != null && (_tourType.TourTypeId != ((TourType)value).TourTypeId) || (_tourType == null)))
                {
                    _tourType = value;
                    EndDate = _tourType.Days == 0? EndDate : StartDate.AddDays(_tourType.Days - 1);
                    string[] destinations = SimpleSplitter.Split(_tourType.Destinations);
                    if (_subTours != null)
                    {
                        _subTours.Clear();
                        destinations.ToList().ForEach((destination) =>
                        {
                            SubTours.Add(new SubTour()
                            {
                                DestinationName = destination,
                            });
                        });
                    }

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
                    EndDate = _tourType.Days == 0 ? EndDate : StartDate.AddDays(_tourType.Days - 1);
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

        private ObservableCollection<TourOptional> _tourOptionals;

        public ObservableCollection<TourOptional> TourOptionals
        {
            get
            {
                return _tourOptionals;
            }

            set
            {
                _tourOptionals = value;
                OnPropertyChanged(() => TourOptionals, false);
            }
        }

        private ObservableCollection<TourHotel> _tourHotels;

        public ObservableCollection<TourHotel> TourHotels
        {
            get
            {
                return _tourHotels;
            }
            set
            {
                _tourHotels = value;
                OnPropertyChanged(() => TourHotels, true);
            }
        }


        private ObservableCollection<SubTour> _subTours;

        public ObservableCollection<SubTour> SubTours
        {
            get
            {
                return _subTours;
            }
            set
            {
                _subTours = value;
                OnPropertyChanged(() => SubTours);

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
                RuleFor(obj => obj.PickupAddress).NotEmpty();
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
