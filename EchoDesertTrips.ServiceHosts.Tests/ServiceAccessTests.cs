using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel;
using EchoDesertTrips.Business.Contracts;

namespace EchoDesertTrips.ServiceHosts.Tests
{
    [TestClass]
    public class ServiceAccessTests
    {
        [TestMethod]
        public void test_inventory_manager_as_service()
        {
            //First create an instance of the channel factory
            ChannelFactory<IInventoryService> channelFactory =
                new ChannelFactory<IInventoryService>("InventoryEndPoint");
            //Create the proxy using this ChannelFactory
            //The return type of that proxy will be the service contract type
            //because thats what I'm going to be able to make calls on

            IInventoryService proxy = channelFactory.CreateChannel();

            //We want to check just connectivity
            //In the case of proxy class it is done with the open statement.
            //In out case the "proxy" object is of type "IInventoryService" and we don't
            //have there an "open" method.
            //The solution is to pass to ICommunicationObject the proxy and then make an open call
            (proxy as ICommunicationObject).Open();
            channelFactory.Close();
        }
    }
}
