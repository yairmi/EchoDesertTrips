using EchoDesertTrips.Client.Entities;

namespace Core.Common.UI.Core
{
    public class OperatorSingle : ViewModelBase
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
