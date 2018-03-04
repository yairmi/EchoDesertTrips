using System.Runtime.Serialization;

namespace EchoDesertTrips.Client.Contracts
{
    [DataContract()]
    public class EventDataType
    {
        [DataMember]
        public string ClientName { get; set; }

        [DataMember]
        public string EventMessage { get; set; }
    }
}
