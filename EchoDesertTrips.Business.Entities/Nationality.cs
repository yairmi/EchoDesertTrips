using Core.Common.Contracts;
using Core.Common.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Business.Entities
{
    [DataContract]
    public class Nationality : EntityBase, IIdentifiableEntity
    {
        [DataMember]
        public int NationalityId { get; set; }
        [DataMember]
        public string NationalityName { get; set; }
        [DataMember]
        public bool Visible { get; set; }

        public int EntityId
        {
            get
            {
                return NationalityId;
            }

            set
            {
                NationalityId = value;
            }
        }
    }
}
