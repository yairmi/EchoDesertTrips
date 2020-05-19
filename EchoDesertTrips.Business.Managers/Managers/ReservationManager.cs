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
using Core.Common.Core;
using System.Threading.Tasks;
using EchoDesertTrips.Business.Common;
using System.Configuration;

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
        private const int POOL_SIZE = 16;
        private static readonly Dictionary<int, object> _lockers = new Dictionary<int, object>();
        private static object GetLocker(int id)
        {
            lock (_lockers)
            {
                if (!_lockers.ContainsKey(id))
                    _lockers.Add(id, new object());
                return _lockers[id];
            }
        }

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
        public Reservation DeleteReservation(int reservationId)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IReservationRepository reservationRepository = _DataRepositoryFactory.GetDataRepository<IReservationRepository>();
                lock (GetLocker(reservationId % POOL_SIZE))
                {
                    var reservation = reservationRepository.Get(reservationId);
                    if (reservation != null && reservation.Lock == false)
                    {
                        reservationRepository.RemoveReservation(reservationId);
                    }
                    return reservationRepository.Get(reservationId);
                }
            });
        }

        public Reservation[] GetAllReservations()
        {
            return null;
        }

        [OperationBehavior(TransactionScopeRequired = true)]
        public Reservation UpdateReservation(Reservation reservation)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                var res = new Reservation();

                IReservationRepository reservationRepository =
                    _DataRepositoryFactory.GetDataRepository<IReservationRepository>();

                if (reservation.ReservationId == 0) //Add new 
                {
                    res = reservationRepository.AddReservation(reservation);
                }
                else
                {
                    //var mappedReservation = AutoMapperUtil.Map<Reservation, Reservation>(reservation);
                    res = reservationRepository.UpdateReservation(reservation);
                }

                return res;
            });
        }

        [OperationBehavior(TransactionScopeRequired = true)]
        public Reservation EditReservation(int ReservationID, Operator o)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                lock (GetLocker(ReservationID % POOL_SIZE))
                {
                    bool bCanEdit = false;

                    IReservationRepository reservationRepository = _DataRepositoryFactory.GetDataRepository<IReservationRepository>();
                    var reservation = reservationRepository.Get(ReservationID);

                    int nLockTimeout = 30;
                    bool bResult = Int32.TryParse(ConfigurationManager.AppSettings["LockTimeout"], out nLockTimeout);

                    if (reservation.Lock && ((reservation.LockTime.AddMinutes(nLockTimeout) < DateTime.Now) || (reservation.LockedById == o.OperatorId)))
                    {
                        //Record is locked but time elapsed. 
                        //The lock is not released since the new request grab it.
                        bCanEdit = true;
                        if (reservation.LockedById != o.OperatorId)
                        {
                            reservation.LockTime = DateTime.Now;
                            reservation.LockedById = o.OperatorId;
                            reservation = reservationRepository.Update(reservation);
                        }
                    }
                    else
                    if (reservation.Lock == false)
                    {
                        bCanEdit = true;
                        reservation.Lock = true;
                        reservation.LockTime = DateTime.Now;
                        reservation.LockedById = o.OperatorId;
                        reservation = reservationRepository.Update(reservation);
                    }
                    reservation.Lock = !(bCanEdit == true);
                    return reservation;
                }
            });
        }

        public void UnLock(int ReservationID)
        {
            ExecuteFaultHandledOperation(() =>
            {
                var reservationRepository = _DataRepositoryFactory.GetDataRepository<IReservationRepository>();
                var reservation = reservationRepository.Get(ReservationID);
                if (reservation != null)
                {
                    reservation.Lock = false;
                    reservation.LockedById = -1;
                    reservationRepository.Update(reservation);
                }
            });
        }

        public ReservationDTO[] GetReservationsForDayRange(DateTime DayFrom, DateTime DayTo)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                var reservationRepository = _DataRepositoryFactory.GetDataRepository<IReservationRepository>();
                return reservationRepository.GetReservationsForDayRange(DayFrom, DayTo);
            });
        }

        public async Task<ReservationDTO[]> GetReservationsForDayRangeAsynchronous(DateTime DayFrom, DateTime DayTo)
        {
            return await ExecuteFaultHandledOperation(async () =>
            {
                var reservationRepository = _DataRepositoryFactory.GetDataRepository<IReservationRepository>();
                var task = Task<ReservationDTO[]>.Factory.StartNew(() =>
                {
                    var reservations = reservationRepository.GetReservationsForDayRange(DayFrom, DayTo);
                    return reservations;
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
                    log.Error($"No reservation found for GroupId: {GroupId}");
                    NotFoundException ex = new NotFoundException($"No reservation found for GroupId '{GroupId}'");

                    throw new FaultException<NotFoundException>(ex, ex.Message);
                }
                return reservations;
            });
        }

        public async Task<ReservationDTO[]> GetReservationsByIdsAsynchronous(List<int> idList)
        {
            return await ExecuteFaultHandledOperation(async () =>
            {
                var reservationRepository = _DataRepositoryFactory.GetDataRepository<IReservationRepository>();
                var task = Task<ReservationDTO[]>.Factory.StartNew(() =>
                {
                    IReservationEngine reservationEngine = _BusinessEngineFactory.GetBusinessEngine<IReservationEngine>();
                    return reservationEngine.GetReservationsByIds(idList);
                });
                return await task.ConfigureAwait(false);
            });
        }

        public async Task<Customer[]> GetCustomersByReservationGroupIdAsynchronous(int GroupId)
        {
            return await ExecuteFaultHandledOperation(async () =>
            {
                var reservationRepository = _DataRepositoryFactory.GetDataRepository<IReservationRepository>();
                var task = Task<Customer[]>.Factory.StartNew(() =>
                {
                    return  reservationRepository.GetCustomersByReservationGroupId(GroupId);
                });
                return await task.ConfigureAwait(false);
            });
        }
    }
}
