using Core.Common.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Client.Entities
{
    public class SubTour : ObjectBase
    {
        public int SubTourId { get; set; }

        private string _destinationName;

        public string DestinationName
        {
            get
            {
                return _destinationName;
            }
            set
            {
                if (_destinationName != value)
                {
                    _destinationName = value;
                    OnPropertyChanged(() => DestinationName);
                }
            }
        }

        //private bool _private;

        //public bool Private
        //{
        //    get
        //    {
        //        return _private;
        //    }
        //    set
        //    {
        //        _private = value;
        //        OnPropertyChanged(() => Private);
        //    }
        //}

        //public DateTime StartDate { get; set; }
    }
}
