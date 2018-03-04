using Core.Common.Contracts;
using Core.Common.Core;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EchoDesertTrips.Business.Entities
{
    [DataContract]
    public class Agency : EntityBase, IIdentifiableEntity
    {
        [DataMember]
        public int AgencyId { get; set; }
        [DataMember]
        public string AgencyName { get; set; }
        [DataMember]
        public string AgencyAddress { get; set; }
        [DataMember]
        public string Phone1 { get; set; }
        [DataMember]
        public string Phone2 { get; set; }
        [DataMember]
        public List<Agent> Agents { get; set; }

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
