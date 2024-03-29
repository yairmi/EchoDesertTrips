﻿using Core.Common.Core;
using FluentValidation;

namespace EchoDesertTrips.Client.Entities
{
    public class Operator : ObjectBase
    {
        public Operator()
        {
            _operatorName = string.Empty;
            _password = string.Empty;
        }

        private int _operatorId;

        public int OperatorId
        {
            get
            {
                return _operatorId;
            }

            set
            {
                _operatorId = value;
            }
        }

        private string _operatorName;

        public string OperatorName
        {
            get
            {
                return _operatorName;
            }

            set
            {
                if (_operatorName != value)
                {
                    _operatorName = value;
                    OnPropertyChanged(() => OperatorName, true);
                }
            }
        }

        private string _password;

        public string Password
        {
            get
            {
                return _password;
            }

            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged(() => Password, true);
                }
            }
        }

        private string _operatorFullName;
        public string OperatorFullName
        {
            get
            {
                return _operatorFullName;
            }
            set
            {
                _operatorFullName = value;
                OnPropertyChanged(() => OperatorFullName, true);
            }
        }

        private bool _admin;

        public bool Admin
        {
            get
            {
                return _admin;
            }

            set
            {
                if (_admin != value)
                {
                    _admin = value;
                    OnPropertyChanged(() => Admin, true);
                }
            }
        }

        public override int EntityId
        {
            get
            {
                return OperatorId;
            }

            set
            {
                OperatorId = value;
            }
        }

        class OperatorValidator : AbstractValidator<Operator>
        {
            public OperatorValidator()
            {
                RuleFor(obj => obj.OperatorName).NotEmpty().MaximumLength(50);
                RuleFor(obj => obj.Password).NotEmpty().MaximumLength(50);
            }
        }

        protected override IValidator GetValidator()
        {
            return new OperatorValidator();
        }
    }
}
