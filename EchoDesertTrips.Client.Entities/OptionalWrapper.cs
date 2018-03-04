using Core.Common.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Client.Entities
{
    public class OptionalWrapper : ObjectBase
    {
        public int OptionalId { get; set; }

        private string _optionalDescription;

        public string OptionalDescription
        {
            get
            {
                return _optionalDescription;
            }

            set
            {
                if (_optionalDescription != value)
                {
                    _optionalDescription = value;
                    OnPropertyChanged(() => OptionalDescription);
                }
            }
        }

        private float _pricePerPerson;

        public float PricePerPerson
        {
            get
            {
                return _pricePerPerson;
            }
            set
            {
                if (_pricePerPerson != value)
                {
                    bool dirty = _pricePerPerson != 0;
                    _pricePerPerson = value;
                    OnPropertyChanged(() => PricePerPerson, dirty);
                }
            }
        }

        private float _priceInclusive;

        public float PriceInclusive
        {
            get
            {
                return _priceInclusive;
            }
            set
            {
                if (_priceInclusive != value)
                {
                    bool dirty = _priceInclusive != 0;
                    _priceInclusive = value;
                    OnPropertyChanged(() => PriceInclusive, dirty);
                }
            }
        }

        private ObservableCollection<Tour> _tours;

        public ObservableCollection<Tour> Tours
        {
            get
            {
                return _tours;
            }

            set
            {
                _tours = value;
                OnPropertyChanged(() => Tours, false);
            }
        }
    }
}
