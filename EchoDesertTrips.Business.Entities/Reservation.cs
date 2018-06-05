using System;
using Core.Common.Contracts;
using Core.Common.Core;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace EchoDesertTrips.Business.Entities
{
    [DataContract(IsReference =true)]
    public class Reservation : EntityBase, IIdentifiableEntity
    {
        [DataMember]
        public int ReservationId { get; set; }
        [DataMember]
        public Operator Operator { get; set; }
        [DataMember]
        public int? OperatorId { get; set; }
        [DataMember]
        public List<Customer> Customers { get; set; }
        [DataMember]
        public List<Tour> Tours { get; set; }
        [DataMember]
        public Agency Agency { get; set; }
        [DataMember]
        public int? AgencyId { get; set; }
        [DataMember]
        public Agent Agent { get; set; }
        [DataMember]
        public int? AgentId { get; set; }
        [DataMember]
        public double AdvancePayment { get; set; }
        [DataMember]
        public DateTime PickUpTime { get; set; }
        [DataMember]
        public string Comments { get; set; }
        [DataMember]
        public string Messages { get; set; }
        [DataMember]
        public Group Group { get; set; }
        [DataMember]
        public int GroupID { get; set; }
        [DataMember]
        public DateTime CreationTime { get; set; }
        [DataMember]
        public DateTime UpdateTime { get; set; }
        [DataMember]
        public int NumberOfCustomers { get; set; } 
        [DataMember]
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public int EntityId
        {
            get
            {
                return ReservationId;
            }

            set
            {
                ReservationId = value;
            }
        }
    }
}
