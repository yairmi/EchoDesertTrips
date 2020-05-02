using Core.Common.Contracts;
using Core.Common.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace EchoDesertTrips.Business.Entities
{
    [DataContract]
    public class Customer : EntityBase, IIdentifiableEntity
    {
        [DataMember]
        public int CustomerId { get; set; }
        [DataMember]
        [Column(TypeName = "VARCHAR")]
        [StringLength(10)]
        public string IdentityId { get; set; }
        [DataMember]
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        [DataMember]
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        //[DataMember]
        //public string FullName { get; set; }
        [DataMember]
        [Required]
        [Column(TypeName = "VARCHAR")]
        [StringLength(30)]
        public string Phone1 { get; set; }
        [DataMember]
        [Column(TypeName = "VARCHAR")]
        [StringLength(30)]
        public string Phone2 { get; set; }
        [DataMember]
        public DateTime DateOfBirdth { get; set; }
        [DataMember]
        [Column(TypeName = "VARCHAR")]
        [StringLength(30)]
        public string PassportNumber { get; set; }
        [DataMember]
        public DateTime IssueData { get; set; }
        [DataMember]
        public DateTime ExpireyDate { get; set; }
        [DataMember]
        [MaxLength(10)]
        public string Nationality { get; set; }
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
