using EchoDesertTrips.Client.Entities;
using System;

namespace Core.Common.UI.CustomEventArgs
{
    public class AgencyEventArgs : EventArgs
    {
        public AgencyEventArgs(Agency agency, bool IsNew, bool sendUpdateToClients = true)
        {
            Agency = agency;
            bIsNew = IsNew;
            bSendUpdateToClients = sendUpdateToClients;
        }
        public Agency Agency { get; set; }
        public bool bIsNew { get; set; }
        public bool bSendUpdateToClients { get; set; }
    }
}
