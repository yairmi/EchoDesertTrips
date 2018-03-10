using Core.Common.Contracts;
using Core.Common.Exceptions;
using EchoDesertTrips.Business.Contracts;
using EchoDesertTrips.Business.Entities;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using EchoDesertTrips.Data.Contracts.DTOs;
using Core.Common.Core;
using EchoDesertTrips.Business.Contracts.Data_Contracts;
using System.Data.Entity.Infrastructure;
using Core.Common.Utils;
using System.Threading.Tasks;

namespace EchoDesertTrips.Business.Managers.Managers
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple,
    ReleaseServiceInstanceOnTransactionComplete = false)]
    public class ReservationManager : ManagerBase, IOrderService
    {
        public ReservationManager()
        {
            ObjectBase.Container.SatisfyImportsOnce(this);
        }

        public ReservationManager(IDataRepositoryFactory dataRepositoryFactory)
        {
            _DataRepositoryFactory = dataRepositoryFactory;
        }

        [Import]
        private IDataRepositoryFactory _DataRepositoryFactory;

        [Import]
        public IBusinessEngineFactory _BusinessEngineFactory;

        public Reservation[] GetDeadReservations()
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IReservationRepository orderRepository = _DataRepositoryFactory.GetDataRepository<IReservationRepository>();

                IEnumerable<Reservation> reservations =
                    orderRepository.GetReservationByEndDate(DateTime.Now.AddDays(-1));

                return (reservations != null ? reservations.ToArray() : null);
            });
        }
        [OperationBehavior(TransactionScopeRequired = true)]
        public void CancelReservation(int reservationId)
        {
            ExecuteFaultHandledOperation(() =>
            {
                IReservationRepository reservationRepository = _DataRepositoryFactory.GetDataRepository<IReservationRepository>();

                Reservation reservation = reservationRepository.Get(reservationId);

                if (reservation == null)
                {
                    NotFoundException ex = new NotFoundException(string.Format("No order found for ID '{0}'", reservationId));

                    throw new FaultException<NotFoundException>(ex, ex.Message);
                }

                reservationRepository.RemoveReservation(reservation.ReservationId);
            });
        }
        public Reservation[] GetAllReservations()
        {
            return ExecuteFaultHandledOperation(() =>
            {
                var reservationRepository = _DataRepositoryFactory.GetDataRepository<IReservationRepository>();
                var tourOptionalRepository = _DataRepositoryFactory.GetDataRepository<IReservationRepository>();
                var reservationData = new List<Reservation>();
                IEnumerable<Reservation> reservations = reservationRepository.Get();
                foreach (var reservation in reservations)
                {
                    reservationData.Add(reservation);
                }
                return reservationData.ToArray();
            });
        }

        /*        [OperationBehavior(TransactionScopeRequired = true)]
                public Reservation UpdateReservation(Reservation reservation)
                {
                    return ExecuteFaultHandledOperation(() =>
                    {
                        UpdateTours(reservation.Tours);
                        var tours = new List<Tour>();
                        reservation.Tours.ForEach((item) =>
                        {
                            tours.Add((Tour)item.Clone());
                        });

                        reservation.Tours.ForEach((tour) =>
                        {
                            tour.TourOptionals = null;
                            tour.TourHotelRoomTypes = null;
                            tour.TourType = null;
                        });

                        reservation.Customers.ForEach((customer) =>
                        {
                            customer.Nationality = null;
                        });

                        IReservationRepository reservationRepository = 
                            _DataRepositoryFactory.GetDataRepository<IReservationRepository>();

                        Reservation updateEntity = null;

                        if (reservation.ReservationId == 0)
                        {
                            updateEntity = reservationRepository.Add(reservation);
                        }
                        else
                        {
                            updateEntity = reservationRepository.Update(reservation);
                        }
                        updateEntity.Tours = tours;
                        return updateEntity;
                    });
                }*/

        [OperationBehavior(TransactionScopeRequired = true)]
        public ReservationData UpdateReservation(Reservation reservation)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                ReservationData reservationData = new ReservationData()
                {
                    DbReservation = null,
                    ClientReservation = null
                };

                IReservationRepository reservationRepository =
                    _DataRepositoryFactory.GetDataRepository<IReservationRepository>();

                if (reservation.ReservationId == 0)
                {
                    reservationData.DbReservation = reservationRepository.AddReservation(reservation);
                }
                else
                {
                    var mappedReservation = AutoMapperUtil.Map<Reservation, Reservation>(reservation);
                    reservationData = reservationRepository.UpdateReservation(mappedReservation);
                    //reservationData.DbReservation = reservationRepository.Get(reservation.ReservationId);
                }

                return reservationData;
            });
        }

        //public Reservation[] GetReservationsForDay(DateTime Day)
        //{
        //    return ExecuteFaultHandledOperation(() =>
        //    {
        //        var orderRepository = _DataRepositoryFactory.GetDataRepository<IReservationRepository>();
        //        var reservationData = new List<Reservation>();
        //        IEnumerable<Reservation> reservations = orderRepository.GetReservationsByDay(Day);

        //        foreach (var r in reservations)
        //        {
        //            reservationData.Add(r);
        //        }
        //        return reservationData.ToArray();
        //    });
        //}

        public Reservation[] GetReservationsForDayRange(DateTime DayFrom, DateTime DayTo)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                var reservationRepository = _DataRepositoryFactory.GetDataRepository<IReservationRepository>();
                var reservationData = new List<Reservation>();
                IEnumerable<Reservation> reservations = reservationRepository.GetReservationsForDayRange(DayFrom, DayTo);

                foreach (var r in reservations)
                {
                    reservationData.Add(r);
                }
                return reservationData.ToArray();
            });
        }

        public void DeleteTour(Tour tour)
        {
            ExecuteFaultHandledOperation(() =>
            {
                ITourRepository tourRepository =
                    _DataRepositoryFactory.GetDataRepository<ITourRepository>();

                Tour tmpTour = tourRepository.Get(tour.TourId);

                if (tmpTour == null)
                {
                    NotFoundException ex = new NotFoundException(string.Format("No Tour found for ID '{0}'", tour.TourId));

                    throw new FaultException<NotFoundException>(ex, ex.Message);
                }

                tourRepository.RemoveTour(tour.TourId);
            });
        }

        public void DeleteCustomer(Customer customer)
        {
            ExecuteFaultHandledOperation(() =>
            {
                ICustomerRepository customerRepository =
                    _DataRepositoryFactory.GetDataRepository<ICustomerRepository>();

                Customer tmpCustomer = customerRepository.Get(customer.CustomerId);

                if (tmpCustomer == null)
                {
                    NotFoundException ex = new NotFoundException(string.Format("No customer found for ID '{0}'", customer.CustomerId));

                    throw new FaultException<NotFoundException>(ex, ex.Message);
                }

                customerRepository.Remove(customer.CustomerId);
            });
        }

        public Reservation GetReservation(int ReservationId)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IReservationRepository reservationRepository = _DataRepositoryFactory.GetDataRepository<IReservationRepository>();

                var reservation = reservationRepository.Get(ReservationId);

                if (reservation == null)
                {
                    log.Error("No reservation found for ID: " + ReservationId);
                    NotFoundException ex = new NotFoundException(string.Format("No reservation found for ID '{0}'", ReservationId));

                    throw new FaultException<NotFoundException>(ex, ex.Message);
                }

                return (reservation);
            });
        }

        public async Task<Reservation[]> GetReservationsForDayRangeAsynchronous(DateTime DayFrom, DateTime DayTo)
        {
            return await ExecuteFaultHandledOperation(async () =>
            {
                var reservationRepository = _DataRepositoryFactory.GetDataRepository<IReservationRepository>();
                var task = Task<Reservation[]>.Factory.StartNew(() =>
                {
                    var reservations = reservationRepository.GetReservationsForDayRange(DayFrom, DayTo);
                    return reservations.ToArray();
                });
                return await task.ConfigureAwait(false);
            });
        }

        public Reservation[] GetReservationsByGroupId(int GroupId)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IReservationRepository reservationRepository = _DataRepositoryFactory.GetDataRepository<IReservationRepository>();

                var reservations = reservationRepository.GetReservationsByGroupId(GroupId);

                if (reservations == null)
                {
                    log.Error("No reservation found for GroupId: " + GroupId);
                    NotFoundException ex = new NotFoundException(string.Format("No reservation found for GroupId '{0}'", GroupId));

                    throw new FaultException<NotFoundException>(ex, ex.Message);
                }

                return (reservations.ToArray());
            });
        }


        //public ReservationData EditReservation(int ReservationId)
        //{
        //    return ExecuteFaultHandledOperation(() =>
        //    {
        //        IReservationRepository reservationRepository = _DataRepositoryFactory.GetDataRepository<IReservationRepository>();
        //        ReservationData reservationData = new ReservationData();

        //        var reservation = reservationRepository.EditReservation(ReservationId);

        //        if (reservation == null)
        //        {
        //            NotFoundException ex = new NotFoundException(string.Format("No reservation found for ID '{0}'", ReservationId));

        //            throw new FaultException<NotFoundException>(ex, ex.Message);
        //        }

        //        reservationData.DbReservation = reservation;
        //        return (reservationData);
        //    });
        //}
    }
}
