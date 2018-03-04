using Microsoft.VisualStudio.TestTools.UnitTesting;
using EchoDesertTrips.Client.Contracts;
using Core.Common.Core;
using Core.Common.Contracts;
using EchoDesertTrips.Client.Bootstrapper;
using EchoDesertTrips.Client.Proxies.Service_Proxies;

namespace EchoDesertTrips.Client.Proxies.Test
{
    [TestClass]
    public class ProxyObtainmentTests
    {
        [TestInitialize]
        public void Initialize()
        {
            ObjectBase.Container = MEFLoader.Init();
        }

        [TestMethod]
        public void obtain_proxy_from_container_using_service_contract()
        {
            IInventoryService proxy = 
                ObjectBase.Container.GetExportedValue<IInventoryService>();

            Assert.IsTrue(proxy is InventoryClient);
        }

        [TestMethod]
        public void obtain_proxy_from_service_factory()
        {
            IServiceFactory serviceFactory = new ServiceFactory();
            IInventoryService proxy = serviceFactory.CreateClient<IInventoryService>();

            Assert.IsTrue(proxy is InventoryClient);
        }

        [TestMethod]
        public void obtain_service_factory_and_proxy_from_container()
        {
            IServiceFactory factory =
                ObjectBase.Container.GetExportedValue<IServiceFactory>();

            IInventoryService proxy = factory.CreateClient<IInventoryService>();

            Assert.IsTrue(proxy is InventoryClient);
        }
    }
}
