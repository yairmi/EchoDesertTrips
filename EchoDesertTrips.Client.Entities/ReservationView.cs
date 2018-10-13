using Core.Common.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Client.Entities
{
    public class ReservationView : ObjectBase
    {
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

        private int _paxs;

        public int Paxs
        {
            get
            {
                return _paxs;
            }
            set
            {
                _paxs = value;
                OnPropertyChanged(() => Paxs);
            }
        }

        private Customer _customer;

        public Customer Customer
        {
            get
            {
                return _customer;
            }
            set
            {
                _customer = value;
                OnPropertyChanged(() => Customer);
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
                OnPropertyChanged(() => Tours);
            }
        }

        private DateTime _pickupTime;

        public DateTime PickUpTime
        {
            get { return _pickupTime; }
            set
            {
                if (_pickupTime != value)
                {
                    _pickupTime = value;
                    OnPropertyChanged(() => PickUpTime, true);
                }
            }
        }

        //private string _pickUpAddress;

        //public string PickUpAddress
        //{
        //    get
        //    {
        //        return _pickUpAddress;
        //    }
        //    set
        //    {
        //        _pickUpAddress = value;
        //        OnPropertyChanged(() => PickUpAddress);
        //    }
        //}


        private string _phone;

        public string Phone
        {
            get
            {
                return _phone;
            }
            set
            {
                _phone = value;
                OnPropertyChanged(() => Phone);
            }
        }

        private string _hotelName;

        public string HotelName
        {
            get
            {
                return _hotelName;
            }
            set
            {
                _hotelName = value;
                OnPropertyChanged(() => HotelName);
            }
        }

        private string _agencyAndAgentName;
        public string AgencyAndAgentName
        {
            get
            {
                return _agencyAndAgentName;
            }
            set
            {
                _agencyAndAgentName = value;
                OnPropertyChanged(() => AgencyAndAgentName);
            }
        }

        private double _totalPrice;
        public double TotalPrice
        {
            get
            {
                return _totalPrice;
            }
            set
            {
                _totalPrice = value;
                OnPropertyChanged(() => TotalPrice);
            }
        }
        private double _advancePayment;

        public double AdvancePayment
        {
            get
            {
                return _advancePayment;
            }

            set
            {
                if (Math.Abs(_advancePayment - value) > 0.0001)
                {
                    _advancePayment = value;
                    OnPropertyChanged(() => AdvancePayment, true);
                }
            }
        }

        private string _comments;

        public string Comments
        {
            get { return _comments; }
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
            get { return _messages; }
            set
            {
                if (_messages != value)
                {
                    _messages = value;
                    OnPropertyChanged(() => Messages, true);
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
                _groupID = value;
                OnPropertyChanged(() => GroupID);
            }
        }
    }
}
