using System;
using Core.Common.Core;
using FluentValidation;

namespace EchoDesertTrips.Client.Entities
{
    public class AgentWrapper : ObjectBase
    {
        private int _agentId;

        public int AgentId
        {
            get
            {
                return _agentId;
            }

            set
            {
                if (_agentId != value)
                {
                    bool dirty = _agentId != 0;
                    _agentId = value;
                    OnPropertyChanged(() => AgentId, dirty);
                }
            }
        }

        private string _firstName;

        public string FirstName
        {
            get
            {
                return _firstName;
            }

            set
            {
                if (_firstName != value)
                {
                    bool dirty = _firstName != null;
                    _firstName = value;
                    OnPropertyChanged(() => FirstName, dirty);
                }
            }
        }

        private string _lastName;

        public string LastName
        {
            get
            {
                return _lastName;
            }

            set
            {
                if (_lastName != value)
                {
                    bool dirty = _lastName != null;
                    _lastName = value;
                    OnPropertyChanged(() => LastName, dirty);
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
                    bool dirty = _phone1 != null;
                    _phone1 = value;
                    OnPropertyChanged(() => Phone1, dirty);
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
                if (_phone2 != value)
                {
                    bool dirty = _phone2 != null;
                    _phone2 = value;
                    OnPropertyChanged(() => Phone2, dirty);
                }
            }
        }

        class AgentWrapperValidator : AbstractValidator<AgentWrapper>
        {
            public AgentWrapperValidator()
            {
                RuleFor(obj => obj.FirstName).NotEmpty();
                RuleFor(obj => obj.LastName).NotEmpty();
                RuleFor(obj => obj._phone1).NotEmpty();
            }
        }

        protected override IValidator GetValidator()
        {
            return new AgentWrapperValidator();
        }
    }
}
