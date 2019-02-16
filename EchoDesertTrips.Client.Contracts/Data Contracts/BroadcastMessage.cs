using Core.Common.ServiceModel;
using EchoDesertTrips.Client.Entities;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using static Core.Common.Core.Const;

namespace EchoDesertTrips.Client.Contracts
{
    [DataContract()]
    public class BroadcastMessage : DataContractBase
    {
        [DataMember]
        public eMsgTypes MessageType { get; set; }
        [DataMember]
        public List<Reservation> ReservationsResult { get; set; }
        [DataMember]
        public InventoryData Inventories { get; set; }
    }
}
