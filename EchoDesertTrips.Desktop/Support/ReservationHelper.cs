using Core.Common.Utils;
using static Core.Common.Core.Const;
using EchoDesertTrips.Client.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EchoDesertTrips.Desktop.Support
{
    public class ReservationHelper
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static decimal CalculateReservationTotalPrice(Reservation reservation)
        {
            try
            {
                decimal totalPrice = 0;

                if (reservation.Tours != null)
                {
                    var adultPrices = new Dictionary<int, decimal>();
                    var childPrices = new Dictionary<int, decimal>();
                    var infantPrices = new Dictionary<int, decimal>();

                    foreach (var tour in reservation.Tours)
                    {
                        if (tour.TourTypePrice != null && tour.TourTypePrice != string.Empty)
                        {
                            adultPrices.Clear();
                            childPrices.Clear();
                            infantPrices.Clear();
                            TimeSpan dt2 = tour.EndDate.Subtract(tour.StartDate);

                            string[] separators1 = { ":" };

                            string[] tourTypePrices = SimpleSplitter.Split(tour.TourTypePrice, separators1);
                            string AdultPrices = "";
                            string ChildPrices = "";
                            string InfantPrices = "";

                            if (tourTypePrices.Count() > 0)
                                AdultPrices = tourTypePrices[0];
                            if (tourTypePrices.Count() > 1)
                                ChildPrices = tourTypePrices[1];
                            if (tourTypePrices.Count() > 2)
                                InfantPrices = tourTypePrices[2];

                            AddPersonTypePrices(AdultPrices, adultPrices, reservation, ref totalPrice);
                            AddPersonTypePrices(ChildPrices, childPrices, reservation, ref totalPrice);
                            AddPersonTypePrices(InfantPrices, infantPrices, reservation, ref totalPrice);
                        }
                        else
                        {
                            //TODO. log message
                        }

                        foreach (var tourOptional in tour.TourOptionals)
                        {
                            totalPrice += tourOptional.PriceInclusive == true ? tourOptional.PriceInclusiveValue :
                                tourOptional.PricePerPerson * reservation.ActualNumberOfCustomers;
                        }
                        tour.TourHotels.ToList().ForEach((tourHotel) =>
                        {
                            tourHotel.TourHotelRoomTypes.ToList().ForEach((tourHotelRoomType) =>
                            {

                                foreach (var daysRange in tourHotelRoomType.HotelRoomType.HotelRoomTypeDaysRanges)
                                {
                                    int days = GetDaysInRange(tourHotel, daysRange);
                                    totalPrice += tourHotelRoomType.Persons * daysRange.PricePerPerson * days;
                                }
                            });
                        });
                    }
                }
                return totalPrice;
            }
            catch (Exception ex)
            {
                log.Error($"Failed to calculate reservation total price. ReservationId = {reservation.ReservationId} ", ex);
            }
            return 0;
        }

        private static void AddPersonTypePrices(string PersonTypePrices, Dictionary<int, decimal> PersonTypePricesDictionay, Reservation reservation, ref decimal totalPrice)
        {
            string[] separators = { ";" };
            string[] pairs;
            if (PersonTypePrices != string.Empty)
            {
                pairs = SimpleSplitter.Split(PersonTypePrices, separators);
                foreach (var pair in pairs)
                {
                    var prices = new Prices();
                    prices.Deserialize(pair);
                    PersonTypePricesDictionay.Add(prices.Persons, prices.Price);
                }
            }

            decimal value = 0;
            if (PersonTypePricesDictionay.TryGetValue(reservation.Adults, out value))
            {
                totalPrice += value * reservation.Adults;
            }
            else
            {
                if (PersonTypePricesDictionay.Count > 0)
                    totalPrice += PersonTypePricesDictionay[PersonTypePricesDictionay.Keys.Max()] * reservation.Adults;
            }
        }

        private static int GetDaysInRange(TourHotel tourHotel, HotelRoomTypeDaysRange hotelRoomTypeDaysRange)
        {
            int result = 0;
            for (int i = 0; i < (int)(tourHotel.HotelEndDay.Date - tourHotel.HotelStartDay.Date).TotalDays + 1; i++)
            {
                var date = tourHotel.HotelStartDay.AddDays(i);
                if (date >= hotelRoomTypeDaysRange.StartDaysRange.Date && date <= hotelRoomTypeDaysRange.EndDaysRange.Date)
                {
                    result++;
                }
            }
            return result = result > 0 ? result - 1 : 0;//The last day is not count
        }

        public static int GetCustomerLeft(Reservation reservation)
        {
            return (reservation.Adults + reservation.Childs + reservation.Infants) - reservation.Customers.Count;
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
    }
}
