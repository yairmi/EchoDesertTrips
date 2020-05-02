using Core.Common.Core;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Client.Entities
{
    public class SubTour : ObjectBase
    {
        public int SubTourId { get; set; }

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
                    OnPropertyChanged(() => DestinationName);
                }
            }
        }

        class SubTourValidator : AbstractValidator<SubTour>
        {
            public SubTourValidator()
            {
                RuleFor(obj => obj.DestinationName).NotEmpty().MaximumLength(100);
            }
        }

        protected override IValidator GetValidator()
        {
            return new SubTourValidator();
        }
    }
}
