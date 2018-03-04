using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Client.Entities
{
    class OperatorCustomer
    {
        private int _operatorCustomerId;
        private int _operatorId;
        private int _customerId;

        public int OperatorCustomerId
        {
            get
            {
                return _operatorCustomerId;
            }

            set
            {
                _operatorCustomerId = value;
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
                _operatorId = value;
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
                _customerId = value;
            }
        }
    }
}
