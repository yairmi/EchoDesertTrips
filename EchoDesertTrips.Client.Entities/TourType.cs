using AutoMapper;
using Core.Common.Core;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Client.Entities
{
    public class TourType : ObjectBase
    {
        public int TourTypeId { get; set; }

        private string _tourTypeName;
        [DefaultValue("")]
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
        [DefaultValue("")]
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
        [DefaultValue("")]
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
        [DefaultValue("")]
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
        [DefaultValue("")]
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

        public ObservableCollection<TourTypeDescription> _tourTypeDescriptions = new ObservableCollection<TourTypeDescription>();

        public ObservableCollection<TourTypeDescription> TourTypeDescriptions
        {
            get
            {
                return _tourTypeDescriptions;
            }
            set
            {
                _tourTypeDescriptions = value;
            }
        }

        private bool _incramentExternalId;
        [DefaultValue(true)]
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

        public override int EntityId
        {
            get
            {
                return TourTypeId;
            }

            set
            {
                TourTypeId = value;
            }
        }

        class TourTypeValidator : AbstractValidator<TourType>
        {
            public TourTypeValidator()
            {
                RuleFor(obj => obj.TourTypeName).NotEmpty().MaximumLength(100);
                RuleFor(obj => obj.AdultPrices).MaximumLength(300);
                RuleFor(obj => obj.ChildPrices).MaximumLength(300);
                RuleFor(obj => obj.InfantPrices).MaximumLength(300);
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
