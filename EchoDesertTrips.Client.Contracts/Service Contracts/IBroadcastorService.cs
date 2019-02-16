using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Client.Contracts
{
    //Since it is a duplex service we need to decorate it with CallbackContract
    [ServiceContract(CallbackContract = typeof(IBroadcastorCallBack))]
    public interface IBroadcastorService
    {
        [OperationContract]
        void RegisterClient(string clientName);

        [OperationContract(IsOneWay = true)]
        void UnRegisterClient(string clientName);

        [OperationContract(IsOneWay = true)]
        void NotifyServer(EventDataType eventData);
    }

    public interface IBroadcastorCallBack
    {
        [OperationContract(IsOneWay = true)]
        void BroadcastToClient(BroadcastMessage Message);
    }
}
