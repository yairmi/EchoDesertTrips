using Core.Common.Core;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        //private bool _visible;

        //public bool Visible
        //{
        //    get
        //    {
        //        return _visible;
        //    }
        //    set
        //    {
        //        if (_visible != value)
        //        {
        //            _visible = value;
        //            OnPropertyChanged(() => Visible, true);
        //        }
        //    }
        //}

        class OptionalValidator : AbstractValidator<Optional>
        {
            public OptionalValidator()
            {
                RuleFor(obj => obj.OptionalDescription).NotEmpty();
            }
        }

        protected override IValidator GetValidator()
        {
            return new OptionalValidator();
        }
    }
}
