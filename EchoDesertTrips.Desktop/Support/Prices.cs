﻿using Core.Common.Core;
using Core.Common.Utils;
using System;

namespace EchoDesertTrips.Desktop.Support
{
    public class Prices : ObjectBase
    {
        public string Serialize()
        {
            return $"{Persons},{Price};";
        }

        public void Deserialize(string pair)
        {
            string[] prices = SimpleSplitter.Split(pair);
            Persons = Int32.Parse(prices[0]);
            Price = decimal.Parse(prices[1]);
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

        private decimal _price;

        public decimal Price
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
