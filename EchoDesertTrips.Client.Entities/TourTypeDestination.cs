using Core.Common.Core;

namespace EchoDesertTrips.Client.Entities
{
    public class TourTypeDestination : ObjectBase
    {
        public int TourTypeDestinationId { get; set; }
        public int TourTypeId { get; set; }
        public int TourDestinationId { get; set; }

        private TourDestination _tourDestination;

        public TourDestination TourDestination
        {
            get
            {
                return _tourDestination;
            }
            set
            {
                if (_tourDestination != value)
                {
                    _tourDestination = value;
                    OnPropertyChanged(() => TourDestination);
                }
            }
        }
    }
}
