using Core.Common.ServiceModel;
using EchoDesertTrips.Business.Entities;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using static Core.Common.Core.Const;

namespace EchoDesertTrips.Business.Contracts
{
    [DataContract()]
    public class BroadcastMessage : DataContractBase
    {
        public BroadcastMessage()
        {
            ReservationsResult = new List<Reservation>();
            Inventories = new InventoryData();
        }
        [DataMember]
        public eMsgTypes MessageType { get; set; }
        [DataMember]
        public List<Reservation> ReservationsResult { get; set; }
        [DataMember]
        public InventoryData Inventories { get; set; }
    }


}
