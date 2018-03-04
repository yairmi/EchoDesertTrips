using Core.Common.Core;
using FluentValidation;

namespace EchoDesertTrips.Client.Entities
{
    public class TourDestinationWrapper : ObjectBase
    {
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
                    bool dirty = _tourDestinationId != 0;
                    _tourDestinationId = value;
                    OnPropertyChanged(() => TourDestinationId, dirty);
                }
            }
        }

        private string _tourDestinationName;

        public string TourDestinationName
        {
            get
            {
                return _tourDestinationName;
            }

            set
            {
                if (_tourDestinationName != value)
                {
                    bool dirty = _tourDestinationName != null;
                    _tourDestinationName = value;
                    OnPropertyChanged(() => TourDestinationName, dirty);
                }
            }
        }

        class TourDestinationValidator : AbstractValidator<TourDestinationWrapper>
        {
            public TourDestinationValidator()
            {
                RuleFor(obj => obj.TourDestinationName).NotEmpty();
            }
        }

        protected override IValidator GetValidator()
        {
            return new TourDestinationValidator();
        }
    }
}
