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
    public class TourWrapper : ObjectBase
    {
        public TourWrapper()
        {
            TourOptionals = new ObservableCollection<TourOptional>();
            TourHotelRoomTypes = new ObservableCollection<TourHotelRoomType>();
            //Hotel = new Hotel();
            TourType = new TourType();
            _startDate = DateTime.Today;
            _endDate = DateTime.Today;

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
                    //bool dirty = _tourId != 0;
                    _tourId = value;
                    OnPropertyChanged(() => TourId, true);
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
                    /*bool dirty = _startDate.Year != DateTime.Now.Year ||
                                  _startDate.Month != DateTime.Now.Month ||
                                  _startDate.Day != DateTime.Now.Day;*/ 
                    _startDate = value;
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
                    /*bool dirty = _endDate.Year != DateTime.Now.Year ||
                                  _endDate.Month != DateTime.Now.Month ||
                                  _endDate.Day != DateTime.Now.Day;*/ 
                    _endDate = value;
                    OnPropertyChanged(() => EndDate, true);
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

        //private int _hotelId;

        //public int HotelId
        //{
        //    get
        //    {
        //        return _hotelId;
        //    }

        //    set
        //    {
        //        if (_hotelId != value)
        //        {
        //            _hotelId = value;
        //            OnPropertyChanged(() => HotelId, true);
        //        }
        //    }
        //}

        private string _collectionAddress;

        public string CollectionAddress
        {
            get
            {
                return _collectionAddress;
            }

            set
            {
                if (CollectionAddress != value)
                {
                    _collectionAddress = value;
                    OnPropertyChanged(() => CollectionAddress, true);
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

        //private Hotel _hotel;

        //public Hotel Hotel
        //{
        //    get
        //    {
        //        return _hotel;
        //    }

        //    set
        //    {
        //        if (_hotel != value)
        //        {
        //            _hotel = value;
        //            OnPropertyChanged(() => Hotel, true);
        //        }
        //    }
        //}

        //private float _pricePerChild;

        //public float PricePerChild
        //{
        //    get
        //    {
        //        return _pricePerChild;
        //    }
        //    set
        //    {
        //        if (_pricePerChild != value)
        //        {
        //            bool dirty = PricePerChild != 0;
        //            _pricePerChild = value;
        //            OnPropertyChanged(() => PricePerChild, dirty);
        //        }
        //    }
        //}

        //private float _pricePerAdult;

        //public float PricePerAdult
        //{
        //    get
        //    {
        //        return _pricePerAdult;
        //    }
        //    set
        //    {
        //        if (_pricePerAdult != value)
        //        {
        //            bool dirty = PricePerAdult != 0;
        //            _pricePerAdult = value;
        //            OnPropertyChanged(() => PricePerAdult, dirty);
        //        }
        //    }
        //}

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

        private ObservableCollection<TourHotelRoomType> _tourHotelRoomTypes;

        public ObservableCollection<TourHotelRoomType> TourHotelRoomTypes
        {
            get
            {
                return _tourHotelRoomTypes;
            }

            set
            {
                _tourHotelRoomTypes = value;
                OnPropertyChanged(() => TourHotelRoomTypes, false);
            }
        }

        public TourWrapper CopyTour(Tour tour)
        {
            var tw = new TourWrapper()
            {
                TourId = tour.TourId,
                TourTypeId = tour.TourTypeId,
                TourType = tour.TourType,
                StartDate = tour.StartDate,
                EndDate = tour.EndDate,
                //Hotel = tour.Hotel,
                //HotelId = tour.HotelId,
                CollectionAddress = tour.CollectionAddress,
            };
            tour.TourOptionals.ForEach(tourOptional => tw.TourOptionals.Add(tourOptional));
            tour.TourHotelRoomTypes.ForEach(tourHotelRoomType => tw.TourHotelRoomTypes.Add(tourHotelRoomType));
            return tw;
        }

        public void UpdateTour(TourWrapper source, Tour target)
        {
            target.TourId = source.TourId;
            target.TourTypeId = source.TourTypeId;
            target.TourType = source.TourType;
            target.StartDate = source.StartDate;
            target.EndDate = source.EndDate;
            //target.HotelId = source.HotelId;
            //target.Hotel = source.Hotel;
            target.CollectionAddress = source.CollectionAddress;
            target.TourOptionals = new List<TourOptional>(source.TourOptionals);
            target.TourHotelRoomTypes = new List<TourHotelRoomType>(source.TourHotelRoomTypes);
            //target.TourOptionals = new List<TourOptional>();
            //if (source.TourOptionals != null)
            //    foreach (var tourOptional in source.TourOptionals)
            //        target.TourOptionals.Add(tourOptional);
        }

        class TourValidator : AbstractValidator<TourWrapper>
        {
            public TourValidator()
            {
                RuleFor(obj => obj.TourType.TourTypeName).NotEmpty();
                RuleFor(obj => obj.CollectionAddress).NotEmpty();
                RuleFor(obj => obj.StartDate).NotEqual(default(DateTime));
                RuleFor(obj => obj.EndDate).GreaterThanOrEqualTo(p => p.StartDate);
            }
        }

        protected override IValidator GetValidator()
        {
            return new TourValidator();
        }
    }
}
