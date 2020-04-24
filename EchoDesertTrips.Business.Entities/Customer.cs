using Core.Common.Contracts;
using Core.Common.Core;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EchoDesertTrips.Business.Entities
{
    [DataContract]
    public class Customer : EntityBase, IIdentifiableEntity
    {
        [DataMember]
        public int CustomerId { get; set; }
        [DataMember]
        public string IdentityId { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        //[DataMember]
        //public string FullName { get; set; }
        [DataMember]
        public string Phone1 { get; set; }
        [DataMember]
        public string Phone2 { get; set; }
        [DataMember]
        public DateTime DateOfBirdth { get; set; }
        [DataMember]
        public string PassportNumber { get; set; }
        [DataMember]
        public DateTime IssueData { get; set; }
        [DataMember]
        public DateTime ExpireyDate { get; set; }
        [DataMember]
        public string Nationality { get; set; }
        //[DataMember]
        //public int NationalityId { get; set; }
        [DataMember]
        public bool HasVisa { get; set; }
 
        public int EntityId
        {
            get
            {
                return CustomerId;
            }

            set
            {
                CustomerId = value;
            }
        }
    }
}
