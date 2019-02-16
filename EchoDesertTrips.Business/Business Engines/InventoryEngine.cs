using EchoDesertTrips.Business.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EchoDesertTrips.Business.Entities;
using Core.Common.Contracts;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using Core.Common.Exceptions;

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
            var agencyRepository = _DataRepositoryFactory.GetDataRepository<IAgencyRepository>();
            var agency = agencyRepository.Get(id);
            if (agency == null)
                throw new NotFoundException(string.Format("Agency was not found"));
            return agency;
        }

        public Hotel GetHotelById(int id)
        {
            var hotelRepository = _DataRepositoryFactory.GetDataRepository<IHotelRepository>();
            var hotel = hotelRepository.Get(id);
            if (hotel == null)
                throw new NotFoundException(string.Format("Hotel was not found"));
            return hotel;
        }

        public Operator GetOperatorById(int id)
        {
            var operatorRepository = _DataRepositoryFactory.GetDataRepository<IOperatorRepository>();
            var _operator = operatorRepository.Get(id);
            if (_operator == null)
                throw new NotFoundException(string.Format("Operator was not found"));
            return _operator;
        }

        public Optional GetOptionalById(int id)
        {
            var optionalRepository = _DataRepositoryFactory.GetDataRepository<IOptionalRepository>();
            var optional = optionalRepository.Get(id);
            if (optional == null)
                throw new NotFoundException(string.Format("Optional was not found"));
            return optional;
        }

        public RoomType GetRoomTypeById(int id)
        {
            var roomTypeRepository = _DataRepositoryFactory.GetDataRepository<IRoomTypeRepository>();
            var roomType = roomTypeRepository.Get(id);
            if (roomType == null)
                throw new NotFoundException(string.Format("RoomType was not found"));
            return roomType;
        }

        public TourType GetTourTypeById(int id)
        {
            var tourTypeRepository = _DataRepositoryFactory.GetDataRepository<ITourTypeRepository>();
            var tourType = tourTypeRepository.Get(id);
            if (tourType == null)
                throw new NotFoundException(string.Format("TourType was not found"));
            return tourType;
        }
    }
}
