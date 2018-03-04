using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Common.Core;
using FluentValidation;

namespace EchoDesertTrips.Client.Entities
{
    public class ReservationWrapper : ObjectBase
    {
        public ReservationWrapper()
        {
            //Remove CustomerWrapper
            _customers = new ObservableCollection<Customer>();;
            _tours = new ObservableCollection<Tour>();
            _customerChanged = false;
        }

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
                    //bool dirty = _reservationId != 0;
                    _reservationId = value;
                    OnPropertyChanged(() => ReservationId, true);
                }
            }
        }

        private Operator _operator;

        public Operator Operator
        {
            get
            {
                return _operator;
            }

            set
            {
                _operator = value;
                OnPropertyChanged(() => Operator, false);
            }
        }

        private int? _operatorId;

        public int? OperatorId
        {
            get
            {
                return _operatorId;
            }

            set
            {
                if (_operatorId != value)
                {
                    //bool dirty = _operatorId != null;
                    _operatorId = value;
                    OnPropertyChanged(() => OperatorId, true);
                }
            }
        }
        //Remove CustomerWrapper
        private ObservableCollection<Customer> _customers;

        public ObservableCollection<Customer> Customers
        {
            get
            {
                return _customers;
            }

            set
            {
                _customers = value;
                OnPropertyChanged(() => Customers, false);
            }
        }

        private ObservableCollection<Tour> _tours;

        public ObservableCollection<Tour> Tours
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

        private int _tourId;

        public int TourId
        {
            get
            {
                return _tourId;
            }

            set
            {
                if (_tourId != value)
                {
                    //bool dirty = _tourId != 0;
                    _tourId = value;
                    OnPropertyChanged(() => TourId, true);
                }
            }
        }

        private Agency _agency;
 
        public Agency Agency
        {
            get
            {
                return _agency;
            }

            set
            {
                _agency = value;
                OnPropertyChanged(() => Agency, false);
            }
        }

        private int? _agencyId;

        public int? AgencyId
        {
            get
            {
                return _agencyId;
            }

            set
            {
                if (_agencyId != value)
                {
                    //bool dirty = _agencyId != null;
                    _agencyId = value;
                    OnPropertyChanged(() => AgencyId, true);
                }
            }
        }

        private Agent _agent;

        public Agent Agent
        {
            get
            {
                return _agent;
            }

            set
            {
                _agent = value;
                OnPropertyChanged(() => Agent, false);
            }
        }

        private int? _agentId;

        public int? AgentId
        {
            get
            {
                return _agentId;
            }

            set
            {
                if (_agentId != value)
                {
                    //bool dirty = _agentId != null;
                    _agentId = value;
                    OnPropertyChanged(() => AgentId, true);
                }
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
                if (_advancePayment != value)
                {
                    //bool dirty = _advancePayment != 0;
                    _advancePayment = value;
                    OnPropertyChanged(() => AdvancePayment, true);
                }
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
                    //bool dirty = _pickupTime != default(DateTime);
                    _pickupTime = value;
                    OnPropertyChanged(() => PickUpTime, true);
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
                    //bool dirty = _comments != null;
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
                    //bool dirty = _messages != null;
                    _messages = value;
                    OnPropertyChanged(() => Messages, true);
                }
            }
        }

        private bool _customerChanged;

        public bool CustomersChanged
        {
            get { return _customerChanged; }
            set
            {
                _customerChanged = value;
                OnPropertyChanged(()=> CustomersChanged, true);
            }
        }

        private DateTime _creationTime;

        public DateTime CreationTime
        {
            get { return _creationTime; }
            set
            {
                _creationTime = value;
                OnPropertyChanged(() => CreationTime, false);
            }
        }

        public ReservationWrapper CopyReservation(Reservation reservation)
        {
            var reservationWrapper = new ReservationWrapper()
            {
                ReservationId = reservation.ReservationId,
                AgentId = reservation.AgentId,
                AdvancePayment = reservation.AdvancePayment,
                OperatorId = reservation.OperatorId,
                Operator = new Operator(),
                //Remove CustomerWrapper
                Customers = new ObservableCollection<Customer>(),
                Tours = new ObservableCollection<Tour>(),
                Agent = reservation.Agent,
                Agency = reservation.Agency,
                AgencyId = reservation.AgencyId,
                Comments = reservation.Comments,
                Messages = reservation.Messages,
                PickUpTime = reservation.PickUpTime,
                CreationTime = reservation.CreationTime
            };
            //Remove CustomerWrapper
            //var cw = new CustomerWrapper();
            //foreach (var customer in reservation.Customers)
            //{
            //    reservationWrapper.Customers.Add(cw.CopyCustomer(customer));
            //}
            foreach (var customer in reservation.Customers)
                reservationWrapper.Customers.Add((Customer)customer.Clone());
            foreach (var tour in reservation.Tours)
                reservationWrapper.Tours.Add((Tour)tour.Clone());
            //var tw = new TourWrapper();
            //foreach (var trip in reservation.Tours)
            //{
            //    reservationWrapper.Tours.Add(tw.CopyTour(trip));
            //}

            return reservationWrapper;

        }

        public void UpdateReservation(ReservationWrapper source, Reservation target)
        {
            target.ReservationId = source.ReservationId;
            target.AgentId = source.AgentId;
            target.AdvancePayment = source.AdvancePayment;
            target.OperatorId = source.OperatorId;
            target.Operator = source.Operator;
            //Remove CustomerWrapper
            //var cw = new CustomerWrapper();
            //foreach (var customer in source.Customers)
            //{
            //    var cus = new Customer();
            //    cw.UpdateCustomer(customer, cus);
            //    target.Customers.Add(cus);
            //}
            target.Customers = new List<Customer>(source.Customers);
            //var tw = new TourWrapper();
            //foreach (var tour in source.Tours)
            //{
            //    var tr = new Tour();
            //    tw.UpdateTour(tour, tr);
            //    tr.TourOptionals.RemoveAll(t => t.Selected == false);
            //    target.Tours.Add(tr);
            //}
            foreach(var tour in source.Tours)
            {
                var tr = new Tour();
                tr = (Tour)tour.Clone();
                var tourOptionals = new List<TourOptional>(tr.TourOptionals);
                tourOptionals.RemoveAll(t => t.Selected == false);
                tr.TourOptionals.Clear();
                tourOptionals.ForEach((tourOptional) =>
                {
                    tr.TourOptionals.Add(tourOptional);
                });

                target.Tours.Add(tr);
            }
            target.Agent = source.Agent;
            target.Agency = source.Agency;
            target.AgencyId = source.AgencyId;
            target.Comments = source.Comments;
            target.Messages = source.Messages;
            target.PickUpTime = source.PickUpTime;
            target.CreationTime = source.CreationTime;
        }

        class ReservationValidator : AbstractValidator<ReservationWrapper>
        {
            public ReservationValidator()
            {
                RuleFor(obj => obj.ReservationId > 0);
                RuleFor(obj => obj.AdvancePayment >= 0);
                RuleFor(obj => obj.Comments).MaximumLength(50);
                RuleFor(obj => obj.Messages).MaximumLength(25);
            }
        }

        protected override IValidator GetValidator()
        {
            return new ReservationValidator();
        }
    }
}
