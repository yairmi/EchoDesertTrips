using Core.Common.Core;
using FluentValidation;

namespace EchoDesertTrips.Client.Entities
{
    public class Agent : ObjectBase
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
                    _agentId = value;
                    OnPropertyChanged(() => AgentId, true);
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
                    //bool dirty = _firstName != null;
                    _firstName = value;
                    OnPropertyChanged(() => FirstName, true);
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
                    //bool dirty = _lastName != null;
                    _lastName = value;
                    OnPropertyChanged(() => LastName, true);
                }
            }
        }

        //public string FullName
        //{
        //    get
        //    {
        //        return _fullName = string.Format("{0} {1}", FirstName, LastName);
        //    }

        //    set
        //    {
        //        if (FullName != value)
        //        {
        //            _fullName = value;
        //            OnPropertyChanged(() => FullName, false);
        //        }
        //    }
        //}

        private string _phone1;

        public string Phone1
        {
            get
            {
                return _phone1;
            }

            set
            {
                if (Phone1 != value)
                {
                    //bool dirty = Phone1 != null;
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
                if (_phone2 != value)
                {
                   // bool dirty = _phone2 != null;
                    _phone2 = value;
                    OnPropertyChanged(() => Phone2, true);
                }
            }
        }

        class AgentValidator : AbstractValidator<Agent>
        {
            public AgentValidator()
            {
                RuleFor(obj => obj.FirstName).NotEmpty().MaximumLength(50);
                RuleFor(obj => obj.LastName).NotEmpty().MaximumLength(50);
                RuleFor(obj => obj.Phone1).NotEmpty().MaximumLength(30);
                RuleFor(obj => obj.Phone2).MaximumLength(30);
            }
        }

        protected override IValidator GetValidator()
        {
            return new AgentValidator();
        }
    }
}
