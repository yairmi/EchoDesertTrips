using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Common.Core;
using Bootstrapper;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using EchoDesertTrips.Business.Entities;
using Core.Common.Contracts;
using System.Linq;
using Core.Common.Utils;

namespace EchoDesertTrips.Data.RepositoryTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestInitialize]
        public void Initialize()
        {
            ObjectBase.Container = MEFLoader.Init();
        }

        //This one is like an integration test since it is actually goes to the db
        [TestMethod]
        public void test_repository_usage()
        {
            RepositoryTestClass rt = new RepositoryTestClass();
            IEnumerable<Tour> trips = rt.GetTours();

            Assert.IsTrue(trips != null);
        }

        [TestMethod]
        public void test_update_reservation_usage()
        {
            /*RepositoryTestClass rt = new RepositoryTestClass();

            var hotel = new Hotel()
            {
                HotelId = 1,
                HotelAddress = "Petra 2",
                HotelName = "Petra hotel"
            };

            var tourDestination = new TourDestination()
            {
                TourDestinationId = 7,
                TourDestinationName = "Petra"
            };

            var tourTypeDestinations = new List<TourTypeDestination>();
            tourTypeDestinations.Add(tourDestination);

            var tourType = new TourType()
            {
                TourTypeId = 4,
                //TourDestinationId = 7,
                TourTypeDestinations = tourDestinations,
                TourTypeName = "2 Days In Petra Regular"
            };

            Customer cus = new Customer
            {
                CustomerId = 0,
                FirstName = "Ruth",
                LastName = "Rozen",
                DateOfBirdth = new DateTime(1974, 1, 9),
                IssueData = new DateTime(1985, 1, 9),
                ExpireyDate = new DateTime(2055, 1, 9),
                //NationalityId = 1,
                PassportNumber = "1234",
                Phone1 = "052-3699085"
            };

            Tour tour = new Tour()
            {
                TourId = 0,
                TourType = tourType,
                TourTypeId = 4,
                PickupAddress = "Address 4",
                StartDate = new DateTime(2017, 12, 14),
                EndDate = new DateTime(2017, 12, 16),
                //HotelId = 1,
                //Hotel = hotel,
            };

            Reservation r = new Reservation()
            {
                ReservationId = 0,
                OperatorId = 1,
                Agency = null,
                Agent = null,
                AgencyId = null,
                AgentId = null,
                PickUpTime = new DateTime(2017, 12, 14),
                CreationTime = new DateTime(2017, 12, 1)
            };
            r.Tours = new List<Tour>();
            r.Tours.Add(tour);


            var tmpList = new List<Tour>();
            r.Tours.ForEach((item) =>
            {
                Tour mappedTour = AutoMapperUtil.Map<Tour, Tour>(item);
                tmpList.Add(mappedTour);
            });

            //foreach (var t in r.Tours)
            //    t.Hotel = null;

            r = rt.UpdateReservation(r);*/
        }

        [TestMethod]
        public void test_update_usage()
        {
            RepositoryTestClass rt = new RepositoryTestClass();
            Tour tour = new Tour()
            {
                TourId = 24,
                TourTypeId = 4,
                //TourDescription = "More in Petra",
                PickupAddress = "Address 4",
                StartDate = new DateTime(2017, 12, 14),
                EndDate = new DateTime(2017, 12, 16),
                //HotelId = 1,
                //PricePerAdult = 129,
                //PricePerChild = 149,
            };
            TourOptional tourOptional = new TourOptional()
            {
                OptionalId = 5,
                TourId = 2
            };
            tour.TourOptionals = new List<TourOptional>();
            tour.TourOptionals.Add(tourOptional);
            tour = rt.UpdateTour(tour);

            Assert.IsTrue(tour != null);
        }

        [TestMethod]
        public void test_repository_factory_usage()
        {
            RepositoryFactoryTestClass rft = new RepositoryFactoryTestClass();
            IEnumerable<Tour> tours = rft.GetTours();

            Assert.IsTrue(tours != null);
        }
    }

    public class RepositoryTestClass
    {
        public RepositoryTestClass()
        {
            //It tells MEF go ahead and resolve me. Because RepositoryFactoryTestClass
            //was not exported , it did not participate in the MEF building, in the container building
            //I'm telling it explicity here you probably have dependencies defined somewhere
            //Within the body of your code, Go ahead resolve them if you can
            ObjectBase.Container.SatisfyImportsOnce(this);
        }
        //In that way we can send a mock repository
        public RepositoryTestClass(ITourRepository tourRepository, IReservationRepository reservationRepository)
        {
            _TourRepository = tourRepository;
            _ReservationRepository = reservationRepository;
        }
        //The import is for MEF to resolve ICarRepository
        [Import]
        ITourRepository _TourRepository;

        [Import]
        IReservationRepository _ReservationRepository;

        public IEnumerable<Tour> GetTours()
        {
            IEnumerable<Tour> tours = _TourRepository.Get();

            return tours;
        }

        public Tour AddTour(Tour tour)
        {
            tour = _TourRepository.Add(tour);

            return tour;
        }

        public Tour UpdateTour(Tour tour)
        {
            tour = _TourRepository.Update(tour);

            return tour;
        }

        public Reservation UpdateReservation(Reservation r)
        {
            r = _ReservationRepository.Update(r);

            return r;
        }
    }

    public class RepositoryFactoryTestClass
    {
        public RepositoryFactoryTestClass()
        {
            //It tells MEF go ahead and resolve me. Because RepositoryFactoryTestClass
            //was not exported , it did not participate in the MEF building, in the container building
            //I'm telling it explicity here you probably have dependencies defined somewhere
            //Within the body of your code, Go ahead resolve them if you can
            ObjectBase.Container.SatisfyImportsOnce(this);
        }

        public RepositoryFactoryTestClass(IDataRepositoryFactory dataRepositoryFactory)
        {
            _dataRepositoryFactory = dataRepositoryFactory;
        }
        //Mark it so MEF knows to resolve it.
        [Import]
        IDataRepositoryFactory _dataRepositoryFactory;

        public IEnumerable<Tour> GetTours()
        {
            ITourRepository tourRepository = _dataRepositoryFactory.GetDataRepository<ITourRepository>();

            IEnumerable<Tour> tours = tourRepository.Get();

            return tours;
        }
    }
}
