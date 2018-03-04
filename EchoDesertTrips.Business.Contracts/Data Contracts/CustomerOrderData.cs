using Core.Common.ServiceModel;
using EchoDesertTrips.Business.Entities;
using System;
using System.Runtime.Serialization;

namespace EchoDesertTrips.Business.Contracts
{
    [DataContract]
    public class CustomerOrderData : DataContractBase
    {
        [DataMember]
        public Customer Customer { get; set; }
        [DataMember]
        public Reservation Reservation { get; set; }
        [DataMember]
        public Tour Trip { get; set; }
        [DataMember]
        public Agency Agency { get; set; }
        [DataMember]
        public Agent Agent { get; set; }
    }
}
