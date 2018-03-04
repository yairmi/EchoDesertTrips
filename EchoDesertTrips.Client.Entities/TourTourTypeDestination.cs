using Core.Common.Core;
using System;

namespace EchoDesertTrips.Client.Entities
{
    public class TourTourTypeDestination : ObjectBase
    {
        public int TourId { get; set; }
        public int TourTypeDestinationId { get; set; }

        private DateTime _startDate;

        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    OnPropertyChanged(() => StartDate);
                }
            }
        }

        private DateTime _endDate;

        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    OnPropertyChanged(() => EndDate);
                }
            }
        }
    }
}
