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
        virtual public Operator Operator { get; set; }
        [DataMember]
        public int? OperatorId { get; set; }
        [DataMember]
        virtual public List<Customer> Customers { get; set; }
        [DataMember]
        virtual public List<Tour> Tours { get; set; }
        [DataMember]
        virtual public Agency Agency { get; set; }
        [DataMember]
        public int? AgencyId { get; set; }
        [DataMember]
        virtual public Agent Agent { get; set; }
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
        virtual public Group Group { get; set; }
        [DataMember]
        public int GroupID { get; set; }
        [DataMember]
        public DateTime CreationTime { get; set; }
        [DataMember]
        public DateTime UpdateTime { get; set; }
        [DataMember]
        public int Adults { get; set; }
        [DataMember]
        public int Childs { get; set; }
        [DataMember]
        public int Infants { get; set; }
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
