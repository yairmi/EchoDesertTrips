using System;
using Core.Common.Core;
using Core.Common.UI.PubSubEvent;
using EchoDesertTrips.Client.Entities;
using Microsoft.Practices.Prism.PubSubEvents;

namespace Core.Common.UI.Core
{
    public class OperatorSingle : ObjectBase
    {
        private static readonly OperatorSingle INSTANCE = new OperatorSingle()
        {
            _operator = new Operator()
        };
        private OperatorSingle() { }
        public static OperatorSingle Instance
        {
            get
            {
                return INSTANCE;
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
                OnPropertyChanged(() => Operator);
            }
        }

        public void UpdateOperator(Operator op)
        {
            if (Operator.OperatorId == op.OperatorId)
            {
                Operator.OperatorName = op.OperatorName;
                Operator.Password = op.Password;
                Operator.OperatorFullName = op.OperatorFullName;
            }
        }
    }
}
