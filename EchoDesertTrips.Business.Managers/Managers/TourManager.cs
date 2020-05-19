using EchoDesertTrips.Business.Contracts;
using EchoDesertTrips.Business.Entities;
using System.ServiceModel;
using Core.Common.Contracts;
using System.ComponentModel.Composition;
using Core.Common.Core;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using System.Collections.Generic;
using System.Linq;
using EchoDesertTrips.Business.Common;
using Core.Common.Exceptions;

namespace EchoDesertTrips.Business.Managers.Managers
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple,
    ReleaseServiceInstanceOnTransactionComplete = false)]
    public class TourManager : ManagerBase, ITourService
    {
        public TourManager()

        {
            ObjectBase.Container.SatisfyImportsOnce(this);
        }
        //For unit testing (Mock parameter). WCF is only interested in the default constructor.
        public TourManager(IDataRepositoryFactory DataRepositoryFactory)
        {
            _DataRepositoryFactory = DataRepositoryFactory;
        }

        //For unit testing (Mock parameter). WCF is only interested in the default constructor.
        public TourManager(IBusinessEngineFactory BusinessEngineFactory)
        {
            _BusinessEngineFactory = BusinessEngineFactory;
        }

        //For unit testing (Mock parameter). WCF is only interested in the default constructor.
        public TourManager(IDataRepositoryFactory DataRepositoryFactory, IBusinessEngineFactory BusinessEngineFactory)
        {
            _DataRepositoryFactory = DataRepositoryFactory;
            _BusinessEngineFactory = BusinessEngineFactory;
        }

        [Import]
        public IDataRepositoryFactory _DataRepositoryFactory;

        [Import]
        public IBusinessEngineFactory _BusinessEngineFactory;

        [OperationBehavior(TransactionScopeRequired = true)]
        //[PrincipalPermission(SecurityAction.Demand, Role = Security.EchoDesertTripsAdminRole)]
        public void DeleteTour(Tour tour)
        {
            ExecuteFaultHandledOperation(() =>
            {
                ITourRepository tripRepository =
                    _DataRepositoryFactory.GetDataRepository<ITourRepository>();

                tripRepository.Remove(tour.TourId);
            });
        }

        //WCF is authorize this operation only for users that are in the car rental admin windows group
        //[PrincipalPermission(SecurityAction.Demand, Role = Security.EchoDesertTripsAdminRole)]
        //WCF authorize against any other user. this one is not againt a role. It is againt a user name
        //[PrincipalPermission(SecurityAction.Demand, Name = Security.EchoDesertTripsUser)]
        public Tour[] GetAllTours()
        {
            return ExecuteFaultHandledOperation(() =>
            {
                ITourRepository tripRepository =
                     _DataRepositoryFactory.GetDataRepository<ITourRepository>();

                IEnumerable<Tour> tours = tripRepository.Get();
                return tours.ToArray();
            });
        }

        public TourType[] GetAllTourTypes()
        {
            return ExecuteFaultHandledOperation(() =>
            {
                ITourTypeRepository tripTypeRepository =
                     _DataRepositoryFactory.GetDataRepository<ITourTypeRepository>();

                IEnumerable<TourType> tripTypes = tripTypeRepository.Get();
                return tripTypes.ToArray();
            });
        }

        public Tour[] GetOrderedTours()
        {
            return null; 
        }

        //[PrincipalPermission(SecurityAction.Demand, Role = Security.EchoDesertTripsAdminRole)]
        //[PrincipalPermission(SecurityAction.Demand, Name = Security.EchoDesertTripsUser)]
        public Tour GetTour(int tourId)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                ITourRepository tripRepository =
                    _DataRepositoryFactory.GetDataRepository<ITourRepository>();

                Tour tour = tripRepository.Get(tourId);

                if (tour == null)
                {
                    NotFoundException ex = new NotFoundException($"TourId: {tourId} was not foudn");
                    //Wrap this in something called SOAP vault, which will get transmitted through
                    //The SOAP message, and then the client will know how to handle that later.
                    //WCF allows as to do this with something called a FaultException of T.
                    throw new FaultException<NotFoundException>(ex, ex.Message);
                    //The client will be able to trap specifically and translate back to NotFoundException
                    //Also, throwing a FaultException of T will not fault the proxy, which means that the proxy will be usable
                }

                return tour;
            });
        }

        //[OperationBehavior(TransactionScopeRequired = true)]
        //[PrincipalPermission(SecurityAction.Demand, Role = Security.EchoDesertTripsAdminRole)]
        public Tour UpdateTour(Tour tour)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                ITourRepository tourRepository =
                    _DataRepositoryFactory.GetDataRepository<ITourRepository>();

                ITourOptionalRepository tourOptionalRepository =
                    _DataRepositoryFactory.GetDataRepository<ITourOptionalRepository>();

                Tour updateEntity = null;
                if (tour.TourId == 0)
                {
                    updateEntity = tourRepository.Add(tour);
                    tour.TourOptionals.ForEach(t => t.TourId = updateEntity.TourId);
                }
                else
                {
                    updateEntity = tourRepository.Update(tour);
                    tourOptionalRepository.RemoveAll(tour.TourId);
                    tourOptionalRepository.Add(tour.TourOptionals);
                    updateEntity.TourOptionals = tour.TourOptionals;
                }

                //BreakCircularDepndency(updateEntity);

                return updateEntity;
            });
        }

        //private static void BreakCircularDepndency(Tour updateEntity)
        //{
        //    //Remove Optional,Tour From TourOptionals
        //    //if (updateEntity.TourOptionals != null)
        //    //foreach (var tourOptional in updateEntity.TourOptionals)
        //    //{
        //    //    tourOptional.Tour = null;
        //    //    tourOptional.Optional.TourOptionals = null;
        //    //}
        //}
    }
}
