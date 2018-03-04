using System;
using System.Collections.ObjectModel;
using Core.Common.Core;
using FluentValidation;

namespace EchoDesertTrips.Client.Entities
{
    public class CustomerWrapper : ObjectBase
    {
        public CustomerWrapper()
        {
            DateOfBirdth = default(DateTime);
            IssueData = default(DateTime);
            ExpireyDate = default(DateTime);
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
                    //var dirty = _customerId != 0;
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
                    //var dirty = _identityId != null;
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
                    //var dirty = _firstName != null;
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
                    //var dirty = _lastName != null;
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
                    //var dirty = _phone1 != null;
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
                    //var dirty = _phone2 != null;
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
                    //var dirty = _dateOfBirdth != default(DateTime);
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
                    //var dirty = _passportNumber != null;
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
                    //var dirty = _issueData != default(DateTime);
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
                    //var dirty = _expireyDate != default(DateTime);
                    _expireyDate = value;
                    OnPropertyChanged(() => ExpireyDate, true);
                }
            }
        }

        private Nationality _nationality;

        public Nationality Nationality
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

        private int _nationalityId;

        public int NationalityId
        {
            get
            {
                return _nationalityId;
            }

            set
            {
                if (_nationalityId != value)
                {
                    //var dirty = _nationalityId != 0;
                    _nationalityId = value;
                    OnPropertyChanged(() => NationalityId, true);
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

        private string _email;

        public string LoginEmail
        {
            get
            {
                return _email;
            }

            set
            {
                if (_email != value)
                {
                    //var dirty = _email != null;
                    _email = value;
                    OnPropertyChanged(() => LoginEmail, true);
                }
            }
        }

        private string _fullName;

        public string FullName
        {
            get
            {
                _fullName = $"{LastName} {FirstName}";
                return _fullName;
            }

            set
            {
                if (_fullName != value)
                {
                    _fullName = value;
                    OnPropertyChanged(() => FullName, true);
                }
            }
        }

        public CustomerWrapper CopyCustomer(Customer customer)
        {
            var customerWrapper = new CustomerWrapper()
            {
                CustomerId = customer.CustomerId,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                FullName = customer.FullName,
                DateOfBirdth = customer.DateOfBirdth,
                PassportNumber = customer.PassportNumber,
                IssueData = customer.IssueData,
                ExpireyDate = customer.ExpireyDate,
                NationalityId = customer.NationalityId,
                HasVisa = customer.HasVisa,
                LoginEmail = customer.LoginEmail,
                Phone1 = customer.Phone1,
                Phone2 = customer.Phone2,
                Nationality = customer.Nationality,
                IdentityId = customer.IdentityId,
            };
            return customerWrapper;
        }

        public void UpdateCustomer(CustomerWrapper source, Customer target)
        {
            target.CustomerId = source.CustomerId;
            target.FirstName = source.FirstName;
            target.LastName = source.LastName;
            target.FullName = source.FullName;
            target.DateOfBirdth = source.DateOfBirdth;
            target.PassportNumber = source.PassportNumber;
            target.IssueData = source.IssueData;
            target.ExpireyDate = source.ExpireyDate;
            target.NationalityId = source.NationalityId;
            target.Nationality = source.Nationality;
            target.HasVisa = source.HasVisa;
            target.LoginEmail = source.LoginEmail;
            target.Phone1 = source.Phone1;
            target.Phone2 = source.Phone2;
            target.IdentityId = source.IdentityId;
        }

        class CustomerWrapperValidator : AbstractValidator<CustomerWrapper>
        {
            public CustomerWrapperValidator()
            {
                RuleFor(obj => obj.NationalityId).NotEqual(0);
                RuleFor(obj => obj.FirstName).NotEmpty();
                RuleFor(obj => obj.LastName).NotEmpty();
                RuleFor(obj => obj.PassportNumber).NotEmpty();
                RuleFor(obj => obj.IssueData).LessThanOrEqualTo(DateTime.Now).NotEmpty();
                RuleFor(obj => obj.ExpireyDate).NotEmpty();
                RuleFor(obj => obj.DateOfBirdth).LessThanOrEqualTo(DateTime.Now);
                RuleFor(obj => obj.Phone1).NotEmpty();
            }
        }

        protected override IValidator GetValidator()
        {
            return new CustomerWrapperValidator();
        }
    }
}
