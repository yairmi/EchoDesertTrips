using EchoDesertTrips.Client.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Client.Contracts.Data_Contracts
{
    class GetMessageCompletedEventArgs : AsyncCompletedEventArgs
    {
        private readonly object[] _results;

        public GetMessageCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) :
         base(exception, cancelled, userState)
        {
            _results = results;
        }

        public Reservation Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return ((Reservation)(_results[0]));
            }
        }
    }
}
