using EchoDesertTrips.Business.Entities;
using EchoDesertTrips.Data.Contracts.DTOs;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Data.Entity;
using EchoDesertTrips.Business.Contracts;
using System.Data.Entity.Infrastructure;

namespace EchoDesertTrips.Data.Data_Repositories
{
    [Export(typeof(IReservationRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReservationRepository : DataRepositoryBase<Reservation>, IReservationRepository
    {
        protected override Reservation AddEntity(EchoDesertTripsContext entityContext, Reservation entity)
        {
            return null;
        }

        protected override Reservation UpdateEntity(EchoDesertTripsContext entityContext, Reservation entity)
        {
            return null;
        }

        protected override IEnumerable<Reservation> GetEntities(EchoDesertTripsContext entityContext)
        {
            var reservations = (from e in entityContext.ReservationSet select e)
                .Include(a => a.Agency.Agents)
                .Include(a => a.Agent)
                .Include(o => o.Customers)
                .Include(o => o.Operator)
                .Include(o => o.Group)
                .Include(o => o.Tours.Select(t => t.TourType))
                .Include(o => o.Tours.Select(t => t.SubTours))
                .Include(o => o.Tours.Select(t => t.TourOptionals.Select(k => k.Optional)))
                .Include(o => o.Tours.Select(th => th.TourHotels
                .Select(throomTypes => throomTypes.TourHotelRoomTypes
                .Select(hotelRoomType => hotelRoomType.HotelRoomType.RoomType))));
            var hotels = (from e in entityContext.HotelSet select e).ToList();
            foreach (var reservation in reservations)
                updateHotel(entityContext, reservation, hotels);

            return reservations;
        }

        protected override IEnumerable<Reservation> GetEntities(EchoDesertTripsContext entityContext, int id)
        {
            return null;

        }

        protected override Reservation GetEntity(EchoDesertTripsContext entityContext, int id)
        {
            var reservation = (from e in entityContext.ReservationSet where e.ReservationId == id select e)
                .Include(a => a.Agency.Agents)
                .Include(a => a.Agent)
                .Include(o => o.Customers)
                .Include(o => o.Operator)
                .Include(o => o.Group)
                .Include(o => o.Tours.Select(t => t.TourType))
                .Include(o => o.Tours.Select(t => t.SubTours))
                .Include(o => o.Tours.Select(t => t.TourOptionals.Select(k => k.Optional)))
                .Include(o => o.Tours.Select(th => th.TourHotels
                .Select(throomTypes => throomTypes.TourHotelRoomTypes
                .Select(hotelRoomType => hotelRoomType.HotelRoomType.RoomType))))
                .FirstOrDefault();
            var hotels = (from e in entityContext.HotelSet select e).ToList();
            updateHotel(entityContext, reservation, hotels);
            return reservation;
        }

        public IEnumerable<Reservation> GetReservationHistoryByCustomer(int CustomerId)
        {
            using (EchoDesertTripsContext entityContext = new EchoDesertTripsContext())
            {
                var query = from e in entityContext.ReservationSet
                            where e.Customers.Any(c => c.CustomerId == CustomerId)
                            select e;

                return query.ToList().ToArray();
            }
        }

        public IEnumerable<Reservation> GetReservationByEndDate(DateTime date)
        {
             return null;
        }

        public IEnumerable<CustomerOrderInfo> GetCustomerOrderInfo()
        {
            using (EchoDesertTripsContext entityContext = new EchoDesertTripsContext())
            {
                 return null;
            }
        }

        public IEnumerable<Reservation> GetReservationsForDayRange(DateTime DayFrom, DateTime DayTo)
        {
            using (EchoDesertTripsContext entityContext = new EchoDesertTripsContext())
            {
                var reservations = (from e in entityContext.ReservationSet
                                    where e.Tours.Any(
                                       t => t.StartDate >= DayFrom
                                       && t.StartDate <= DayTo)
                                    select e)
                            .Include(a => a.Agency.Agents)
                            .Include(a => a.Agent)
                            .Include(o => o.Customers)
                            .Include(o => o.Operator)
                            .Include(o => o.Group)
                            .Include(o => o.Tours.Select(t => t.TourType))
                            .Include(o => o.Tours.Select(t => t.SubTours))
                            .Include(o => o.Tours.Select(t => t.TourOptionals.Select(k => k.Optional)))
                            .Include(o => o.Tours.Select(th => th.TourHotels
                            .Select(throomTypes => throomTypes.TourHotelRoomTypes
                            .Select(hotelRoomType => hotelRoomType.HotelRoomType.RoomType))));
                var hotels = (from e in entityContext.HotelSet select e).ToList();
                foreach (var reservation in reservations)
                    updateHotel(entityContext, reservation, hotels);
                return reservations.ToList().ToArray();
            }
        }

        public void RemoveReservation(int reservationId)
        {
            using (EchoDesertTripsContext entityContext = new EchoDesertTripsContext())
            {
                var reservation = (from r in entityContext.ReservationSet
                                   where r.ReservationId == reservationId select r)
                                   .Include(r => r.Customers)
                                   .Include(t => t.Tours.Select(o => o.TourOptionals))
                                   .Include(t => t.Tours.Select(o => o.TourHotels))
                                   .FirstOrDefault();
                var customer = reservation.Customers.FirstOrDefault();
                while (customer != null)
                {
                    entityContext.CustomerSet.Remove(customer);
                    customer = reservation.Customers.FirstOrDefault();
                }

                var tour = reservation.Tours.FirstOrDefault();
                while (tour != null)
                {
                    if (tour.TourOptionals != null)
                    {
                        var tourOptional = tour.TourOptionals.FirstOrDefault();
                        while (tourOptional != null)
                        {
                            entityContext.TourOptionalSet.Remove(tourOptional);
                            tourOptional = tour.TourOptionals.FirstOrDefault();
                        }
                    }

                    if (tour.TourHotels != null)
                    {
                        var tourHotel = tour.TourHotels.FirstOrDefault();
                        while (tourHotel != null)
                        {
                            if (tourHotel.TourHotelRoomTypes != null)
                            {
                                var tourHotelRoomType = tourHotel.TourHotelRoomTypes.FirstOrDefault();
                                while (tourHotelRoomType != null)
                                {
                                    entityContext.TourHotelRoomTypesSet.Remove(tourHotelRoomType);
                                    tourHotelRoomType = tourHotel.TourHotelRoomTypes.FirstOrDefault();
                                }
                            }
                            entityContext.TourHotelSet.Remove(tourHotel);
                            tourHotel = tour.TourHotels.FirstOrDefault();
                        }
                    }

                    entityContext.TourSet.Remove(tour);
                    tour = reservation.Tours.FirstOrDefault();
                }

                entityContext.Entry(reservation).State = EntityState.Deleted;
                entityContext.SaveChanges();
            }
        }

        public ReservationData UpdateReservation(Reservation reservation)
        {
            var reservationData = new ReservationData()
            {
                ClientReservation = null,
                DbReservation = null,
                InEdit = false
            };
            using (EchoDesertTripsContext entityContext = new EchoDesertTripsContext())
            {
                ResetNavProperties(reservation);
                var exsitingReservation = (from e in entityContext.ReservationSet where e.ReservationId == reservation.ReservationId select e)
                            .Include(r => r.Customers)
                            //Check if h.TourHotels includes TourHotelsRoomTypes
                            .Include(t => t.Tours.Select(h => h.TourHotels))
                            .Include(o => o.Operator)
                            .FirstOrDefault();
                entityContext.ReservationSet.Attach(exsitingReservation);
                entityContext.Entry(exsitingReservation).CurrentValues.SetValues(reservation);
                //Handle Group
                if (reservation.Group != null)
                {
                    var existingGroup = (from e in entityContext.GroupSet where e.ExternalId == reservation.Group.ExternalId select e).FirstOrDefault();
                    if (existingGroup == null)
                    {
                        exsitingReservation.Group = null;
                        exsitingReservation.GroupID = reservation.GroupID;
                    }
                }
                //Handle Tours
                foreach (var tour in reservation.Tours)
                {
                    var exsitingTour = (from e in entityContext.TourSet where e.TourId == tour.TourId select e)
                         .FirstOrDefault();
                    if (exsitingTour != null)
                    {
                        entityContext.Entry(exsitingTour).CurrentValues.SetValues(tour);
                    }
                    else //new Tour
                    {
                        exsitingReservation.Tours.Add(tour);
                    }
                    //Sub Tours
                    if (tour.SubTours != null)
                    {
                        foreach(var subTour in tour.SubTours)
                        {
                            if (subTour.SubTourId != 0)
                            {
                                var existingSubTour = (from e in entityContext.SubTourSet
                                                       where e.SubTourId == subTour.SubTourId
                                                       select e).FirstOrDefault();
                                entityContext.Entry(existingSubTour).CurrentValues.SetValues(subTour);
                            }
                        }
                    }
                    //Tour Optionals
                    if (tour.TourOptionals != null)
                    foreach (var tourOptional in tour.TourOptionals)
                    {
                        var existingTourOptional = (from e in entityContext.TourOptionalSet
                                             where e.TourId == tourOptional.TourId 
                                             && e.OptionalId == tourOptional.OptionalId
                                             select e).FirstOrDefault();
                            if (existingTourOptional == null)  //new TourOptional
                            {
                                entityContext.TourOptionalSet.Add(tourOptional);
                            }
                            else
                                entityContext.Entry(existingTourOptional).CurrentValues.SetValues(tourOptional);
                            //entityContext.Entry(existingTourOptional).State = EntityState.Modified;
                        }

                    if (exsitingTour != null && exsitingTour.TourOptionals != null)
                        for (var i = exsitingTour.TourOptionals.Count() - 1; i >= 0; i--)
                        {
                            var exist = tour.TourOptionals.Exists(t => t.TourId == exsitingTour.TourOptionals[i].TourId
                                        && t.OptionalId == exsitingTour.TourOptionals[i].OptionalId);
                            if (!exist)
                            {
                                entityContext.Entry(exsitingTour.TourOptionals[i]).State = EntityState.Deleted;
                            }
                        }
                    if (tour.TourHotels != null)
                    {
                        tour.TourHotels.ForEach((tourHotel) =>
                        {
                            var existingTourHotel = (from e in entityContext.TourHotelSet
                                                     where e.TourHotelId == tourHotel.TourHotelId
                                                     select e).FirstOrDefault();
                            if (existingTourHotel == null)
                            {
                                //TODO: check if tourhotelroomtypes are also added
                                entityContext.TourHotelSet.Add(tourHotel);
                            }
                            else
                            //Tour Hotel Room Types
                            if (tourHotel.TourHotelRoomTypes != null)
                                foreach (var tourHotelRoomType in tourHotel.TourHotelRoomTypes)
                                {
                                    var existingTourHotelRoomType = (from e in entityContext.TourHotelRoomTypesSet
                                                                     where e.TourId == tourHotelRoomType.TourId
                                                                     && e.HotelRoomTypeId == tourHotelRoomType.HotelRoomTypeId
                                                                     select e).FirstOrDefault();
                                    if (existingTourHotelRoomType == null)  //new tourHotelRoomType
                                    {
                                        entityContext.TourHotelRoomTypesSet.Add(tourHotelRoomType);
                                    }
                                }
                        });
                    }
                    //TODO : unRemark and add TourHotels to the code below
                    /*if (exsitingTour != null && exsitingTour.TourHotelRoomTypes != null)
                        for (var i = exsitingTour.TourHotelRoomTypes.Count() - 1; i >= 0; i--)
                        {
                            var exist = tour.TourHotelRoomTypes.Exists(t => t.TourId == exsitingTour.TourHotelRoomTypes[i].TourId
                                        && t.HotelRoomTypeId == exsitingTour.TourHotelRoomTypes[i].HotelRoomTypeId);
                            if (!exist)
                            {
                                entityContext.Entry(exsitingTour.TourHotelRoomTypes[i]).State = EntityState.Deleted;
                            }
                        }*/
                }
                //Delete Tours that are not exist in reservation (The most updated Reservation)
                for (var i = exsitingReservation.Tours.Count() - 1; i >= 0; i--)
                {
                    var exist = reservation.Tours.Exists(n => n.TourId == exsitingReservation.Tours[i].TourId);
                    if (!exist)
                    {
                        entityContext.Entry(exsitingReservation.Tours[i]).State = EntityState.Deleted;
                    }
                }
                //Handle Customers
                foreach (var customer in reservation.Customers)
                {
                    var exsitingCustomer = (from e in entityContext.CustomerSet where e.CustomerId == customer.CustomerId select e)
                        //.Include(n => n.Nationality)
                        .FirstOrDefault();
                    if (exsitingCustomer != null)
                    {
                        entityContext.Entry(exsitingCustomer).CurrentValues.SetValues(customer);
                    }
                    else //new customer
                    {
                        exsitingReservation.Customers.Add(customer);
                    }
                }
                //Delete Customers that are not exist in reservation (The most updated reservation)
                for (var i = exsitingReservation.Customers.Count() - 1; i >= 0; i--)
                {
                    var exist = reservation.Customers.Exists(n => n.CustomerId == exsitingReservation.Customers[i].CustomerId);
                    if (!exist)
                    {
                        entityContext.Entry(exsitingReservation.Customers[i]).State = EntityState.Deleted;
                    }
                }
                entityContext.Entry(exsitingReservation).OriginalValues["RowVersion"] = reservation.RowVersion;
                exsitingReservation.UpdateTime = DateTime.Now;

                try
                {
                    entityContext.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    reservationData.InEdit = true;
                }
            }
            reservationData.DbReservation = Get(reservation.ReservationId);
            return reservationData;
        }

        public Reservation AddReservation(Reservation reservation)
        {
            using (EchoDesertTripsContext entityContext = new EchoDesertTripsContext())
            {
                ResetNavProperties(reservation);
                //Handle Group
                if (reservation.Group != null)
                {
                    var existingGroup = (from e in entityContext.GroupSet where e.ExternalId == reservation.Group.ExternalId select e).FirstOrDefault();
                    if (existingGroup != null)
                    {
                        reservation.Group = null;
                        reservation.GroupID = existingGroup.GroupId;
                    }
                }
                foreach (var tour in reservation.Tours)
                {
                    entityContext.TourSet.Add(tour);
                    if (tour.TourOptionals != null)
                        foreach (var tourOptional in tour.TourOptionals)
                        {
                            {
                                entityContext.TourOptionalSet.Add(tourOptional);
                            }
                        }
                    if (tour.TourHotels != null)
                    {
                        tour.TourHotels.ForEach((tourHotel) =>
                        {
                            if (tourHotel.TourHotelRoomTypes != null)
                                tourHotel.TourHotelRoomTypes.ForEach((tourHotelRoomType) =>
                                {
                                    {
                                        entityContext.TourHotelRoomTypesSet.Add(tourHotelRoomType);
                                    }
                                });
                            entityContext.TourHotelSet.Add(tourHotel);
                        });
                    }
                }

                //Handle Customers
                foreach (var customer in reservation.Customers)
                {
                    entityContext.Entry(customer).State = EntityState.Added;
                }
                entityContext.Entry(reservation).State = EntityState.Added;
                reservation.UpdateTime = DateTime.Now;
                reservation.CreationTime = DateTime.Now;
                entityContext.SaveChanges();
            }
            var existingReservation = Get(reservation.ReservationId);
            return existingReservation;
        }

        private void ResetNavProperties(Reservation reservation)
        {
            reservation.Tours.ForEach((tour) =>
            {
                tour.TourType = null;
                tour.TourOptionals.ForEach((tourOptional) => { tourOptional.Optional = null; });
                tour.TourHotels.ForEach((tourHotel) =>
                {
                    tourHotel.TourHotelRoomTypes.ForEach((tourHotelRoomType) => { tourHotelRoomType.HotelRoomType = null; });
                });
            });

            reservation.Operator = null;
            reservation.Agency = null;
            reservation.Agent = null;
        }

        private void updateHotel(EchoDesertTripsContext entityContext, Reservation reservation, List<Hotel> hotels)
        {
            reservation.Tours.ForEach((tour) =>
            {
                if (tour.TourHotels != null)
                {
                    tour.TourHotels.ForEach((tourHotel) =>
                    {
                        if (tourHotel.TourHotelRoomTypes != null ||
                            tourHotel.TourHotelRoomTypes.Capacity > 0)
                            tourHotel.TourHotelRoomTypes.ForEach((tourHotelRoomType) =>
                            {
                                var hotel = hotels.Where(h => h.HotelId == tourHotelRoomType.HotelRoomType.HotelId)
                                .FirstOrDefault();
                                tourHotelRoomType.HotelRoomType.HotelName = (hotel == null) ? string.Empty : hotel.HotelName;
                            });
                    });
                }
            });

        }
    }
}
