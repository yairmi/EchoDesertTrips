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
    [DataContract(IsReference = true)]
    public class ReservationView : EntityBase, IIdentifiableEntity
    {
        [DataMember]
        public int ReservationId { get; set; }
        [DataMember]
        public int Paxs { get; set; }
        [DataMember]
        public string PaxFullName { get; set; }
        [DataMember]
        public DateTime PickUpTime { get; set; }
        [DataMember]
        public string PickUpAddress { get; set; }
        [DataMember]
        public string Phone { get; set; }
        [DataMember]
        public string HotelName { get; set; }
        [DataMember]
        public string AgencyAndAgentName { get; set; }
        [DataMember]
        public double TotalPrice { get; set; }
        [DataMember]
        public double AdvancePayment { get; set; }
        [DataMember]
        public double Balance { get; set; }
        [DataMember]
        public string Comments { get; set; }
        [DataMember]
        public string Messages { get; set; }
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
