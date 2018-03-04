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

        public InventoryData GetHotelsData()
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IHotelRepository HotelRepository =
                    _DataRepositoryFactory.GetDataRepository<IHotelRepository>();

                IRoomTypeRepository RoomTypeRepository =
                                    _DataRepositoryFactory.GetDataRepository<IRoomTypeRepository>();

                var hotels = HotelRepository.Get();
                var roomTypes = RoomTypeRepository.Get();

                var inventoryData = new InventoryData()
                {
                    Hotels = hotels.ToList().ToArray(),
                    RoomTypes = roomTypes.ToList().ToArray()
                };
                return inventoryData;

            });
        }

        [OperationBehavior(TransactionScopeRequired = true)]
        public void UpdateHotelAndRoomTypes(List<HotelRoomType> hotelRoomTypes)
        {
            ExecuteFaultHandledOperation(() =>
            {
                IHotelRoomTypeRepository hotelRepository =
                    _DataRepositoryFactory.GetDataRepository<IHotelRoomTypeRepository>();
                hotelRoomTypes.ForEach((hotelRoomType) =>
                {
                    HotelRoomType updateEntity = hotelRepository.GetEntity(hotelRoomType.HotelId, hotelRoomType.RoomTypeId);
                    var roomType = hotelRoomType.RoomType;
                    hotelRoomType.RoomType = null;
                    if (updateEntity == null)
                    {
                        updateEntity = hotelRepository.Add(hotelRoomType);
                    }
                    else
                    {
                        updateEntity = hotelRepository.Update(hotelRoomType);
                    }
                    hotelRoomType.RoomType = roomType;
                });
            });
        }
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
                    updateEntity = hotelRepository.Add(hotel);
                }
                else
                {
                    updateEntity = hotelRepository.UpdateHotel(hotel);
                }
                return updateEntity;
            });
        }

        //[OperationBehavior(TransactionScopeRequired = true)]
        //public Nationality UpdateNationality(Nationality nationality)
        //{
        //    return ExecuteFaultHandledOperation(() =>
        //    {
        //        INationalityRepository nationalityRepository =
        //            _DataRepositoryFactory.GetDataRepository<INationalityRepository>();

        //        Nationality updateEntity = null;

        //        if (nationality.NationalityId == 0)
        //        {
        //            updateEntity = nationalityRepository.Add(nationality);
        //        }
        //        else
        //        {
        //            updateEntity = nationalityRepository.Update(nationality);
        //        }

        //        return updateEntity;
        //    });
        //}

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

        public Agency[] GetAllAgencies()
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IAgencyRepository agencyRepository =
                     _DataRepositoryFactory.GetDataRepository<IAgencyRepository>();

                IEnumerable<Agency> agencies = agencyRepository.Get();
                return agencies.ToArray();
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

        //public Nationality[] GetAllNationalities()
        //{
        //    return ExecuteFaultHandledOperation(() =>
        //    {
        //        INationalityRepository nationalityRepository =
        //              _DataRepositoryFactory.GetDataRepository<INationalityRepository>();

        //        IEnumerable<Nationality> Nationalities = nationalityRepository.Get();
        //        return Nationalities.ToArray();
        //    });


        //}

        public TourType[] GetAllTourTypes()
        {
            return ExecuteFaultHandledOperation(() =>
            {
                ITourTypeRepository tripTypeRepository =
                    _DataRepositoryFactory.GetDataRepository<ITourTypeRepository>();

                IEnumerable<TourType> TourTypes = tripTypeRepository.Get();
                return TourTypes.ToArray();
            });
        }

        public Optional[] GetAllOptionals()
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IOptionalRepository optionalRepository =
                    _DataRepositoryFactory.GetDataRepository<IOptionalRepository>();

                IEnumerable<Optional> Optionals = optionalRepository.Get();
                return Optionals.ToArray();
            });
        }

        //public TourDestination[] GetAllTourDestinations()
        //{
        //    return ExecuteFaultHandledOperation(() =>
        //    {
        //        ITourDestinationRepository tourDestinationRepository =
        //            _DataRepositoryFactory.GetDataRepository<ITourDestinationRepository>();

        //        IEnumerable<TourDestination> TourDestinations = tourDestinationRepository.Get();
        //        return TourDestinations.ToArray();
        //    });
        //}

        //public TourDestination UpdateTourDestination(TourDestination tourDestination)
        //{
        //    return ExecuteFaultHandledOperation(() =>
        //    {
        //        ITourDestinationRepository tourDestinationRepository =
        //            _DataRepositoryFactory.GetDataRepository<ITourDestinationRepository>();

        //        TourDestination updateEntity = null;

        //        if (tourDestination.TourDestinationId == 0)
        //        {
        //            updateEntity = tourDestinationRepository.Add(tourDestination);
        //        }
        //        else
        //        {
        //            updateEntity = tourDestinationRepository.Update(tourDestination);
        //        }

        //        return updateEntity;
        //    });
        //}

        public RoomType[] GetAllRoomTypes()
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IRoomTypeRepository roomTypeRepository =
                    _DataRepositoryFactory.GetDataRepository<IRoomTypeRepository>();

                IEnumerable<RoomType> roomTypes = roomTypeRepository.Get();
                return roomTypes.ToArray();
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

        //public void DeleteTourDestination(TourDestination tourDestination)
        //{
        //    ExecuteFaultHandledOperation(() =>
        //    {
        //        ITourDestinationRepository tourDestinationRepository =
        //            _DataRepositoryFactory.GetDataRepository<ITourDestinationRepository>();

        //        tourDestinationRepository.Remove(tourDestination.TourDestinationId);
        //    });
        //}

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

        //public void DeleteNationality(Nationality nationality)
        //{
        //    ExecuteFaultHandledOperation(() =>
        //    {
        //        INationalityRepository nationalityRepository =
        //            _DataRepositoryFactory.GetDataRepository<INationalityRepository>();

        //        nationalityRepository.Remove(nationality.NationalityId);
        //    });
        //}

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
                            RoomTypes = roomTypeRepository.Get().ToArray()
                        };
                    return inventoryData;
                });
                return await task.ConfigureAwait(false);
            });
        }
    }
}
