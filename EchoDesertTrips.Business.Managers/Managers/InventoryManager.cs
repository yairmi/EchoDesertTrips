using Core.Common.Contracts;
using EchoDesertTrips.Business.Contracts;
using System.ComponentModel.Composition;
using EchoDesertTrips.Business.Entities;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Core;
using EchoDesertTrips.Business.Common;
using System;
using System.Threading.Tasks;

namespace EchoDesertTrips.Business.Managers.Managers
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple,
        ReleaseServiceInstanceOnTransactionComplete = false)]
    public class InventoryManager : ManagerBase, IInventoryService
    {
        public InventoryManager()

        {
            ObjectBase.Container.SatisfyImportsOnce(this);
        }
        //For unit testing (Mock parameter). WCF is only interested in the default constructor.
        public InventoryManager(IDataRepositoryFactory DataRepositoryFactory)
        {
            _DataRepositoryFactory = DataRepositoryFactory;
        }

        //For unit testing (Mock parameter). WCF is only interested in the default constructor.
        public InventoryManager(IBusinessEngineFactory BusinessEngineFactory)
        {
            _BusinessEngineFactory = BusinessEngineFactory;
        }

        //For unit testing (Mock parameter). WCF is only interested in the default constructor.
        public InventoryManager(IDataRepositoryFactory DataRepositoryFactory, IBusinessEngineFactory BusinessEngineFactory)
        {
            _DataRepositoryFactory = DataRepositoryFactory;
            _BusinessEngineFactory = BusinessEngineFactory;
        }

        [Import]
        public IDataRepositoryFactory _DataRepositoryFactory;

        [Import]
        public IBusinessEngineFactory _BusinessEngineFactory;

        [OperationBehavior(TransactionScopeRequired = true)]
        public Hotel UpdateHotel(Hotel hotel)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IHotelRepository hotelRepository =
                    _DataRepositoryFactory.GetDataRepository<IHotelRepository>();

                Hotel updateEntity = null;

                if (hotel.HotelId == 0)
                {
                    updateEntity = hotelRepository.AddHotel(hotel);
                }
                else
                {
                    updateEntity = hotelRepository.UpdateHotel(hotel);
                }
                return updateEntity;
            });
        }

        [OperationBehavior(TransactionScopeRequired = true)]
        public TourType UpdateTourType(TourType tourType)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                ITourTypeRepository tripTypeRepository =
                    _DataRepositoryFactory.GetDataRepository<ITourTypeRepository>();

                TourType updateEntity = null;

                if (tourType.TourTypeId == 0)
                {
                    updateEntity = tripTypeRepository.Add(tourType);
                }
                else
                {
                    updateEntity = tripTypeRepository.UpdateTourType(tourType);
                    //updateEntity = tripTypeRepository.Update(tourType);
                }

                return updateEntity;
            });
        }

        [OperationBehavior(TransactionScopeRequired = true)]
        public Optional UpdateOptional(Optional optional)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IOptionalRepository OptionalRepository =
                    _DataRepositoryFactory.GetDataRepository<IOptionalRepository>();

                Optional updateEntity = null;

                if (optional.OptionalId == 0)
                {
                    updateEntity = OptionalRepository.Add(optional);
                }
                else
                {
                    updateEntity = OptionalRepository.Update(optional);
                }

                return updateEntity;
            });
        }

        public InventoryData GetInventoryData()
        {
            
            return ExecuteFaultHandledOperation(() =>
            {
                IHotelRepository HotelRepository =
                    _DataRepositoryFactory.GetDataRepository<IHotelRepository>();

                ITourTypeRepository tourTypeRepository =
                     _DataRepositoryFactory.GetDataRepository<ITourTypeRepository>();

                IAgencyRepository agencyRepository =
                    _DataRepositoryFactory.GetDataRepository<IAgencyRepository>();

                IOptionalRepository optionalRepository =
                    _DataRepositoryFactory.GetDataRepository<IOptionalRepository>();

                IRoomTypeRepository roomTypeRepository =
                    _DataRepositoryFactory.GetDataRepository<IRoomTypeRepository>();

                IOperatorRepository operatorRepository =
                    _DataRepositoryFactory.GetDataRepository<IOperatorRepository>();

                var inventoryData =
                    new InventoryData
                    {
                        Hotels = HotelRepository.Get().ToArray(),
                        TourTypes = tourTypeRepository.Get().ToArray(),
                        Agencies = agencyRepository.Get().ToArray(),
                        Optionals = optionalRepository.Get().ToArray(),
                        RoomTypes = roomTypeRepository.Get().ToArray()
                    };
                return inventoryData;
            });
        }

        public Hotel[] GetAllHotels()
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IHotelRepository hotelRepository =
                     _DataRepositoryFactory.GetDataRepository<IHotelRepository>();

                return hotelRepository.Get().ToArray();
            });
        }

        public Agency[] GetAllAgencies()
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IAgencyRepository agencyRepository =
                     _DataRepositoryFactory.GetDataRepository<IAgencyRepository>();

                return agencyRepository.Get().ToArray();
            });
        }

        public Agency UpdateAgency(Agency agency)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IAgencyRepository agencyRepository =
                    _DataRepositoryFactory.GetDataRepository<IAgencyRepository>();

                Agency updateEntity = null;

                if (agency.AgencyId == 0)
                {
                    updateEntity = agencyRepository.Add(agency);
                }
                else
                {
                    updateEntity = agencyRepository.UpdateAgency(agency);
                }

                return updateEntity;
            });
        }

        public Agent UpdateAgent(Agent agent)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IAgentRepository agentRepository =
                    _DataRepositoryFactory.GetDataRepository<IAgentRepository>();

                Agent updateEntity = null;

                if (agent.AgentId == 0)
                {
                    updateEntity = agentRepository.Add(agent);
                }
                else
                {
                    updateEntity = agentRepository.Update(agent);
                }

                return updateEntity;
            });
        }

        public TourType[] GetAllTourTypes()
        {
            return ExecuteFaultHandledOperation(() =>
            {
                ITourTypeRepository tripTypeRepository =
                    _DataRepositoryFactory.GetDataRepository<ITourTypeRepository>();

                return tripTypeRepository.Get().ToArray();
            });
        }

        public Optional[] GetAllOptionals()
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IOptionalRepository optionalRepository =
                    _DataRepositoryFactory.GetDataRepository<IOptionalRepository>();

                return optionalRepository.Get().ToArray();
            });
        }

        public RoomType[] GetAllRoomTypes()
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IRoomTypeRepository roomTypeRepository =
                    _DataRepositoryFactory.GetDataRepository<IRoomTypeRepository>();

                return roomTypeRepository.Get().ToArray();
            });
        }

        public RoomType UpdateRoomType(RoomType roomType)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IRoomTypeRepository roomTypeRepository =
                    _DataRepositoryFactory.GetDataRepository<IRoomTypeRepository>();

                RoomType updateEntity = null;

                if (roomType.RoomTypeId == 0)
                {
                    updateEntity = roomTypeRepository.Add(roomType);
                }
                else
                {
                    updateEntity = roomTypeRepository.Update(roomType);
                }

                return updateEntity;
            });
        }

        public void DeleteOptional(Optional optional)
        {
            ExecuteFaultHandledOperation(() =>
            {
                IOptionalRepository optionalRepository =
                    _DataRepositoryFactory.GetDataRepository<IOptionalRepository>();

                optionalRepository.Remove(optional.OptionalId);
            });
        }

        public void DeleteRoomType(RoomType roomType)
        {
            ExecuteFaultHandledOperation(() =>
            {
                IRoomTypeRepository roomTypeRepository =
                    _DataRepositoryFactory.GetDataRepository<IRoomTypeRepository>();

                roomTypeRepository.Remove(roomType.RoomTypeId);
            });
        }

        public void DeleteTourType(TourType tourType)
        {
            ExecuteFaultHandledOperation(() =>
            {
                ITourTypeRepository tourTypeRepository =
                    _DataRepositoryFactory.GetDataRepository<ITourTypeRepository>();

                tourTypeRepository.Remove(tourType.TourTypeId);
            });
        }

        public void DeleteAgency(Agency agency)
        {
            ExecuteFaultHandledOperation(() =>
            {
                IAgencyRepository agencyRepository =
                    _DataRepositoryFactory.GetDataRepository<IAgencyRepository>();

                agencyRepository.Remove(agency.AgencyId);
            });
        }

        public void DeleteAgent(Agent agent)
        {
            ExecuteFaultHandledOperation(() =>
            {
                IAgentRepository agentRepository =
                    _DataRepositoryFactory.GetDataRepository<IAgentRepository>();

                agentRepository.Remove(agent.AgentId);
            });
        }

        public void DeleteHotel(Hotel hotel)
        {
            ExecuteFaultHandledOperation(() =>
            {
                IHotelRepository hotelRepository =
                    _DataRepositoryFactory.GetDataRepository<IHotelRepository>();

                hotelRepository.Remove(hotel.HotelId);
            });
        }

        public async Task<InventoryData>  GetInventoryDataAsynchronous()
        {
            return await ExecuteFaultHandledOperation(async () =>
            {
                var task = Task<InventoryData>.Factory.StartNew(() =>
                {
                    IHotelRepository HotelRepository =
                        _DataRepositoryFactory.GetDataRepository<IHotelRepository>();

                    ITourTypeRepository tourTypeRepository =
                         _DataRepositoryFactory.GetDataRepository<ITourTypeRepository>();

                    IAgencyRepository agencyRepository =
                        _DataRepositoryFactory.GetDataRepository<IAgencyRepository>();

                    IOptionalRepository optionalRepository =
                        _DataRepositoryFactory.GetDataRepository<IOptionalRepository>();

                    IRoomTypeRepository roomTypeRepository =
                        _DataRepositoryFactory.GetDataRepository<IRoomTypeRepository>();

                    IOperatorRepository operatorRepository =
                        _DataRepositoryFactory.GetDataRepository<IOperatorRepository>();

                    var inventoryData =
                        new InventoryData
                        {
                            Hotels = HotelRepository.Get().ToArray(),
                            TourTypes = tourTypeRepository.Get().ToArray(),
                            Agencies = agencyRepository.Get().ToArray(),
                            Optionals = optionalRepository.Get().ToArray(),
                            RoomTypes = roomTypeRepository.Get().ToArray(),
                            Operators = operatorRepository.Get().ToArray()
                        };
                    return inventoryData;
                });
                return await task.ConfigureAwait(false);
            });
        }

        public Hotel GetHotelById(int id)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IInventoryEngine inventoryEngine = _BusinessEngineFactory.GetBusinessEngine<IInventoryEngine>();

                return inventoryEngine.GetHotelById(id);
            });
        }

        public TourType GetTourTypeById(int id)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IInventoryEngine inventoryEngine = _BusinessEngineFactory.GetBusinessEngine<IInventoryEngine>();

                return inventoryEngine.GetTourTypeById(id);
            });
        }
    }
}
