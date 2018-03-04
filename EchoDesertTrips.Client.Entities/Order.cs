using Core.Common.Core;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Client.Entities
{
    public class Order : ObjectBase
    {
        private int _orderId;
        private Operator _operator;
        private int _operatorId;
        private Customer _customer;
        private int _customerId;
        private Trip _trip;
        private int _tripId;
        private Agency _agency;
        private int _agencyId;
        private Agent _agent;
        private int _agentId;
        private decimal _advancePayment;

        public int OrderId
        {
            get
            {
                return _orderId;
            }

            set
            {
                if (_orderId != value)
                {
                    bool dirty = _orderId != 0;
                    _orderId = value;
                    OnPropertyChanged(() => OrderId, dirty);
                }
            }
        }

        public Operator Operator
        {
            get
            {
                return _operator;
            }

            set
            {
                if (_operator != value)
                {
                    bool dirty = _operator != null;
                    _operator = value;
                    OnPropertyChanged(() => Operator, dirty);
                }
            }
        }

        public int OperatorId
        {
            get
            {
                return _operatorId;
            }

            set
            {
                if (_operatorId != value)
                {
                    bool dirty = _operatorId != 0;
                    _operatorId = value;
                    OnPropertyChanged(() => OperatorId, dirty);
                }
            }
        }

        public Customer Customer
        {
            get
            {
                return _customer;
            }

            set
            {
                if (_customer != value)
                {
                    bool dirty = _customer != null;
                    _customer = value;
                    OnPropertyChanged(() => Customer, dirty);
                }
            }
        }

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
                    bool dirty = _customerId != 0;
                    _customerId = value;
                    OnPropertyChanged(() => CustomerId, dirty);
                }
            }
        }

        public Trip Trip
        {
            get
            {
                return _trip;
            }

            set
            {
                _trip = value;
                OnPropertyChanged(() => Trip, false);
            }
        }

        public int TripId
        {
            get
            {
                return _tripId;
            }

            set
            {
                if (_tripId != value)
                {
                    bool dirty = _tripId != 0;
                    _tripId = value;
                    OnPropertyChanged(() => TripId, dirty);
                }
            }
        }

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

        public int AgencyId
        {
            get
            {
                return _agencyId;
            }

            set
            {
                if (_agencyId != value)
                {
                    bool dirty = _agencyId != 0;
                    _agencyId = value;
                    OnPropertyChanged(() => AgencyId, dirty);
                }
            }
        }

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
                    bool dirty = _advancePayment != 0;
                    _advancePayment = value;
                    OnPropertyChanged(() => AdvancePayment, dirty);
                }
            }
        }

        public Order Clone()
        {
            return new Order()
            {
                OrderId = this.OrderId,
                CustomerId = this.CustomerId,
                TripId = this.TripId,
                AgentId = this.AgentId,
                AdvancePayment = this.AdvancePayment,
                OperatorId = this.OperatorId,
                Operator = new Operator(),
                Customer = this.Customer.Clone(),
                Trip = this.Trip.Clone(),
                Agent = this.Agent.Clone(),
                Agency = this.Agency.Clone(),
                AgencyId = this.AgencyId,
            };
        }

        class OrderValidator : AbstractValidator<Order>
        {
            public OrderValidator()
            {
                RuleFor(obj => obj.AdvancePayment).GreaterThanOrEqualTo(0);
            }
        }

        protected override IValidator GetValidator()
        {
            return new OrderValidator();
        }
    }
}
