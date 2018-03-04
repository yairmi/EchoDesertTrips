using EchoDesertTrips.Client.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Desktop.Support
{
    public class TourHelper
    {
        public static void RemoveUnSelectedTourOptionals(TourWrapper tour)
        {
            var tourOptionals = new List<TourOptionalWrapper>(tour.TourOptionals);
            tourOptionals.RemoveAll(t => t.Selected == false);
            tour.TourOptionals.Clear();
            tourOptionals.ForEach((tourOptional) =>
            {
                tour.TourOptionals.Add(tourOptional);
            });
        }
        //public static void SetStartDateInSubTours(TourWrapper tour)
        //{
        //    int days = 0;
        //    foreach (var subTour in tour.SubTours)
        //    {
        //        subTour.StartDate = tour.StartDate.AddDays(days++);
        //    }
        //}


    }
}
