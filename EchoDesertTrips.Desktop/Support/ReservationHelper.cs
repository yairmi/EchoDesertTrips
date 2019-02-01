using Core.Common.Extensions;
using Core.Common.Utils;
using EchoDesertTrips.Client.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace EchoDesertTrips.Desktop.Support
{
    public class ReservationHelper
    {
        public static double CalculateReservationTotalPrice(Reservation reservation)
        {
            double totalPrice = 0;
            var adultPrices = new Dictionary<int,double>();
            var childPrices = new Dictionary<int,double>();
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
                    totalPrice += tourOptional.PriceInclusive == true? tourOptional.Optional.PriceInclusive:
                        tourOptional.Optional.PricePerPerson * reservation.Customers.Count;
                }
                tour.TourHotels.ToList().ForEach((tourHotel) =>
                {
                    tourHotel.TourHotelRoomTypes.ToList().ForEach((tourHotelRoomType) =>
                    {

                        foreach(var daysRange in tourHotelRoomType.HotelRoomType.HotelRoomTypeDaysRanges)
                        {
                            int days = GetDaysInRange(tour, daysRange);
                            totalPrice += tourHotelRoomType.Persons * daysRange.PricePerPerson * days;
                        }
                    });
                });
            }
            return totalPrice;
        }

        private static int GetDaysInRange(Tour tour, HotelRoomTypeDaysRange hotelRoomTypeDaysRange)
        {
            int result = 0;
            for (int i = 0; i < tour.TourType.Days; i++)
            {
                var date = tour.StartDate.AddDays(i);
                if (date >= hotelRoomTypeDaysRange.StartDaysRange && date <= hotelRoomTypeDaysRange.EndDaysRange)
                {
                    result++;
                }
            }
            return result;
        }

        public static int GetCustomerLeft(Reservation reservation)
        {
            var customersInHotels = 0;
            reservation.Tours?.ToList().ForEach((tour) =>
            {
                tour.TourHotels?.ToList().ForEach((tourHotel) =>
                {
                    tourHotel.TourHotelRoomTypes?.ToList().ForEach((hotelRoomType) =>
                    {
                        customersInHotels += (hotelRoomType.Persons);
                    });
                });
            });

            return Math.Max(reservation.Adults + reservation.Childs + reservation.Infants, customersInHotels) - reservation.Customers.Count;
        }

        public static void CreateExternalId(Reservation reservation)
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

        public static void RemoveUnselectedHotels(Reservation reservation)
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

        public static void RemoveUnselectedOptionals(Reservation reservation)
        {
            reservation.Tours.ToList().ForEach((tour) =>
            {
                tour.TourOptionals.RemoveItems(t => t.Selected == false);
            });
        }
    }
}
