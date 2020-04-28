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

        private float _PricePerPerson;

        public float PricePerPerson
        {
            get
            {
                return _PricePerPerson;
            }
            set
            {
                _PricePerPerson = value;
            }
        }

        private float _priceInclusiveValue;

        public float PriceInclusiveValue
        {
            get
            {
                return _priceInclusiveValue;
            }
            set
            {
                _priceInclusiveValue = value;
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
