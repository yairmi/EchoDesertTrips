using Core.Common.Contracts;
using Core.Common.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace EchoDesertTrips.Business.Entities
{
    [DataContract]
    public class Agent : EntityBase, IIdentifiableEntity
    {
        [DataMember]
        public int AgentId { get; set; }
        [DataMember]
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        [DataMember]
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        [DataMember]
        [MaxLength(100)]
        public string FullName { get; set; }
        [DataMember]
        [Required]
        [Column(TypeName = "VARCHAR")]
        [StringLength(30)]
        public string Phone1 { get; set; }
        [DataMember]
        [Column(TypeName = "VARCHAR")]
        [StringLength(30)]
        public string Phone2 { get; set; }

        public int EntityId
        {
            get
            {
                return AgentId;
            }

            set
            {
                AgentId = value;
            }
        }
    }
}
