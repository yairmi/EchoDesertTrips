using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Core;
using FluentValidation;

namespace EchoDesertTrips.Client.Entities
{
    public class NationalityWrapper : ObjectBase
    {
        private int _nationalityId;
        private string _nationalityName;

        public int NationalityId
        {
            get
            {
                return _nationalityId;
            }

            set
            {
                _nationalityId = value;
                OnPropertyChanged(() => NationalityId, false);
            }
        }

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
                    bool dirty = _nationalityName != null;
                    _nationalityName = value;
                    OnPropertyChanged(() => NationalityName, dirty);
                }
            }
        }

        public NationalityWrapper CopyNationality(Nationality nationality)
        {
            return new NationalityWrapper()
            {
                NationalityId = nationality.NationalityId,
                NationalityName = nationality.NationalityName
            };
        }

        public void UpdateNationality(NationalityWrapper source, Nationality target)
        {
            target.NationalityId = source.NationalityId;
            target.NationalityName = source.NationalityName;
        }

        class NationalityValidator : AbstractValidator<NationalityWrapper>
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
