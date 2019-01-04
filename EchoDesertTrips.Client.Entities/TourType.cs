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
    public class TourType : ObjectBase
    {
        public TourType()
        {
            _tourTypeName = string.Empty;
            _destinations = string.Empty;
            _adultPrices = string.Empty;
            _childPrices = string.Empty;
            _infantPrices = string.Empty;

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

        private string _infantPrices;

        public string InfantPrices
        {
            get
            {
                return _infantPrices;
            }
            set
            {
                if (_infantPrices != value)
                {
                    _infantPrices = value;
                    OnPropertyChanged(() => InfantPrices);
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

        class TourTypeValidator : AbstractValidator<TourType>
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
        public static TourType CloneTourType(TourType tourType)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<TourType, TourType>()
                .ForMember(t => t.AdultPrices, o => o.NullSubstitute(string.Empty))
                .ForMember(t => t.ChildPrices, o => o.NullSubstitute(string.Empty))
                .ForMember(t => t.InfantPrices, o => o.NullSubstitute(string.Empty));
            });
            IMapper iMapper = config.CreateMapper();
            var _tourType = iMapper.Map<TourType, TourType>(tourType);
            return _tourType;
        }
    }
}
