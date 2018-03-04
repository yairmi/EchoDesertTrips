using EchoDesertTrips.Client.Proxies.Service_Proxies;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EchoDesertTrips.Client.Proxies.Test
{
    [TestClass]
    public class ServiceAccessTests
    {
        [TestMethod]
        public void test_inventory_client_connection()
        {
            //This time we are going to use an actual client proxy.
            InventoryClient proxy = new InventoryClient();
//            proxy.GetTrip(1);

            proxy.Open();
        }
    }
}
