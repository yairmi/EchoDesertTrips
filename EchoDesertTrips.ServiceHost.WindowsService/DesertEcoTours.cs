﻿using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using SM = System.ServiceModel;
using EchoDesertTrips.Business.Managers.Managers;

namespace EchoDesertTrips.ServiceHost.WindowsService
{
    public enum ServiceState
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public int dwServiceType;
        public ServiceState dwCurrentState;
        public int dwControlsAccepted;
        public int dwWin32ExitCode;
        public int dwServiceSpecificExitCode;
        public int dwCheckPoint;
        public int dwWaitHint;
    };

    public partial class DesertEcoTours : ServiceBase
    {
        public DesertEcoTours(string[] args)
        {
            InitializeComponent();
            string eventSourceName = "DesertEcoToursSource";
            string logName = "DesertEcoToursNewLog";
            if (args.Count() > 0)
            {
                eventSourceName = args[0];
            }
            if (args.Count() > 1)
            {
                logName = args[1];
            }
            //eventLog1 = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists(eventSourceName))
            {
                System.Diagnostics.EventLog.CreateEventSource(eventSourceName, logName);
            }
            eventLog1.Source = eventSourceName;
            eventLog1.Log = logName;

            eventLog1.WriteEntry("Ctor Ending");
        }

        //Service host defenition at class level
        private SM.ServiceHost hostInventoryManager;
        private SM.ServiceHost HostOrderManager;
        private SM.ServiceHost HostOperatorManager;
        private SM.ServiceHost HostBroadcastorManager;

        protected override void OnStart(string[] args)
        {
            //Create the service host
            //open the host
            // Update the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            eventLog1.WriteEntry("In OnStart");
            // Set up a timer to trigger every minute.  
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60000; // 60 seconds  
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();

            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            hostInventoryManager = new SM.ServiceHost(typeof(InventoryManager));
            HostOrderManager = new SM.ServiceHost(typeof(ReservationManager));
            HostOperatorManager = new SM.ServiceHost(typeof(OperatorManager));
            HostBroadcastorManager = new SM.ServiceHost(typeof(BroadcastorManager));

            hostInventoryManager.Open();
            HostOrderManager.Open();
            HostOperatorManager.Open();
            HostBroadcastorManager.Open();

            eventLog1.WriteEntry("OnStart Ending");
        }

        protected override void OnStop()
        {
            hostInventoryManager.Close();
            HostOrderManager.Close();
            HostOperatorManager.Close();
            HostBroadcastorManager.Close();
            // Update the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            eventLog1.WriteEntry("In onStop.");

            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnContinue()
        {
            eventLog1.WriteEntry("In OnContinue.");
            base.OnContinue();
        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            // TODO: Insert monitoring activities here.  
            eventLog1.WriteEntry("Monitoring the System", EventLogEntryType.Information, eventId++);
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);


        private int eventId = 1;
    }
}
