using Bootstrapper;
using Core.Common.Core;
using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.ServiceHost.WindowsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>  
        static void Main(string[] args)
        {
            if (System.Environment.UserInteractive)
            {

                if (args.Length > 0)
                {
                    switch (args[0])
                    {
                        case "-install":
                            {
                                ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                                break;
                            }
                        case "-uninstall":
                            {
                                ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                                break;
                            }
                    }
                }
            }
            else
            {
                log4net.Config.XmlConfigurator.Configure();
                ObjectBase.Container = MEFLoader.Init();
                ServiceBase[] ServicesToRun = new ServiceBase[] { new DesertEcoTours(args) };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
