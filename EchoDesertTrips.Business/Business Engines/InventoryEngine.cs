using EchoDesertTrips.Business.Common;
using System;
using System.ComponentModel.Composition;
using EchoDesertTrips.Business.Entities;
using Core.Common.Contracts;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using Core.Common.Exceptions;
using Core.Common.Core;

namespace EchoDesertTrips.Business.Business_Engines
{
    [Export(typeof(IInventoryEngine))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class InventoryEngine : IInventoryEngine
    {
        [ImportingConstructor]
        public InventoryEngine(IDataRepositoryFactory dataRepositoryFactory)
        {
            _DataRepositoryFactory = dataRepositoryFactory;
        }

        IDataRepositoryFactory _DataRepositoryFactory;

        public Agency GetAgencyById(int id)
        {
            return ExecuteGetEntityById(() =>
            {
                var agencyRepository = _DataRepositoryFactory.GetDataRepository<IAgencyRepository>();
                return agencyRepository.Get(id);
            }, id);
        }

        public Hotel GetHotelById(int id)
        {
            return ExecuteGetEntityById(() =>
            {
                var hotelRepository = _DataRepositoryFactory.GetDataRepository<IHotelRepository>();
                return hotelRepository.Get(id);
            }, id);
        }

        public Operator GetOperatorById(int id)
        {
            return ExecuteGetEntityById(() =>
            {
                var operatorRepository = _DataRepositoryFactory.GetDataRepository<IOperatorRepository>();
                return operatorRepository.Get(id);
            }, id);
        }

        public Optional GetOptionalById(int id)
        {
            return ExecuteGetEntityById(() =>
            {
                var optionalRepository = _DataRepositoryFactory.GetDataRepository<IOptionalRepository>();
                return optionalRepository.Get(id);
            }, id);
        }

        public RoomType GetRoomTypeById(int id)
        {
            return ExecuteGetEntityById(() =>
            {
                var roomTypeRepository = _DataRepositoryFactory.GetDataRepository<IRoomTypeRepository>();
                return roomTypeRepository.Get(id);
            }, id);
        }

        public TourType GetTourTypeById(int id)
        {
            return ExecuteGetEntityById(() =>
            {
                var tourTypeRepository = _DataRepositoryFactory.GetDataRepository<ITourTypeRepository>();
                return tourTypeRepository.Get(id);
            }, id);
        }

        private T ExecuteGetEntityById<T>(Func<T> CodeToExecute, int id) where T: EntityBase
        {
            T entity = CodeToExecute();
            if (entity == null)
                throw new NotFoundException($"{typeof(T)} was not found for id: {id}");
            return entity;
        }
    }
}
