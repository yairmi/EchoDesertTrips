using Core.Common.Core;
using Core.Common.Utils;
using System;

namespace EchoDesertTrips.Desktop.Support
{
    public class Prices : ObjectBase
    {
        public string Serialize()
        {
            return String.Format("{0},{1};", Persons, Price);
        }

        public void Deserialize(string pair)
        {
            string[] prices = SimpleSplitter.Split(pair);
            Persons = Int32.Parse(prices[0]);
            Price = Double.Parse(prices[1]);
        }

        private int _persons;

        public int Persons
        {
            get
            {
                return _persons;
            }
            set
            {
                _persons = value;
                OnPropertyChanged(() => Persons, true);
            }
        }

        private double _price;

        public double Price
        {
            get
            {
                return _price;
            }
            set
            {
                _price = value;
                OnPropertyChanged(() => Price, true);
            }
        }
    }
}
