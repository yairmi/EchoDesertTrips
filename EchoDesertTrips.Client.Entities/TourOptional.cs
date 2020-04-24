using Core.Common.Core;

namespace EchoDesertTrips.Client.Entities
{
    public class TourOptional : ObjectBase
    {
        public int TourId { get; set; }
        public int OptionalId { get; set; }

        private bool _priceInclusive;

        public bool PriceInclusive
        {
            get
            {
                return _priceInclusive;
            }
            set
            {
                _priceInclusive = value;
                OnPropertyChanged(() => PriceInclusive, true);
            }
        }

        public Optional Optional { get; set; }

        private float _originalPricePerPerson;

        public float OriginalPricePerPerson
        {
            get
            {
                return _originalPricePerPerson;
            }
            set
            {
                _originalPricePerPerson = value;
                OnPropertyChanged(() => OriginalPricePerPerson, true);
            }
        }

        private float _originalPriceInclusive;

        public float OriginalPriceInclusive
        {
            get
            {
                return _originalPriceInclusive;
            }
            set
            {
                _originalPriceInclusive = value;
                OnPropertyChanged(() => OriginalPriceInclusive, true);
            }
        }

        private bool _selected;

        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    OnPropertyChanged(() => Selected, true);
                }
            }
        }


    }
}
