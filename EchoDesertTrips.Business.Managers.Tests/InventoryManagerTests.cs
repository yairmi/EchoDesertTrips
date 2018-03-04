using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EchoDesertTrips.Business.Entities;
using EchoDesertTrips.Business.Managers.Managers;
using Core.Common.Contracts;
using Moq;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;

namespace EchoDesertTrips.Business.Managers.Tests
{
    [TestClass]
    public class InventoryManagerTests
    {
        [TestInitialize]
        public void Intitialize()
        {
            //GenericPrincipal principal = new GenericPrincipal(
            //    new GenericIdentity("Miguel"), new string[] { "CarRentalAdmin" });

            //Thread.CurrentPrincipal = principal;
        }

        [TestMethod]
        public void UpdateCar_add_new()
        {
            Tour newTour = new Tour();
            Tour addedTour = new Tour() { TourId = 1 };

            Mock<IDataRepositoryFactory> mockRepositoryFactory = new Mock<IDataRepositoryFactory>();
            mockRepositoryFactory.Setup(obj => obj.GetDataRepository<ITourRepository>().Add(newTour)).Returns(addedTour);

            TourManager manager = new TourManager(mockRepositoryFactory.Object);

            Tour results = manager.UpdateTour(newTour);

            Assert.IsTrue(results == addedTour);
        }

        [TestMethod]
        public void UpdateCar_update_existing()
        {
            Tour existingTour = new Tour() { TourId = 1 };
            Tour updatedTour = new Tour() { TourId = 1 };

            Mock<IDataRepositoryFactory> mockRepositoryFactory = new Mock<IDataRepositoryFactory>();
            mockRepositoryFactory.Setup(obj => obj.GetDataRepository<ITourRepository>().Update(existingTour)).Returns(updatedTour);

            TourManager manager = new TourManager(mockRepositoryFactory.Object);

            Tour results = manager.UpdateTour(existingTour);

            Assert.IsTrue(results == updatedTour);
        }
    }
}
