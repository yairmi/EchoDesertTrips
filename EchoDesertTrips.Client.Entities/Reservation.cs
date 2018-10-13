﻿using System;
using Core.Common.Core;
using FluentValidation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.Utils;
using AutoMapper;

namespace EchoDesertTrips.Client.Entities
{
    public class Reservation : ObjectBase
    {
        public Reservation()
        {
            _customers = new ObservableCollection<Customer>(); 
            _tours = new ObservableCollection<Tour>();
            _group = new Group();
            Agency = null;
            Agent = null;
            Operator = null;

            _pickupTime = DateTime.Today;
            _creationTime = DateTime.Today;
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
                    _operatorId = value;
                    OnPropertyChanged(() => OperatorId, true);
                }
            }
        }

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

        /*private int _tourId;

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
                    _tourId = value;
                    OnPropertyChanged(() => TourId, true);
                }
            }
        }*/

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
                if (_agency != null)
                    AgencyId = _agency.AgencyId;
                OnPropertyChanged(() => Agency, true);
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
                if (_agent != null)
                    AgentId = _agent.AgentId;
                OnPropertyChanged(() => Agent, true);
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
                if (Math.Abs(_advancePayment - value) > 0.0001)
                {
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

        private Group _group;

        public Group Group
        {
            get
            {
                return _group;
            }

            set
            {
                _group = value;
                OnPropertyChanged(() => Group);
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

        public DateTime UpdateTime { get; set; }

        //private int _numberOfCustomers;

        //public int NumberOfCustomers
        //{
        //    get { return _numberOfCustomers; }
        //    set
        //    {
        //        _numberOfCustomers = value;
        //        OnPropertyChanged(() => NumberOfCustomers, true);
        //    }
        //}
        private int _adults;
        public int Adults
        {
            get
            {
                return _adults;
            }
            set
            {
                if (_adults != value)
                {
                    _adults = value;
                    OnPropertyChanged(() => Adults, true);
                }
            }
        }
        private int _childs;
        public int Childs
        {
            get
            {
                return _childs;
            }
            set
            {
                if (_childs != value)
                {
                    _childs = value;
                    OnPropertyChanged(() => Childs, true);
                }
            }
        }
        private int _infants;
        public int Infants
        {
            get
            {
                return _infants;
            }
            set
            {
                if (_infants != value)
                {
                    _infants = value;
                    OnPropertyChanged(() => Infants, true);
                }
            }
        }



        public byte[] RowVersion { get; set; }

        public bool Lock { get; set; }

        public DateTime LockTime { get; set; }

        public int LockedById { get; set; }

        private int _actualNumberOfCustomers;

        public int ActualNumberOfCustomers
        {
            get
            {
                if (_actualNumberOfCustomers == 0)
                    return Customers.Count;
                return _actualNumberOfCustomers;
            }
            set
            {
                _actualNumberOfCustomers = value;
                OnPropertyChanged(() => ActualNumberOfCustomers);
            }
        }

        public bool RowVersionConflict { get; set; }

        class ReservationValidator : AbstractValidator<Reservation>
        {
            public ReservationValidator()
            {
                //RuleFor(obj => obj.NumberOfCustomers > 0);
                RuleFor(obj => obj.ReservationId > 0);
                RuleFor(obj => obj.AdvancePayment >= 0);
                RuleFor(obj => obj.Comments).MaximumLength(100);
                RuleFor(obj => obj.Messages).MaximumLength(100);
            }
        }

        protected override IValidator GetValidator()
        {
            return new ReservationValidator();
        }
    }

    /*public class ReservationMapper
    {
        public static ReservationWrapper CreateReservationWrapper(Reservation reservation)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Reservation, ReservationWrapper>();
                cfg.CreateMap<Tour, TourWrapper>();
                cfg.CreateMap<TourOptional, TourOptionalWrapper>();
                cfg.CreateMap<SubTour, SubTourWrapper>();
                cfg.CreateMap<TourType, TourTypeWrapper>()
                .ForMember(t => t.AdultPrices, o => o.NullSubstitute(string.Empty))
                .ForMember(t => t.ChildPrices, o => o.NullSubstitute(string.Empty));
                cfg.CreateMap<Customer, CustomerWrapper>();
            });

            var iMapper = config.CreateMapper();
            var reservationWrapper = iMapper.Map<Reservation, ReservationWrapper>(reservation);
            return reservationWrapper;
        }

        public static Reservation CreateReservation(ReservationWrapper reservationWrapper)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<ReservationWrapper, Reservation>();
                cfg.CreateMap<TourWrapper, Tour>();
                cfg.CreateMap<TourOptionalWrapper, TourOptional>();
                cfg.CreateMap<SubTour, SubTourWrapper>();
                cfg.CreateMap<TourTypeWrapper, TourType>();
                cfg.CreateMap<CustomerWrapper, Customer>();
            });

            IMapper iMapper = config.CreateMapper();
            var reservation = iMapper.Map<ReservationWrapper, Reservation>(reservationWrapper);
            return reservation;
        }

        public static ReservationWrapper CloneReservationWrapper(ReservationWrapper reservationWrapper)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<ReservationWrapper, ReservationWrapper>();
                cfg.CreateMap<Tour, TourWrapper>();
                cfg.CreateMap<TourOptional, TourOptionalWrapper>();
                cfg.CreateMap<SubTour, SubTourWrapper>();
                cfg.CreateMap<TourType, TourTypeWrapper>()
                .ForMember(t => t.AdultPrices, o => o.NullSubstitute(string.Empty))
                .ForMember(t => t.ChildPrices, o => o.NullSubstitute(string.Empty));
                cfg.CreateMap<Customer, CustomerWrapper>();
            });

            var iMapper = config.CreateMapper();
            var rw = iMapper.Map<ReservationWrapper, ReservationWrapper>(reservationWrapper);
            return rw;
        }
    }*/
}
