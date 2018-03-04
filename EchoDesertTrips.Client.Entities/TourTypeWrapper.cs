using Core.Common.Core;
using FluentValidation;

namespace EchoDesertTrips.Client.Entities
{
    public class TourTypeWrapper : ObjectBase
    {
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
                    bool dirty = _tourTypeId != 0;
                    _tourTypeId = value;
                    OnPropertyChanged(() => TourTypeId, dirty);
                }
            }
        }

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
                    bool dirty = _tourTypeName != null;
                    _tourTypeName = value;
                    OnPropertyChanged(() => TourTypeName, dirty);
                }
            }
        }

        private float _pricePerChild;

        public float PricePerChild
        {
            get
            {
                return _pricePerChild;
            }
            set
            {
                if (_pricePerChild != value)
                {
                    bool dirty = _pricePerChild != 0;
                    _pricePerChild = value;
                    OnPropertyChanged(() => PricePerChild, dirty);
                }
            }
        }

        private float _pricePerAdult;

        public float PricePerAdult
        {
            get
            {
                return _pricePerAdult;
            }
            set
            {
                if (_pricePerAdult != value)
                {
                    bool dirty = _pricePerAdult != 0;
                    _pricePerAdult = value;
                    OnPropertyChanged(() => PricePerAdult, dirty);
                }
            }
        }

        private TourDestination _tourDestination;

        public TourDestination TourDestination
        {
            get
            {
                return _tourDestination;
            }
            set
            {
                if (_tourDestination != value)
                {
                    _tourDestination = value;
                    OnPropertyChanged(() => TourDestination);
                }
            }
        }

        private int _tourDestinationId;

        public int TourDestinationId
        {
            get
            {
                return _tourDestinationId;
            }

            set
            {
                if (_tourDestinationId != value)
                {
                    var dirty = _tourTypeId != 0;
                    _tourDestinationId = value;
                    OnPropertyChanged(() => TourDestinationId, dirty);
                }
            }
        }

        private string _tourDescription;

        public string TourDescription
        {
            get
            {
                return _tourDescription;
            }

            set
            {
                if (_tourDescription != value)
                {
                    bool dirty = _tourDescription != null;
                    _tourDescription = value;
                    OnPropertyChanged(() => TourDescription, dirty);
                }
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
                if (_private != value)
                {
                    _private = value;
                    OnPropertyChanged(() => Private);
                }
            }
        }

        class TourTypeValidator : AbstractValidator<TourTypeWrapper>
        {
            public TourTypeValidator()
            {
                RuleFor(obj => obj.TourTypeName).NotEmpty();
            }
        }

        protected override IValidator GetValidator()
        {
            return new TourTypeValidator();
        }
    }
}
