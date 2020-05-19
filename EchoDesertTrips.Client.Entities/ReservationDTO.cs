using Core.Common.Core;
using System;
using System.Collections.Generic;

namespace EchoDesertTrips.Client.Entities
{
    public class ReservationDTO : ObjectBase
    {
        //public ReservationDTO
        private int _reservationId;
        public int ReservationId
        {
            get
            {
                return _reservationId;
            }
            set
            {
                if (_reservationId != value)
                {
                    _reservationId = value;
                    OnPropertyChanged(() => ReservationId, true);
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

        //private string _pickUpAddress;
        //public string PickupAddress
        //{
        //    get
        //    {
        //        return _pickUpAddress;
        //    }
        //    set
        //    {
        //        if (_pickUpAddress != value)
        //        {
        //            _pickUpAddress = value;
        //            OnPropertyChanged(() => PickupAddress, true);
        //        }
        //    }
        //}

        private string _hotelName;
        public string HotelName
        {
            get
            {
                return _hotelName;
            }
            set
            {
                if (_hotelName != value)
                {
                    _hotelName = value;
                    OnPropertyChanged(() => HotelName, true);
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

        private decimal _advancePayment;
        public decimal AdvancePayment
        {
            get
            {
                return _advancePayment;
            }
            set
            {
                if (_advancePayment != value)
                {
                    _advancePayment = value;
                    OnPropertyChanged(() => AdvancePayment, true);
                }
            }
        }

        private decimal _totalPrice;
        public decimal TotalPrice
        {
            get
            {
                return _totalPrice;
            }
            set
            {
                if (_totalPrice != value)
                {
                    _totalPrice = value;
                    OnPropertyChanged(() => TotalPrice, true);
                }
            }
        }

        private DateTime _pickUpTime;
        public DateTime PickUpTime
        {
            get
            {
                return _pickUpTime;
            }
            set
            {
                if (_pickUpTime != value)
                {
                    _pickUpTime = value;
                    OnPropertyChanged(() => PickUpTime, true);
                }
            }
        }

        //private DateTime _rservationStartDate;
        //public DateTime ReservationStartDate
        //{
        //    get
        //    {
        //        return _rservationStartDate;
        //    }
        //    set
        //    {
        //        if (_rservationStartDate != value)
        //        {
        //            _rservationStartDate = value;
        //            OnPropertyChanged(() => ReservationStartDate, true);
        //        }
        //    }
        //}

        private string _comments;
        public string Comments
        {
            get
            {
                return _comments;
            }
            set
            {
                if (_comments != value)
                {
                    _comments = value;
                    OnPropertyChanged(() => Comments, true);
                }
            }
        }

        private string _messages;
        public string Messages
        {
            get
            {
                return _messages;
            }
            set
            {
                if (_messages != value)
                {
                    _messages = value;
                    OnPropertyChanged(() => Messages, true);
                }
            }
        }

        private Group _group;
        public Group Group
        {
            get
            {
                return _group;
            }
            set
            {
                if (_group == null || _group.GroupId != value.GroupId)
                {
                    _group = value;
                    OnPropertyChanged(() => Group, true);
                }
            }
        }

        private int _groupID;
        public int GroupID
        {
            get
            {
                return _groupID;
            }
            set
            {
                if (_groupID != value)
                {
                    _groupID = value;
                    OnPropertyChanged(() => GroupID, true);
                }
            }
        }

        private int _actualNumberOfCustomers;
        public int ActualNumberOfCustomers
        {
            get
            {
                return _actualNumberOfCustomers;
            }
            set
            {
                if (_actualNumberOfCustomers != value)
                {
                    _actualNumberOfCustomers = value;
                    OnPropertyChanged(() => ActualNumberOfCustomers, true);
                }
            }
        }

        private string _firstTourTypeName;
        public string FirstTourTypeName
        {
            get
            {
                return _firstTourTypeName;
            }
            set
            {
                if (_firstTourTypeName != value)
                {
                    _firstTourTypeName = value;
                    OnPropertyChanged(() => FirstTourTypeName, true);
                }
            }
        }

        private bool _private;
        public bool Private
        {
            get
            {
                return _private;
            }
            set
            {
                if (_private != value)
                {
                    _private = value;
                    OnPropertyChanged(() => Private, true);
                }
            }
        }

        private string _car;
        public string Car
        {
            get
            {
                return _car;
            }
            set
            {
                if (_car != value)
                {
                    _car = value;
                    OnPropertyChanged(() => Car, true);
                }
            }
        }

        private string _guide;
        public string Guide
        {
            get
            {
                return _guide;
            }
            set
            {
                if (_guide != value)
                {
                    _guide = value;
                    OnPropertyChanged(() => Guide, true);
                }
            }
        }

        private string _endIn;
        public string EndIn
        {
            get
            {
                return _endIn;
            }
            set
            {
                if (_endIn != value)
                {
                    _endIn = value;
                    OnPropertyChanged(() => EndIn, true);
                }
            }
        }

        private List<Tour> _tours;
        public List<Tour> Tours
        {
            get
            {
                return _tours;
            }
            set
            {
                _tours = value;
                OnPropertyChanged(() => Tours, false);
            }
        }
    }
}
