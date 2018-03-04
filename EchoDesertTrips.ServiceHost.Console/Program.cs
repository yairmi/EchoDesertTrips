using Bootstrapper;
using Core.Common.Core;
using EchoDesertTrips.Business.Entities;
using EchoDesertTrips.Business.Managers.Managers;
using System;
using System.Security.Principal;
using System.Threading;
using System.Timers;
using System.Transactions;
using SM = System.ServiceModel;
using Timer = System.Timers.Timer;

namespace EchoDesertTrips.ServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            //GenericPrincipal principal = new GenericPrincipal(new GenericIdentity("Miguel"), new string[] { "CarRentalAdmin" });

            //Thread.CurrentPrincipal = principal;
            log4net.Config.XmlConfigurator.Configure();
            ObjectBase.Container = MEFLoader.Init();

            Console.WriteLine("Starting up services");
            Console.WriteLine("");

            SM.ServiceHost hostInventoryManager = new SM.ServiceHost(typeof(InventoryManager));
            SM.ServiceHost HostOrderManager = new SM.ServiceHost(typeof(ReservationManager));
            SM.ServiceHost HostOperatorManager = new SM.ServiceHost(typeof(OperatorManager));
            SM.ServiceHost HostBroadcastorManager = new SM.ServiceHost(typeof(BroadcastorManager));

            StartService(hostInventoryManager, "InventoryManager");
            StartService(HostOrderManager, "OrderManager");
            StartService(HostOperatorManager, "OperatorManager");
            StartService(HostBroadcastorManager, "BroadcastorManager");

            //System.Timers.Timer timer = new Timer(10000);
            //timer.Elapsed += OnTimerElapsed;
            //timer.Start();


            Console.WriteLine("");
            Console.WriteLine("Press [Enter] to exit.");
            Console.ReadLine();

            //timer.Stop();

            Console.WriteLine("Reservation Monitor Stopped");

            StopService(hostInventoryManager, "InventoryManager");
            StopService(HostOrderManager, "OrderManager");
            StopService(HostOperatorManager, "OperatorManager");
            StopService(HostOperatorManager, "BroadcastorManager");
        }

        private static void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            var timer = (Timer)sender;
            timer.Stop();

            Console.WriteLine("Looking for dead reservations as {0}", DateTime.Now.ToString());

            ReservationManager orderManager = new ReservationManager();

            Reservation[] reservations = orderManager.GetDeadReservations();
            if (reservations != null)
            {
                foreach (var reservation in reservations)
                {
                    //Do the remove in a transaction context
                    using (TransactionScope scope = new TransactionScope())
                    {
                        try
                        {
                            orderManager.CancelReservation(reservation.ReservationId);
                            Console.WriteLine("Cancelling reservation '{0}'", reservation.ReservationId);
                            scope.Complete();
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("There was an exception when attempting to cancel reservation '{0}'", reservation.ReservationId);
                        }

                    }
                }
            }

            timer.Start();
        }

        static void StartService(SM.ServiceHost host, string serviceDescription)
        {
            host.Open();
            Console.WriteLine("Service {0} started.", serviceDescription);

            foreach (var endpoint in host.Description.Endpoints)
            {
                Console.WriteLine("Listening on endpoint:");
                Console.WriteLine("Address: {0}", endpoint.Address.Uri);
                Console.WriteLine("Binding: {0}", endpoint.Binding.Name);
                Console.WriteLine("Contract: {0}", endpoint.Contract.Name);
            }

            Console.WriteLine();

        }

        static void StopService(SM.ServiceHost host, string serviceDescription)
        {
            host.Close();
            Console.WriteLine("Service {0} Stopped.", serviceDescription);
        }
    }
}
