using Core.Common.Extensions;
using Core.Common.Utils;
using EchoDesertTrips.Client.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EchoDesertTrips.Desktop.Support
{
    public class ReservationUtils
    {
        public static double CalculateReservationTotalPrice(ReservationWrapper reservation)
        {
            var adultPersons = reservation.Customers.ToList()
                .Where((customer) => reservation.CreationTime.Subtract(customer.DateOfBirdth).TotalDays > 4380);
            var adults = adultPersons.Count();
            var childrens = reservation.Customers.Count - adults;
            //foreach (var customer in reservation.Customers)
            //{
            //    TimeSpan dt2 = reservation.CreationTime.Subtract(customer.DateOfBirdth);
            //    double year = dt2.TotalDays / 365;
            //    if (year > 12)
            //        adults++;
            //    else
            //        childrens++;
            //}
            double totalPrice = 0;
            var adultPrices = new Dictionary<int,double>();
            var childPrices = new Dictionary<int,double>();
            foreach (var tour in reservation.Tours)
            {
                adultPrices.Clear();
                childPrices.Clear();
                TimeSpan dt2 = tour.EndDate.Subtract(tour.StartDate);

                string[] separators = { ";" };
                string[] pairs = SimpleSplitter.Split(tour.TourType.AdultPrices, separators);
                foreach (var pair in pairs)
                {
                    var prices = new Prices();
                    prices.Deserialize(pair);
                    adultPrices.Add(prices.Persons, prices.Price);
                }

                pairs = SimpleSplitter.Split(tour.TourType.ChildPrices, separators);
                foreach (var pair in pairs)
                {
                    var prices = new Prices();
                    prices.Deserialize(pair);
                    childPrices.Add(prices.Persons, prices.Price);
                }
                double value = 0;
                if (adultPrices.TryGetValue(adults, out value))
                {
                    totalPrice += value * adults;
                }
                else
                {
                    if (adultPrices.Count > 0)
                        totalPrice += adultPrices[adultPrices.Keys.Max()] * adults;
                }

                if (childPrices.TryGetValue(childrens, out value))
                {
                    totalPrice += value * childrens;
                }
                else
                {
                    if (childPrices.Count > 0)
                        totalPrice += childPrices[childPrices.Keys.Max()] * childrens;
                }

                foreach (var tourOptional in tour.TourOptionals)
                {
                    totalPrice += tourOptional.PriceInclusive == true? tourOptional.Optional.PriceInclusive:
                        tourOptional.Optional.PricePerPerson * reservation.Customers.Count;
                }

                //foreach (var tourHotelRoomType in tour.TourHotelRoomTypes)
                //{
                //    totalPrice += tourHotelRoomType.Capacity * tourHotelRoomType.Persons * tourHotelRoomType.HotelRoomType.PricePerPerson;
                //}
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

        public static void CreateExternalId(ReservationWrapper reservation)
        {
            if (reservation.Tours[0].TourType.IncramentExternalId)
            {
                StringBuilder reservationForHashCode = new StringBuilder();
                reservationForHashCode.Append(reservation.Tours[0].StartDate.Date.ToString("d"));
                reservation.Tours.ToList().ForEach((tour) =>
                {
                    reservationForHashCode.Append(tour.TourType.TourTypeName);
                    reservationForHashCode.Append(tour.TourType.Private);
                });
                if (reservation.Group == null)
                    reservation.Group = new Group();
                reservation.Group.ExternalId = HashCodeUtil.GetHashCodeBernstein(reservationForHashCode.ToString());
            }

        }

        public static void RemoveUnselectedHotels(ReservationWrapper reservation)
        {
            reservation.Tours.ToList().ForEach((tour) =>
            {
                tour.TourHotels.ToList().ForEach((tourHotel) =>
                {
                    tourHotel.TourHotelRoomTypes.RemoveItems(t => t.Persons == 0 && t.Capacity == 0);
                });

                tour.TourHotels.RemoveItems(t => !t.TourHotelRoomTypes.Any());
            });

            
        }

        public static void RemoveUnselectedOptionals(ReservationWrapper reservation)
        {
            reservation.Tours.ToList().ForEach((tour) =>
            {
                tour.TourOptionals.RemoveItems(t => t.Selected == false);
            });
        }

        //int hf(string const& s) 
        //{
        //    ulong hash = 5381;
        //    for (auto c : s) 
        //    {
        //        hash = (hash << 5) + hash + c; /* hash * 33 + c */
        //    }
        //    return hash;
        //}
    }
}
