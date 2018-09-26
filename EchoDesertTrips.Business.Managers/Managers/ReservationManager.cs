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
                var reservationData = new List<Reservation>();
                IEnumerable<Reservation> reservations = reservationRepository.Get();
                foreach (var reservation in reservations)
                {
                    reservationData.Add(reservation);
                }
                return reservationData.ToArray();
            });
        }

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

        public Reservation[] GetReservationsForDayRange(DateTime DayFrom, DateTime DayTo)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                log.Debug("ReservationManager: GetReservationsForDayRange Start");
                var reservationRepository = _DataRepositoryFactory.GetDataRepository<IReservationRepository>();
                var reservations = reservationRepository.GetReservationsForDayRange(DayFrom, DayTo);
                log.Debug("ReservationManager: GetReservationsForDayRange End");
                //foreach (var reservation in reservations)
                //{
                //    if (reservation.Customers.Count() > 1)
                //    {
                //        var customer = reservation.Customers[0];
                //        reservation.Customers.Clear();
                //        reservation.Customers.Add(customer);
                //    }
                //}
                //log.Debug("ReservationManager: clear customers end");

                /////Conversion check start
                /*log.Debug("ReservationManager: Conversion check start");
                var reservationViews = new List<ReservationView>();
                foreach (var reservation in reservations)
                {
                        var reservationView = new ReservationView()
                        {
                            ReservationId = reservation.ReservationId,
                            NumberOfPaxs = reservation.Customers.Count(),
                            PaxFullName = reservation.Customers.Count() > 0 ? string.Format("{0} {1}", reservation.Customers[0].LastName, reservation.Customers[0].FirstName) : string.Empty,
                            PickUpTime = reservation.PickUpTime,
                            PickUpAddress = reservation.Tours[0].PickupAddress,
                            Phone = reservation.Customers.Count() > 0 ? reservation.Customers[0].Phone1 : string.Empty,
                            HotelName = reservation.Tours[0].TourHotels.Count > 0 ? reservation.Tours[0].TourHotels[0].Hotel.HotelName : string.Empty,
                            AgencyAndAgentName = reservation.Agency != null ? string.Format("{0} {1}", reservation.Agency.AgencyName, reservation.Agent.FullName) : string.Empty,
                            //public double TotalPrice { get; set; }
                            AdvancePayment = reservation.AdvancePayment,
                            //public double Balance { get; set; }
                            Comments = reservation.Comments,
                            Messages = reservation.Messages
                        };
                        reservationViews.Add(reservationView);
                    }
                    log.Debug("ReservationManager: Conversion check end");*/
                return reservations;
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
                    return reservationRepository.GetReservationsForDayRange(DayFrom, DayTo);
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
                return reservations;
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
