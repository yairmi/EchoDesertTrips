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
    public class Tour
    {
        public int TourId { get; set; }
        public TourType TourType { get; set; }
        public int TourTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PickupAddress { get; set; }
        public List<TourOptional> TourOptionals { get; set; }
        //public List<TourHotelRoomType> TourHotelRoomTypes { get; set; }
        public string Hotels { get; set; }
        public string RoomTypes { get; set; }
        public List<TourHotel> TourHotels { get; set; }
        public List<SubTour> SubTours { get; set; }
    }

    public class TourWrapper : ObjectBase
    {
        public TourWrapper()
        {
            //TourOptionals = new ObservableCollection<TourOptionalWrapper>();
            //TourHotels = new ObservableCollection<TourHotel>();
            //SubTours = new ObservableCollection<SubTourWrapper>();
            _tourOptionals = new ObservableCollection<TourOptionalWrapper>();
            _tourHotels = new ObservableCollection<TourHotel>();
            _subTours = new ObservableCollection<SubTourWrapper>();
            TourType = new TourTypeWrapper();
            _startDate = DateTime.Today;
            _endDate = DateTime.Today;

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

        private TourTypeWrapper _tourType;

        public TourTypeWrapper TourType
        {
            get
            {
                return _tourType;
            }

            set
            {
                if ((_tourType != null && (_tourType.TourTypeId != ((TourTypeWrapper)value).TourTypeId) || (_tourType == null)))
                {
                    _tourType = value;
                    EndDate = _tourType.Days == 0? EndDate : StartDate.AddDays(_tourType.Days - 1);
                    string[] destinations = SimpleSplitter.Split(_tourType.Destinations);
                    _subTours.Clear();
                    destinations.ToList().ForEach((destination) =>
                    {
                        SubTours.Add(new SubTourWrapper()
                        {
                            DestinationName = destination,
                        });
                    });

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

        private ObservableCollection<TourOptionalWrapper> _tourOptionals;

        public ObservableCollection<TourOptionalWrapper> TourOptionals
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

        //private ObservableCollection<TourHotelRoomType> _tourHotelRoomTypes;

        //public ObservableCollection<TourHotelRoomType> TourHotelRoomTypes
        //{
        //    get
        //    {
        //        return _tourHotelRoomTypes;
        //    }

        //    set
        //    {
        //        _tourHotelRoomTypes = value;
        //        OnPropertyChanged(() => TourHotelRoomTypes, false);
        //    }
        //}

        private string _hotels;
        public string Hotels
        {
            get
            {
                return _hotels;
            }
            set
            {
                _hotels = value;
                OnPropertyChanged(() => Hotels, true);
            }
        }

        private string _roomTypes;

        public string RoomTypes
        {
            get
            {
                return _roomTypes;
            }
            set
            {
                _roomTypes = value;
                OnPropertyChanged(() => RoomTypes, true);
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


        private ObservableCollection<SubTourWrapper> _subTours;

        public ObservableCollection<SubTourWrapper> SubTours
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

        class TourValidator : AbstractValidator<TourWrapper>
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
        public static TourWrapper CloneTourWrapper(TourWrapper tourWrapper)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<TourWrapper, TourWrapper>();
                cfg.CreateMap<TourHotel, TourHotel>();
                cfg.CreateMap<TourOptionalWrapper, TourOptionalWrapper>();
                cfg.CreateMap<TourHotelRoomType, TourHotelRoomType>();
            });

            IMapper iMapper = config.CreateMapper();
            var _tourWrapper = iMapper.Map<TourWrapper, TourWrapper>(tourWrapper);
            return _tourWrapper;
        }
    }
}
