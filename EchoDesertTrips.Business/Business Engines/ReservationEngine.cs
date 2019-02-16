using EchoDesertTrips.Business.Common;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using EchoDesertTrips.Business.Entities;
using System.Threading.Tasks;
using Core.Common.Exceptions;
using Core.Common.Contracts;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;

namespace EchoDesertTrips.Business.Business_Engines
{
    [Export(typeof(IReservationEngine))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReservationEngine : IReservationEngine
    {

        [ImportingConstructor]
        public ReservationEngine(IDataRepositoryFactory dataRepositoryFactory)
        {
            _DataRepositoryFactory = dataRepositoryFactory;
        }

        IDataRepositoryFactory _DataRepositoryFactory;

        public void PrepareReservationsForTransmition(Reservation[] Reservations)
        {
            Parallel.ForEach(Reservations, new ParallelOptions { MaxDegreeOfParallelism = 4 }, (reservation) =>
            {
                reservation.ActualNumberOfCustomers = reservation.Customers.Count;
                var customer = reservation.Customers.Count > 0 ? reservation.Customers[0] : null;
                reservation.Customers.Clear();
                if (customer != null)
                    reservation.Customers.Add(customer);
            });
        }

        public Reservation[] GetReservationsByIds(List<int> idList)
        {
                var reservationRepository = _DataRepositoryFactory.GetDataRepository<IReservationRepository>();

                var reservations = reservationRepository.GetReservationsByIds(idList);
                if (reservations == null)
                    throw new NotFoundException(string.Format("No Reservation was found"));
                return reservations;
        }
    }
}
