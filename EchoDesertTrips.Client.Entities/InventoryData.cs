using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Client.Entities
{
    public class InventoryData
    {
        private TripType[] _tripTypes;
        private LodggingPlace[] _lodggingPlaces;
        private int _stam;

        public TripType[] TripTypes
        {
            get
            {
                return _tripTypes;
            }

            set
            {
                _tripTypes = value;
            }
        }

        public LodggingPlace[] LodggingPlaces
        {
            get
            {
                return _lodggingPlaces;
            }

            set
            {
                _lodggingPlaces = value;
            }
        }

        public int Stam
        {
            get
            {
                return _stam;
            }

            set
            {
                _stam = value;
            }
        }
    }
}
