using AutoMapper;
using Core.Common.Core;
using FluentValidation;
using System;

namespace EchoDesertTrips.Client.Entities
{
    public class Customer : ObjectBase
    {
        public Customer()
        {
            DateOfBirdth = DateTime.Today;
            IssueData = DateTime.Today;
            ExpireyDate = DateTime.Today;
            bInEdit = false;

            _firstName = string.Empty;
            _lastName = string.Empty;
            _passportNumber = string.Empty;
        }

        private int _customerId;

        public int CustomerId
        {
            get
            {
                return _customerId;
            }

            set
            {
                if (_customerId != value)
                {
                    _customerId = value;
                    OnPropertyChanged(() => CustomerId, true);
                }
            }
        }

        private string _identityId;

        public string IdentityId
        {
            get
            {
                return _identityId;
            }

            set
            {
                if (_identityId != value)
                {
                    _identityId = value;
                    OnPropertyChanged(() => IdentityId, true);
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
                    _lastName = value;
                    OnPropertyChanged(() => LastName, true);
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
                    _phone2 = value;
                    OnPropertyChanged(() => Phone2, true);
                }
            }
        }

        private DateTime _dateOfBirdth;

        public DateTime DateOfBirdth
        {
            get
            {
                return _dateOfBirdth;
            }

            set
            {
                if (_dateOfBirdth != value)
                {
                    _dateOfBirdth = value;
                    OnPropertyChanged(() => DateOfBirdth, true);
                }
            }
        }

        private string _passportNumber;

        public string PassportNumber
        {
            get
            {
                return _passportNumber;
            }

            set
            {
                if (_passportNumber != value)
                {
                    _passportNumber = value;
                    OnPropertyChanged(() => PassportNumber, true);
                }
            }
        }

        private DateTime _issueData;

        public DateTime IssueData
        {
            get
            {
                return _issueData;
            }

            set
            {
                if (_issueData != value)
                {
                    _issueData = value;
                    OnPropertyChanged(() => IssueData, true);
                }
            }
        }

        private DateTime _expireyDate;

        public DateTime ExpireyDate
        {
            get
            {
                return _expireyDate;
            }

            set
            {
                if (_expireyDate != value)
                {
                    _expireyDate = value;
                    OnPropertyChanged(() => ExpireyDate, true);
                }
            }
        }

        private string _nationality;

        public string Nationality
        {
            get
            {
                return _nationality;
            }

            set
            {
                if (_nationality != value)
                {
                    _nationality = value;
                    OnPropertyChanged(() => Nationality, true);
                }
            }
        }

        private bool _hasVisa;

        public bool HasVisa
        {
            get
            {
                return _hasVisa;
            }

            set
            {
                if (_hasVisa != value)
                {
                    _hasVisa = value;
                    OnPropertyChanged(() => HasVisa, true);
                }
            }
        }

        public bool bInEdit { get; set; }

        class CustomerValidator : AbstractValidator<Customer>
        {
            public CustomerValidator()
            {
                RuleFor(obj => obj.FirstName).NotEmpty().MaximumLength(50);
                RuleFor(obj => obj.LastName).NotEmpty().MaximumLength(50);
                RuleFor(obj => obj.PassportNumber).NotEmpty().MaximumLength(30);
                RuleFor(obj => obj.IssueData).LessThanOrEqualTo(DateTime.Now).NotEmpty();
                RuleFor(obj => obj.ExpireyDate).GreaterThanOrEqualTo(o => o.IssueData).NotEmpty();
                RuleFor(obj => obj.DateOfBirdth).LessThanOrEqualTo(DateTime.Now);
                RuleFor(obj => obj.Phone1).NotEmpty();
            }
        }

        protected override IValidator GetValidator()
        {
            return new CustomerValidator();
        }
    }

    public class CustomerHelper
    {
        public static Customer CloneCustomer(Customer customer)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Customer, Customer>();
            });
            IMapper iMapper = config.CreateMapper();
            var _customer = iMapper.Map<Customer, Customer>(customer);
            return _customer;
        }
    }
}
