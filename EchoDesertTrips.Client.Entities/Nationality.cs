using Core.Common.Core;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Client.Entities
{
    public class Nationality : ObjectBase
    {
        public Nationality()
        {
            NationalityName = string.Empty;
        }
        public int NationalityId { get; set; }

        private string _nationalityName;

        public string NationalityName
        {
            get
            {
                return _nationalityName;
            }

            set
            {
                if (_nationalityName != value)
                {
                    _nationalityName = value;
                    OnPropertyChanged(() => NationalityName, true);
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

        class NationalityValidator : AbstractValidator<Nationality>
        {
            public NationalityValidator()
            {
                RuleFor(obj => obj.NationalityName).NotEmpty();
            }
        }

        protected override IValidator GetValidator()
        {
            return new NationalityValidator();
        }
    }
}
