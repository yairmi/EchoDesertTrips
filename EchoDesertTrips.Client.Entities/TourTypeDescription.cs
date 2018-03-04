using Core.Common.Core;

namespace EchoDesertTrips.Client.Entities
{
    public class TourTypeDescription : ObjectBase
    {
        public int TourTypeDescriptionId { get; set; }

        private string _description;

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                OnPropertyChanged(() => Description);
            }
        }

        private string _dayNumber;

        public string DayNumber
        {
            get
            {
                return _dayNumber;
            }
            set
            {
                _dayNumber = value;
                OnPropertyChanged(() => DayNumber);
            }
        }
    }
}
