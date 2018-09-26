using EchoDesertTrips.Business.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using EchoDesertTrips.Business.Entities;
using Core.Common.Utils;

namespace EchoDesertTrips.Business.Business_Engines
{
    [Export(typeof(IReservationEngine))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReservationEngine : IReservationEngine
    {
        private class Prices
        {
            //public string Serialize()
            //{
            //    return $"{Persons},{Price};";
            //}

            public void Deserialize(string pair)
            {
                string[] prices = SimpleSplitter.Split(pair);
                _persons = Int32.Parse(prices[0]);
                _price = Double.Parse(prices[1]);
            }

            //private int _persons;

            public int Persons;
            /*{
                get
                {
                    return _persons;
                }
                set
                {
                    _persons = value;
                    OnPropertyChanged(() => Persons, true);
                }
            }*/

            //private double _price;

            public double Price;
            /*{
                get
                {
                    return _price;
                }
                set
                {
                    _price = value;
                    OnPropertyChanged(() => Price, true);
                }
            }*/
        }
        public double CalculateReservationTotalPrice(Reservation reservation)
        {
            double totalPrice = 0;
            var adultPrices = new Dictionary<int, double>();
            var childPrices = new Dictionary<int, double>();
            var infantPrices = new Dictionary<int, double>();
            foreach (var tour in reservation.Tours)
            {
                adultPrices.Clear();
                childPrices.Clear();
                infantPrices.Clear();
                TimeSpan dt2 = tour.EndDate.Subtract(tour.StartDate);

                string[] separators = { ";" };
                string[] pairs;
                if (tour.TourType.AdultPrices != null)
                {
                    pairs = SimpleSplitter.Split(tour.TourType.AdultPrices, separators);
                    foreach (var pair in pairs)
                    {
                        var prices = new Prices();
                        prices.Deserialize(pair);
                        adultPrices.Add(prices.Persons, prices.Price);
                    }
                }
                if (tour.TourType.ChildPrices != null)
                {
                    pairs = SimpleSplitter.Split(tour.TourType.ChildPrices, separators);
                    foreach (var pair in pairs)
                    {
                        var prices = new Prices();
                        prices.Deserialize(pair);
                        childPrices.Add(prices.Persons, prices.Price);
                    }
                }
                if (tour.TourType.InfantPrices != null)
                {
                    pairs = SimpleSplitter.Split(tour.TourType.InfantPrices, separators);
                    foreach (var pair in pairs)
                    {
                        var prices = new Prices();
                        prices.Deserialize(pair);
                        infantPrices.Add(prices.Persons, prices.Price);
                    }
                }

                double value = 0;
                if (adultPrices.TryGetValue(reservation.Adults, out value))
                {
                    totalPrice += value * reservation.Adults;
                }
                else
                {
                    if (adultPrices.Count > 0)
                        totalPrice += adultPrices[adultPrices.Keys.Max()] * reservation.Adults;
                }

                if (childPrices.TryGetValue(reservation.Childs, out value))
                {
                    totalPrice += value * reservation.Childs;
                }
                else
                {
                    if (childPrices.Count > 0)
                        totalPrice += childPrices[childPrices.Keys.Max()] * reservation.Childs;
                }

                if (infantPrices.TryGetValue(reservation.Infants, out value))
                {
                    totalPrice += value * reservation.Infants;
                }
                else
                {
                    if (infantPrices.Count > 0)
                        totalPrice += infantPrices[infantPrices.Keys.Max()] * reservation.Infants;
                }

                foreach (var tourOptional in tour.TourOptionals)
                {
                    totalPrice += tourOptional.PriceInclusive == true ? tourOptional.Optional.PriceInclusive :
                        tourOptional.Optional.PricePerPerson * reservation.Customers.Count;
                }

                tour.TourHotels.ToList().ForEach((tourHotel) =>
                {
                    tourHotel.TourHotelRoomTypes.ToList().ForEach((tourHotelRoomType) =>
                    {
                        totalPrice += tourHotelRoomType.Persons * tourHotelRoomType.HotelRoomType.PricePerPerson;
                    });
                });
            }
            return totalPrice;
        }
    }
}
