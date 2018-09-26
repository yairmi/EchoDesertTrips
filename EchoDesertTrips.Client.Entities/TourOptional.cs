using Core.Common.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Client.Entities
{
    public class TourOptional : ObjectBase
    {
        public int TourId { get; set; }
        public int OptionalId { get; set; }
        public Optional Optional { get; set; }

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
