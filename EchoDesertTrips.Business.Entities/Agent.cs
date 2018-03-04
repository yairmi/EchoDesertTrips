using Core.Common.Contracts;
using Core.Common.Core;
using System;
using System.Collections.Generic;
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
        //[DataMember]
        //public Agency Agency { get; set; }
        //[DataMember]
        //public int AgencyId { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string FullName { get; set; }
        [DataMember]
        public string Phone1 { get; set; }
        [DataMember]
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
