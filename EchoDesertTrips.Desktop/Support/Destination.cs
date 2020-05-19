using Core.Common.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Desktop.Support
{
    public class Destination : ObjectBase
    {
        public string Serialize()
        {
            return $"{DestinationName};";
        }

        public void Deserialize(string destinationName)
        {
            DestinationName = destinationName;
        }

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
    }
}
