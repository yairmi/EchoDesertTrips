using Core.Common.Core;
using FluentValidation;

namespace EchoDesertTrips.Client.Entities
{
    public class Optional : ObjectBase
    {
        public Optional()
        {
            OptionalDescription = string.Empty;
        }
        public int OptionalId { get; set; }

        private string _optionalDescription;

        public string OptionalDescription
        {
            get
            {
                return _optionalDescription;
            }

            set
            {
                if (_optionalDescription != value)
                {
                    _optionalDescription = value;
                    OnPropertyChanged(() => OptionalDescription, true);
                }
            }
        }

        private float _pricePerPerson;

        public float PricePerPerson
        {
            get
            {
                return _pricePerPerson;
            }
            set
            {
                if (_pricePerPerson != value)
                {
                    _pricePerPerson = value;
                    OnPropertyChanged(() => PricePerPerson, true);
                }
            }
        }

        private float _priceInclusive;

        public float PriceInclusive
        {
            get
            {
                return _priceInclusive;
            }
            set
            {
                if (_priceInclusive != value)
                {
                    _priceInclusive = value;
                    OnPropertyChanged(() => PriceInclusive, true);
                }
            }
        }

        public override int EntityId
        {
            get
            {
                return OptionalId;
            }

            set
            {
                OptionalId = value;
            }
        }

        class OptionalValidator : AbstractValidator<Optional>
        {
            public OptionalValidator()
            {
                RuleFor(obj => obj.OptionalDescription).NotEmpty().MaximumLength(50);
            }
        }

        protected override IValidator GetValidator()
        {
            return new OptionalValidator();
        }
    }
}
