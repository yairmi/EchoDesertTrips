using AutoMapper;
using Core.Common.Core;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Client.Entities
{
    public class TourType
    {
        public int TourTypeId { get; set; }
        public string TourTypeName { get; set; }
        public string AdultPrices { get; set; }
        public string ChildPrices { get; set; }
        public string Destinations { get; set; }
        public bool Private { get; set; }
        //public string TourDescription { get; set; }
        public byte Days { get; set; }
        public List<TourTypeDescription> TourTypeDescriptions { get; set; }
        public bool IncramentExternalId { get; set; }
        public bool Visible { get; set; }
    }


    public class TourTypeWrapper : ObjectBase
    {
        public TourTypeWrapper()
        {
            _tourTypeName = string.Empty;
            _destinations = string.Empty;
            _adultPrices = string.Empty;
            _childPrices = string.Empty;

            _tourTypeDescriptions = new ObservableCollection<TourTypeDescription>();
            _incramentExternalId = true;
        }

        public int TourTypeId { get; set; }

        private string _tourTypeName;

        public string TourTypeName
        {
            get
            {
                return _tourTypeName;
            }

            set
            {
                if (_tourTypeName != value)
                {
                    _tourTypeName = value;
                    OnPropertyChanged(() => TourTypeName, true);
                }
            }
        }

        private string _destinations;

        public string Destinations
        {
            get
            {
                return _destinations;
            }
            set
            {
                if (_destinations != value)
                {
                    _destinations = value;
                    OnPropertyChanged(() => Destinations);
                }
            }
        }

        //private string _tourDescription;

        //public string TourDescription
        //{
        //    get
        //    {
        //        return _tourDescription;
        //    }

        //    set
        //    {
        //        if (_tourDescription != value)
        //        {
        //            _tourDescription = value;
        //            OnPropertyChanged(() => TourDescription, true);
        //        }
        //    }
        //}

        private string _adultPrices;

        public string AdultPrices
        {
            get
            {
                return _adultPrices;
            }
            set
            {
                if (_adultPrices != value)
                {
                    _adultPrices = value;
                    OnPropertyChanged(() => AdultPrices);
                }
            }
        }

        private string _childPrices;

        public string ChildPrices
        {
            get
            {
                return _childPrices;
            }
            set
            {
                if (_childPrices != value)
                {
                    _childPrices = value;
                    OnPropertyChanged(() => ChildPrices);
                }
            }
        }



        private bool _private;

        public bool Private {
            get
            {
                return _private;
            }
            set
            {
                if (_private != value)
                {
                    _private = value;
                    OnPropertyChanged(() => Private, true);
                }
            }
        }

        private byte _days;

        public byte Days
        {
            get
            {
                return _days;
            }
            set
            {
                _days = value;
                OnPropertyChanged(() => Days, true);
            }
        }

        private ObservableCollection<TourTypeDescription> _tourTypeDescriptions;

        public ObservableCollection<TourTypeDescription> TourTypeDescriptions
        {
            get
            {
                return _tourTypeDescriptions;
            }
            set
            {
                _tourTypeDescriptions = value;
                OnPropertyChanged(() => TourTypeDescriptions);
            }
        }

        private bool _incramentExternalId;

        public bool IncramentExternalId
        {
            get
            {
                return _incramentExternalId;
            }
            set
            {
                _incramentExternalId = value;
                OnPropertyChanged(() => IncramentExternalId, true);
            }
        }

        class TourTypeValidator : AbstractValidator<TourTypeWrapper>
        {
            public TourTypeValidator()
            {
                RuleFor(obj => obj.TourTypeName).NotEmpty();
                //RuleFor(obj => obj.TourDestinations.Count).NotEqual(0);
                //RuleFor(obj => obj.TourDestination.TourDestinationId).NotEqual(0);
            }
        }

        protected override IValidator GetValidator()
        {
            return new TourTypeValidator();
        }
    }

    public class TourTypeHelper
    {
        public static TourTypeWrapper CreateTourTypeWrapper(TourTypeWrapper tourTypeWrapper)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<TourTypeWrapper, TourTypeWrapper>()
                .ForMember(t => t.AdultPrices, o => o.NullSubstitute(string.Empty))
                .ForMember(t => t.ChildPrices, o => o.NullSubstitute(string.Empty));
            });
            IMapper iMapper = config.CreateMapper();
            var _tourTypeWrapper = iMapper.Map<TourTypeWrapper, TourTypeWrapper>(tourTypeWrapper);
            return _tourTypeWrapper;
        }

        public static TourTypeWrapper CreateTourTypeWrapper(TourType tourType)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<TourType, TourTypeWrapper>()
                .ForMember(t => t.AdultPrices, o => o.NullSubstitute(string.Empty))
                .ForMember(t => t.ChildPrices, o => o.NullSubstitute(string.Empty));
            });
            IMapper iMapper = config.CreateMapper();
            var _tourTypeWrapper = iMapper.Map<TourType, TourTypeWrapper>(tourType);
            return _tourTypeWrapper;
        }
    }
}
