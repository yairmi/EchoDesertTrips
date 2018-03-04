using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Common.Core;
using Bootstrapper;
using System.ComponentModel.Composition;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using EchoDesertTrips.Business.Entities;
using Moq;
using Core.Common.Contracts;

namespace EchoDesertTrips.Data.Tests
{
    [TestClass]
    public class DataLayerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            ObjectBase.Container = MEFLoader.Init();
        }
        //This one is like an integration test since it is actually goes to the db
        [TestMethod]
        public void test_repository_uisage()
        {
            RepositoryTestClass rt = new RepositoryTestClass();
            IEnumerable<Tour> trips = rt.GetTrips();

            Assert.IsTrue(trips != null);
        }

        [TestMethod]
        public void test_repository_factory_uisage()
        {
            RepositoryFactoryTestClass rft = new RepositoryFactoryTestClass();
            IEnumerable<Tour> trips = rft.GetTrips();

            Assert.IsTrue(trips != null);
        }

        [TestMethod]
        public void test_repository_mocking()
        {
            List<Tour> trips = new List<Tour>()
            {
                new Tour() { TourId = 1, /*TourDescription = "name1"*/ },
                new Tour() { TourId = 2, /*TourDescription = "name2"*/ }
            };

            Mock<ITourRepository> mockRepository = new Mock<ITourRepository>();
            mockRepository.Setup(obj => obj.Get()).Returns(trips);  //When you see a call to get return cars    

            RepositoryTestClass rt = new RepositoryTestClass(mockRepository.Object);

            IEnumerable<Tour> ret = rt.GetTrips();

            Assert.IsTrue(ret == trips);
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
        public RepositoryTestClass(ITourRepository tripRepository)
        {
            _TripRepository = tripRepository;
        }
        //The import is for MEF to resolve ICarRepository
        [Import]
        ITourRepository _TripRepository;

        public IEnumerable<Tour> GetTrips()
        {
            IEnumerable<Tour> trips = _TripRepository.Get();

            return trips;
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

        public IEnumerable<Tour> GetTrips()
        {
            ITourRepository tripRepository = _dataRepositoryFactory.GetDataRepository<ITourRepository>();

            IEnumerable<Tour> trips = tripRepository.Get();

            return trips;
        }
    }
}
