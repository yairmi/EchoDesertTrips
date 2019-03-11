using EchoDesertTrips.Client.Entities;
using System;

namespace Core.Common.UI.CustomEventArgs
{
    public class AgencyEventArgs : EventArgs
    {
        public AgencyEventArgs(Agency agency, Agent agent, bool bIsNew)
        {
            Agency = agency;
            Agent = agent;
            IsNew = bIsNew;
        }
        public Agency Agency { get; set; }
        public Agent Agent { get; set; }
        public bool IsNew { get; set; }
    }
}
