using Core.Common.Contracts;
using Core.Common.Core;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace EchoDesertTrips.Business.Entities
{
    [DataContract]
    public class Agency : EntityBase, IIdentifiableEntity
    {
        [DataMember]
        public int AgencyId { get; set; }
        [DataMember]
        [Required]
        [MaxLength(50)]
        public string AgencyName { get; set; }
        [DataMember]
        [MaxLength(50)]
        public string AgencyAddress { get; set; }
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
        virtual public List<Agent> Agents { get; set; }

        public int EntityId
        {
            get
            {
                return AgencyId;
            }

            set
            {
                AgencyId = value;
            }
        }
    }
}
