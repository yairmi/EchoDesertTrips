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
    public class Order : EntityBase, IIdentifiableEntity
    {
        public int EntityId
        {
            get
            {
                return OrderId;
            }

            set
            {
                OrderId = value;
            }
        }

        [DataMember]
        public int OrderId { get; set; }
        [DataMember]
        public Operator Operator { get; set; }
        [DataMember]
        public int OperatorId { get; set; }
        [DataMember]
        public Customer Customer { get; set; }
        [DataMember]
        public List<Customer> Customers { get; set; }
        [DataMember]
        public Trip Trip { get; set; }
        [DataMember]
        public int TripId { get; set; }
        [DataMember]
        public Agency Agency { get; set; }
        [DataMember]
        public int AgencyId { get; set; }
        [DataMember]
        public Agent Agent { get; set; }
        [DataMember]
        public int AgentId { get; set; }
        [DataMember]
        public decimal AdvancePayment { get; set; }
        [DataMember]
        public uint OrderNumber { get; set; }
    }
}
