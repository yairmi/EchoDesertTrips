using EchoDesertTrips.Business.Managers.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ServiceModel.Samples
{
    public class DesertEcoTours : ServiceBase
    {
        public ServiceHost hostInventoryManager = null;
        public ServiceHost HostOrderManager = null;
        public ServiceHost HostOperatorManager = null;
        public ServiceHost HostBroadcastorManager = null;

        public DesertEcoTours()
        {
            ServiceName = "WCFWindowsServiceEcoDesertTours";
        }

        public void Main()
        {
            ServiceBase.Run(new DesertEcoTours());
        }

        protected override void OnStart(string[] args)
        {
            if (hostInventoryManager != null)
            {
                hostInventoryManager.Close();
            }
            if (HostOrderManager != null)
            {
                HostOrderManager.Close();
            }
            if (HostOperatorManager != null)
            {
                HostOperatorManager.Close();
            }
            if (HostBroadcastorManager != null)
            {
                HostBroadcastorManager.Close();
            }

            // Create a ServiceHost for the CalculatorService type and 
            // provide the base address.
            hostInventoryManager = new ServiceHost(typeof(InventoryManager));
            HostOrderManager = new ServiceHost(typeof(ReservationManager));
            HostOperatorManager = new ServiceHost(typeof(OperatorManager));
            HostBroadcastorManager = new ServiceHost(typeof(BroadcastorManager));

            // Open the ServiceHostBase to create listeners and start 
            // listening for messages.
            hostInventoryManager.Open();
            HostOrderManager.Open();
            HostOperatorManager.Open();
            HostBroadcastorManager.Open();
        }

        protected override void OnStop()
        {
            if (hostInventoryManager != null)
            {
                hostInventoryManager.Close();
                hostInventoryManager = null;
            }
            if (HostOrderManager != null)
            {
                HostOrderManager.Close();
                HostOrderManager = null;
            }
            if (HostOperatorManager != null)
            {
                HostOperatorManager.Close();
                HostOperatorManager = null;
            }
            if (HostBroadcastorManager != null)
            {
                HostBroadcastorManager.Close();
                HostBroadcastorManager = null;
            }
        }

    }
}
