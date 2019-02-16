using System.Runtime.Serialization;
using static Core.Common.Core.Const;

namespace EchoDesertTrips.Client.Contracts
{
    [DataContract()]
    public class EventDataType
    {
        [DataMember]
        public string ClientName { get; set; }
        [DataMember]
        public eMsgTypes MessageType;
        [DataMember]
        public string EventMessage { get; set; }
    }
}
