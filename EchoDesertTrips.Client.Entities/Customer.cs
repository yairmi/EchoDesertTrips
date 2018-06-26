using Core.Common.Core;
using FluentValidation;
using System;

namespace EchoDesertTrips.Client.Entities
{
    /*public class Customer : NotificationObject
    {
        public Customer()
        {
            _firstName = string.Empty;
            _lastName = string.Empty;
            _passportNumber = string.Empty;
        }

        public int CustomerId { get; set; }
        public string IdentityId { get; set; }

        private string _firstName;

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                OnPropertyChanged(()=> FirstName);
            }
        }

        private string _lastName;

        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                OnPropertyChanged(()=> LastName);
            }
        }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        private DateTime _dateOfBirdth;

        public DateTime DateOfBirdth
        {
            get { return _dateOfBirdth; }
            set
            {
                _dateOfBirdth = value;
                OnPropertyChanged(()=> DateOfBirdth);
            }
        }

        private string _passportNumber;

        public string PassportNumber
        {
            get { return _passportNumber; }
            set
            {
                _passportNumber = value;
                OnPropertyChanged(()=> PassportNumber);
            }
        }

        private DateTime _issueDate;

        public DateTime IssueData
        {
            get { return _issueDate; }
            set
            {
                _issueDate = value;
                OnPropertyChanged(()=> IssueData);
            }
        }

        private DateTime _expireyDate;

        public DateTime ExpireyDate
        {
            get { return _expireyDate; }
            set
            {
                _expireyDate = value;
                OnPropertyChanged(()=> ExpireyDate);
            }
        }

        private Nationality _nationality;

        public Nationality Nationality
        {
            get { return _nationality; }
            set
            {
                _nationality = value;
                OnPropertyChanged(()=> Nationality);
            }
        }
        public int NationalityId { get; set; }

        private bool _hasVisa;

        public bool HasVisa
        {
            get { return _hasVisa; }
            set
            {
                _hasVisa = value;
                OnPropertyChanged(()=> HasVisa);
            }
        }
        public string LoginEmail { get; set; }

        //private string _fullName;

        public string FullName
        {
            get
            {
                return $"{LastName} {FirstName}";
                //return _fullName;
            }
            set
            {
                OnPropertyChanged(()=> FullName);
            }
        }
        //public List<Reservation> Reservations { get; set; }
    }*/

    public class Customer
    {
        public int CustomerId { get; set; }
        public string IdentityId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public DateTime DateOfBirdth { get; set; }
        public string PassportNumber { get; set; }
        public DateTime IssueData { get; set; }
        public DateTime ExpireyDate { get; set; }
        public string Nationality { get; set; }
        //public int NationalityId { get; set; }
        public bool HasVisa { get; set; }
    }

    public class CustomerWrapper : ObjectBase
    {
        public CustomerWrapper()
        {
            DateOfBirdth = DateTime.Today;
            IssueData = DateTime.Today;
            ExpireyDate = DateTime.Today;

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

        //private int _nationalityId;

        //public int NationalityId
        //{
        //    get
        //    {
        //        return _nationalityId;
        //    }

        //    set
        //    {
        //        if (_nationalityId != value)
        //        {
        //            _nationalityId = value;
        //            OnPropertyChanged(() => NationalityId, true);
        //        }
        //    }
        //}

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

        class CustomerValidator : AbstractValidator<CustomerWrapper>
        {
            public CustomerValidator()
            {
                RuleFor(obj => obj.FirstName).NotEmpty().MaximumLength(20);
                RuleFor(obj => obj.LastName).NotEmpty().MaximumLength(20);
                RuleFor(obj => obj.PassportNumber).NotEmpty().MaximumLength(50);
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
}
