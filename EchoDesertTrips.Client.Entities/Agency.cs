using Core.Common.Core;
using FluentValidation;
using System.Collections.Generic;

namespace EchoDesertTrips.Client.Entities
{
    public class Agency : ObjectBase
    {
        public Agency()
        {
            _agents = new List<Agent>();
        }

        private int _agencyId;

        public int AgencyId
        {
            get
            {
                return _agencyId;
            }

            set
            {
                if (_agencyId != value)
                {
                    _agencyId = value;
                    OnPropertyChanged(() => AgencyId, true);
                }
            }
        }

        private string _agencyName;

        public string AgencyName
        {
            get
            {
                return _agencyName;
            }

            set
            {
                if (_agencyName != value)
                {
                    _agencyName = value;
                    OnPropertyChanged(() => AgencyName, true);
                }
            }
        }

        private string _agencyAddress;

        public string AgencyAddress
        {
            get
            {
                return _agencyAddress;
            }

            set
            {
                if (_agencyAddress != value)
                {
                    //bool dirty = _agencyAddress != null;
                    _agencyAddress = value;
                    OnPropertyChanged(() => AgencyAddress, true);
                }
            }
        }

        private string _phone1;

        public string Phone1
        {
            get
            {
                return _phone1;
            }

            set
            {
                if (_phone1 != value)
                {
                    //bool dirty = _phone1 != null;
                    _phone1 = value;
                    OnPropertyChanged(() => Phone1, true);
                }
            }
        }

        private string _phone2;

        public string Phone2
        {
            get
            {
                return _phone2;
            }

            set
            {
                if (_phone1 != value)
                {
                    //bool dirty = _phone2 != null;
                    _phone2 = value;
                    OnPropertyChanged(() => Phone2, true);
                }
            }
        }

        private List<Agent> _agents;

        public List<Agent> Agents
        {
            get
            {
                return _agents;
            }

            set
            {
                _agents = value;
                OnPropertyChanged(() => Agents, false);
            }
        }

        class AgencyValidator : AbstractValidator<Agency>
        {
            public AgencyValidator()
            {
                RuleFor(obj => obj.AgencyName).NotEmpty();
                RuleFor(obj => obj.Phone1).NotEmpty();
            }
        }

        protected override IValidator GetValidator()
        {
            return new AgencyValidator();
        }
    }
}
