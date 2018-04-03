using Core.Common.Contracts;
using Core.Common.Core;
using System;
using System.Runtime.Serialization;

namespace EchoDesertTrips.Business.Entities
{
    public class Group : EntityBase, IIdentifiableEntity
    {
        [DataMember]
        public int GroupId { get; set; }
        [DataMember]
        public string ExternalId { get; set; }
        [DataMember]
        public bool Updated { get; set; }
        public int EntityId
        {
            get
            {
                return GroupId;
            }

            set
            {
                GroupId = value;
            }
        }
    }
}
