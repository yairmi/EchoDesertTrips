using Core.Common.Core;
using FluentValidation;

namespace EchoDesertTrips.Client.Entities
{
    public class Destination : ObjectBase
    {
        public Destination()
        {
            _visible = true;
        }

        public int DestinationId { get; set; }

        private string _destinationName;

        public string DestinationName
        {
            get
            {
                return _destinationName;
            }
            set
            {
                if (_destinationName != value)
                {
                    _destinationName = value;
                    OnPropertyChanged(() => DestinationName, true);
                }
            }
        }

        private bool _visible;

        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                if (_visible != value)
                {
                    _visible = value;
                    OnPropertyChanged(() => Visible, true);
                }
            }
        }

        class TourDestinationValidator : AbstractValidator<Destination>
        {
            public TourDestinationValidator()
            {
                RuleFor(obj => obj.DestinationName).NotEmpty();
            }
        }

        protected override IValidator GetValidator()
        {
            return new TourDestinationValidator();
        }
    }
}
