namespace EchoDesertTrips.Client.Entities
{
    public class OperatorTrip
    {
        private int _operatorTripId;
        private int _operatorId;
        private int _tripId;

        public int OperatorTripId
        {
            get
            {
                return _operatorTripId;
            }

            set
            {
                _operatorTripId = value;
            }
        }

        public int OperatorId
        {
            get
            {
                return _operatorId;
            }

            set
            {
                _operatorId = value;
            }
        }

        public int TripId
        {
            get
            {
                return _tripId;
            }

            set
            {
                _tripId = value;
            }
        }
    }
}
