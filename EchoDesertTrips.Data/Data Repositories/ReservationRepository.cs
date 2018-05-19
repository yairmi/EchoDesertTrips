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
                UpdateHotel(entityContext, reservation, hotels);

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
            if (reservation == null) return null;
            UpdateHotel(entityContext, reservation, hotels);
            return reservation;

        }

        public IEnumerable<Reservation> GetReservationsByGroupId(int groupId)
        {
            using (EchoDesertTripsContext entityContext = new EchoDesertTripsContext())
            {
                var reservations = (from e in entityContext.ReservationSet where e.GroupID == groupId select e)
                .Include(a => a.Agency.Agents)
                .Include(a => a.Agent)
                .Include(o => o.Customers)
                .Include(o => o.Operator)
                .Include(o => o.Group)
                .Include(o => o.Tours.Select(t => t.TourType.TourTypeDescriptions))
                .Include(o => o.Tours.Select(t => t.SubTours))
                .Include(o => o.Tours.Select(t => t.TourOptionals.Select(k => k.Optional)))
                .Include(o => o.Tours.Select(th => th.TourHotels
                .Select(throomTypes => throomTypes.TourHotelRoomTypes
                .Select(hotelRoomType => hotelRoomType.HotelRoomType.RoomType))));
                var hotels = (from e in entityContext.HotelSet select e).ToList();
                foreach (var reservation in reservations)
                    UpdateHotel(entityContext, reservation, hotels);
                return reservations.ToArray();
            }
        }

        public IEnumerable<Reservation> GetReservationHistoryByCustomer(int customerId)
        {
            using (EchoDesertTripsContext entityContext = new EchoDesertTripsContext())
            {
                var query = from e in entityContext.ReservationSet
                            where e.Customers.Any(c => c.CustomerId == customerId)
                            select e;

                return query.ToArray();
            }
        }

        public IEnumerable<Reservation> GetReservationByEndDate(DateTime date)
        {
            return null;
        }

        public IEnumerable<CustomerOrderInfo> GetCustomerOrderInfo()
        {
            using (var entityContext = new EchoDesertTripsContext())
            {
                return null;
            }
        }

        public IEnumerable<Reservation> GetReservationsForDayRange(DateTime dayFrom, DateTime dayTo)
        {
            using (var entityContext = new EchoDesertTripsContext())
            {
                var reservations = (from e in entityContext.ReservationSet
                                    where e.Tours.Any(
                                       t => t.StartDate >= dayFrom
                                       && t.StartDate <= dayTo)
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
                    UpdateHotel(entityContext, reservation, hotels);
                return reservations.ToArray();
            }
        }

        public void RemoveReservation(int reservationId)
        {
            using (var entityContext = new EchoDesertTripsContext())
            {
                var existingReservation = (from r in entityContext.ReservationSet
                                   where r.ReservationId == reservationId
                                   select r)
                                   .Include(r => r.Customers)
                                   .Include(t => t.Tours.Select(o => o.TourOptionals))
                                   .Include(t => t.Tours.Select(s => s.SubTours))
                                   .Include(t => t.Tours.Select(o => o.TourHotels.Select(th => th.TourHotelRoomTypes)))
                                   .FirstOrDefault();
                if (existingReservation != null)
                {
                    var customer = existingReservation.Customers.FirstOrDefault();
                    while (customer != null)
                    {
                        entityContext.CustomerSet.Remove(customer);
                        customer = existingReservation.Customers.FirstOrDefault();
                    }

                    var tour = existingReservation.Tours.FirstOrDefault();
                    while (tour != null)
                    {
                        RemoveTour(entityContext, tour);
                        tour = existingReservation.Tours.FirstOrDefault();
                    }

                    entityContext.Entry(existingReservation).State = EntityState.Deleted;
                    entityContext.SaveChanges();
                }
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
            using (var entityContext = new EchoDesertTripsContext())
            {
                try
                { 
                    ResetNavProperties(reservation);
                    var exsitingReservation = (from e in entityContext.ReservationSet where e.ReservationId == reservation.ReservationId select e)
                                .Include(r => r.Customers)
                                .Include(t => t.Tours.Select(h => h.TourHotels.Select(th => th.TourHotelRoomTypes)))
                                .Include(t => t.Tours.Select(o => o.TourOptionals))
                                .Include(t => t.Tours.Select(s => s.SubTours))
                                .Include(o => o.Operator)
                                .FirstOrDefault();
                    if (exsitingReservation != null)
                    {
                        entityContext.ReservationSet.Attach(exsitingReservation);
                        entityContext.Entry(exsitingReservation).CurrentValues.SetValues(reservation);
                        //Handle Group
                        if (reservation.Group != null)
                        {
                            var existingGroup =
                                (from e in entityContext.GroupSet
                                    where e.ExternalId == reservation.Group.ExternalId
                                    select e).FirstOrDefault();
                            if (existingGroup == null)
                            {
                                exsitingReservation.Group = null;
                                exsitingReservation.GroupID = reservation.GroupID;
                            }
                            else
                            {
                                existingGroup.Updated = true;
                            }
                        }

                        //Handle Tours
                        foreach (var tour in reservation.Tours)
                        {
                            var exsitingTour = (from e in entityContext.TourSet where e.TourId == tour.TourId select e)
                                .Include(t => t.TourHotels.Select(h => h.TourHotelRoomTypes))
                                .Include(t => t.TourOptionals)
                                .Include(t => t.SubTours)
                                .Include(t => t.TourHotels.Select(h => h.TourHotelRoomTypes))
                                .FirstOrDefault();
                            if (exsitingTour != null)
                            {
                                entityContext.Entry(exsitingTour).CurrentValues.SetValues(tour);
                            }
                            else //new Tour
                            {
                                exsitingReservation.Tours.Add(tour);
                                continue;
                            }

                            //Sub Tours
                            if (tour.SubTours != null)
                            {
                                foreach (var subTour in tour.SubTours)
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
                            //Add
                            if (tour.TourOptionals != null)
                            {
                                foreach (var tourOptional in tour.TourOptionals)
                                {
                                    var existingTourOptional = (from e in entityContext.TourOptionalSet
                                        where e.TourId == tourOptional.TourId
                                              && e.OptionalId == tourOptional.OptionalId
                                        select e).FirstOrDefault();
                                    if (existingTourOptional == null) //new TourOptional
                                    {
                                        entityContext.TourOptionalSet.Add(tourOptional);
                                    }
                                    else
                                        entityContext.Entry(existingTourOptional).CurrentValues.SetValues(tourOptional);
                                }
                            }

                            //Delete
                            if (exsitingTour.TourOptionals != null)
                            {
                                for (var i = exsitingTour.TourOptionals.Count - 1; i >= 0; i--)
                                {
                                    var exist = tour.TourOptionals.Exists(t =>
                                        t.TourId == exsitingTour.TourOptionals[i].TourId
                                        && t.OptionalId == exsitingTour.TourOptionals[i].OptionalId);
                                    if (!exist)
                                    {
                                        entityContext.Entry(exsitingTour.TourOptionals[i]).State = EntityState.Deleted;
                                    }
                                }
                            }

                            //Tour Hotels
                            if (tour.TourHotels != null)
                            {
                                tour.TourHotels.ForEach((tourHotel) =>
                                {
                                    var existingTourHotel = (from e in entityContext.TourHotelSet
                                        where e.TourHotelId == tourHotel.TourHotelId
                                        select e).Include(t => t.TourHotelRoomTypes).FirstOrDefault();
                                    if (existingTourHotel == null)
                                    {
                                        exsitingTour.TourHotels.Add(tourHotel);
                                    }
                                    else
                                        //Tour Hotel Room Types
                                    if (tourHotel.TourHotelRoomTypes != null)
                                    {
                                        tourHotel.TourHotelRoomTypes.ForEach((tourHotelRoomType) =>
                                        {
                                            var existingTourHotelRoomType =
                                                (from e in entityContext.TourHotelRoomTypesSet
                                                    where e.TourHotelRoomTypeId == tourHotelRoomType.TourHotelRoomTypeId
                                                    select e).FirstOrDefault();
                                            if (existingTourHotelRoomType == null) //new tourHotelRoomType
                                            {
                                                existingTourHotel.TourHotelRoomTypes.Add(tourHotelRoomType);
                                            }
                                            else
                                            {
                                                entityContext.Entry(existingTourHotelRoomType).CurrentValues
                                                    .SetValues(tourHotelRoomType);
                                            }
                                        });
                                    }
                                });
                            }

                            if (exsitingTour != null)
                            {
                                for (var j = exsitingTour.TourHotels.Count() - 1; j >= 0; j--)
                                {
                                    var tourHotel = tour.TourHotels.FirstOrDefault(t =>
                                        t.TourHotelId == exsitingTour.TourHotels[j].TourHotelId);
                                    if (tourHotel != null)
                                    {
                                        for (var i = exsitingTour.TourHotels[j].TourHotelRoomTypes.Count - 1;
                                            i >= 0;
                                            i--)
                                        {
                                            var exist = tourHotel.TourHotelRoomTypes.FirstOrDefault(t =>
                                                t.TourHotelRoomTypeId == exsitingTour.TourHotels[j]
                                                    .TourHotelRoomTypes[i]
                                                    .TourHotelRoomTypeId);
                                            if (exist == null)
                                            {
                                                entityContext.Entry(exsitingTour.TourHotels[j].TourHotelRoomTypes[i])
                                                        .State
                                                    = EntityState.Deleted;
                                            }
                                        }

                                        if (!exsitingTour.TourHotels[j].TourHotelRoomTypes.Any())
                                            entityContext.Entry(exsitingTour.TourHotels[j]).State = EntityState.Deleted;
                                    }
                                    else
                                    {
                                        for (var i = exsitingTour.TourHotels[j].TourHotelRoomTypes.Count - 1;
                                            i >= 0;
                                            i--)
                                        {
                                            entityContext.Entry(exsitingTour.TourHotels[j].TourHotelRoomTypes[i])
                                                    .State =
                                                EntityState.Deleted;
                                        }

                                        entityContext.Entry(exsitingTour.TourHotels[j]).State = EntityState.Deleted;
                                    }
                                }
                            }
                        }

                        //Delete Tours that are not exist in reservation (The most updated Reservation)
                        for (var i = exsitingReservation.Tours.Count - 1; i >= 0; i--)
                        {
                            var exist = reservation.Tours.Exists(n => n.TourId == exsitingReservation.Tours[i].TourId);
                            if (!exist)
                            {
                                RemoveTour(entityContext, exsitingReservation.Tours[i]);
                                //entityContext.Entry(exsitingReservation.Tours[i]).State = EntityState.Deleted;
                            }
                        }

                        //Handle Customers
                        foreach (var customer in reservation.Customers)
                        {
                            var exsitingCustomer = (from e in entityContext.CustomerSet
                                    where e.CustomerId == customer.CustomerId
                                    select e)
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
                        for (var i = exsitingReservation.Customers.Count - 1; i >= 0; i--)
                        {
                            var exist = reservation.Customers.Exists(n =>
                                n.CustomerId == exsitingReservation.Customers[i].CustomerId);
                            if (!exist)
                            {
                                entityContext.Entry(exsitingReservation.Customers[i]).State = EntityState.Deleted;
                            }
                        }

                        entityContext.Entry(exsitingReservation).OriginalValues["RowVersion"] = reservation.RowVersion;
                        exsitingReservation.UpdateTime = DateTime.Now;
                    }
                    else
                        throw new DbUpdateConcurrencyException();

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
                    tourHotel.Hotel = null;
                    tourHotel.TourHotelRoomTypes.ForEach((tourHotelRoomType) => { tourHotelRoomType.HotelRoomType = null; });
                });
            });

            reservation.Operator = null;
            reservation.Agency = null;
            reservation.Agent = null;
        }

        private void UpdateHotel(EchoDesertTripsContext entityContext, Reservation reservation, List<Hotel> hotels)
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
                                var hotel = hotels
                                .FirstOrDefault(h => h.HotelId == tourHotelRoomType.HotelRoomType.HotelId);
                                tourHotelRoomType.HotelRoomType.HotelName = (hotel == null) ? string.Empty : hotel.HotelName;
                            });
                    });
                }
            });

        }

        private void RemoveTour(EchoDesertTripsContext entityContext, Tour tour)
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

            if (tour.SubTours != null)
            {
                var subTour = tour.SubTours.FirstOrDefault();
                while (subTour != null)
                {
                    entityContext.SubTourSet.Remove(subTour);
                    subTour = tour.SubTours.FirstOrDefault();
                }
            }

            entityContext.TourSet.Remove(tour);
        }
    }
}
