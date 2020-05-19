using Core.Common.Contracts;
using Core.Common.Core;
using EchoDesertTrips.Business.Entities;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EchoDesertTrips.Business.Entities
{
    [DataContract(IsReference = true)]
    public class ReservationDTO : EntityBase, IIdentifiableEntity
    {
        [DataMember]
        public int ReservationId { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string Phone1 { get; set; }
        //[DataMember]
        //public string PickupAddress { get; set; }
        [DataMember]
        public string HotelName { get; set; }
        [DataMember]
        public string AgencyName { get; set; }
        [DataMember]
        public decimal AdvancePayment { get; set; }
        [DataMember]
        public decimal TotalPrice { get; set; }
        [DataMember]
        public DateTime PickUpTime { get; set; }
        //[DataMember]
        //public DateTime ReservationStartDate { get; set; }
        [DataMember]
        public string Comments { get; set; }
        [DataMember]
        public string Messages { get; set; }
        [DataMember]
        virtual public Group Group { get; set; }
        [DataMember]
        public int GroupID { get; set; }
        [DataMember]
        public int ActualNumberOfCustomers { get; set; }
        [DataMember]
        public string FirstTourTypeName { get; set; }
        [DataMember]
        public bool Private { get; set; }
        [DataMember]
        public string Car { get; set; }
        [DataMember]
        public string Guide { get; set; }
        [DataMember]
        public string EndIn { get; set; }
        [DataMember]
        public List<Tour> Tours { get; set; }
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
