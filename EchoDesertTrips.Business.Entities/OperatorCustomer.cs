using Core.Common.Contracts;
using Core.Common.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Business.Entities
{
    [DataContract]
    public class OperatorCustomer : EntityBase, IIdentifiableEntity
    {
        [DataMember]
        public int OperatorCustomerId { get; set; }
        [DataMember]
        public int OperatorId { get; set; }
        [DataMember]
        public int CustomerId { get; set; }

        public int EntityId
        {
            get
            {
                return OperatorCustomerId;
            }

            set
            {
                OperatorCustomerId = value;
            }
        }
    }
}
