using Core.Common.Contracts;
using Core.Common.Core;
using EchoDesertTrips.Business.Common;
using EchoDesertTrips.Business.Contracts;
using EchoDesertTrips.Business.Entities;
using EchoDesertTrips.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using static Core.Common.Core.Const;

namespace EchoDesertTrips.Business.Managers.Managers
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class BroadcastorManager : ManagerBase, IBroadcastorService 
    {
        // list of all currently connected active clients
        private static Dictionary<string, IBroadcastorCallBack> clients = new Dictionary<string, IBroadcastorCallBack>();
        //synchronize the access to the variable clients among multiple threads
        private static object locker = new object();

        public BroadcastorManager()
        {
            ObjectBase.Container.SatisfyImportsOnce(this);
        }

        [Import]
        public IBusinessEngineFactory _BusinessEngineFactory;

        public void RegisterClient(string clientName)
        {
            ExecuteFaultHandledOperation(() =>
            {
                if (!string.IsNullOrEmpty(clientName))
                {
                    var callback =
                        OperationContext.Current.GetCallbackChannel<IBroadcastorCallBack>();
                    lock (locker)
                    {
                        //remove the old client
                        if (clients.Keys.Contains(clientName))
                            clients.Remove(clientName);
                        clients.Add(clientName, callback);
                    }
                }
            });
        }

        public void UnRegisterClient(string clientName)
        {
            ExecuteFaultHandledOperation(() =>
            {
                lock (locker)
                {
                    if (clients.Keys.Contains(clientName))
                        clients.Remove(clientName);
                }
            });
        }

        public void NotifyServer(EventDataType eventData)
        {
            ExecuteFaultHandledOperation(() =>
            {
                lock (locker)
                {
                    var broadcastMsg = new BroadcastMessage();
                    var ser = new Serializer();

                    if (eventData.MessageType == eMsgTypes.E_RESERVATION) //If reservations request
                    {

                        var des = ser.Deserialize<ReservationsMessage>(eventData.EventMessage);
                        var reservations = GetReservationsByIds(des.ReservationsIds);
                        broadcastMsg.MessageType = eMsgTypes.E_RESERVATION;
                        broadcastMsg.ReservationsResult = reservations.ToList();

                    }
                    else if (eventData.MessageType == eMsgTypes.E_INVENTORY)
                    {
                        broadcastMsg.MessageType = eMsgTypes.E_INVENTORY;
                        var des = ser.Deserialize<InventoryMessage>(eventData.EventMessage);
                        if (des.InventoryType == eInventoryTypes.E_HOTEL)
                        {
                            var hotel = GetHotelById(des.EntityId);
                            broadcastMsg.Inventories.Hotels = new Hotel[] { hotel };
                        }
                        else if (des.InventoryType == eInventoryTypes.E_AGENCY)
                        {
                            var agency = GetAgencyById(des.EntityId);
                            broadcastMsg.Inventories.Agencies = new Agency[] { agency };
                        }
                        else if (des.InventoryType == eInventoryTypes.E_OPTIONAL)
                        {
                            var optional = GetOptionalById(des.EntityId);
                            broadcastMsg.Inventories.Optionals = new Optional[] { optional };
                        }
                        else if (des.InventoryType == eInventoryTypes.E_TOUR_TYPE)
                        {
                            var tourType = GetTourTypeById(des.EntityId);
                            broadcastMsg.Inventories.TourTypes = new TourType[] { tourType };
                        }
                        else if (des.InventoryType == eInventoryTypes.E_ROOM_TYPE)
                        {
                            var roomType = GetRoomTypeById(des.EntityId);
                            broadcastMsg.Inventories.RoomTypes = new RoomType[] { roomType };
                        }
                        else if (des.InventoryType == eInventoryTypes.E_OPERATOR)
                        {
                            var _operator = GetOperatorById(des.EntityId);
                            broadcastMsg.Inventories.Operators = new Operator[] { _operator };
                        }
                    }

                    var inactiveClients = new List<string>();

                    foreach (var client in clients)
                    {
                        if (client.Key != eventData.ClientName)
                        {
                            try
                            {
                                client.Value.BroadcastToClient(broadcastMsg);
                            }
                            catch (Exception ex)
                            {
                                inactiveClients.Add(client.Key);
                            }
                        }
                    }

                    if (inactiveClients.Count > 0)
                    {
                        foreach (var client in inactiveClients)
                        {
                            clients.Remove(client);
                        }
                    }
                }
            });
        }

        private Reservation[] GetReservationsByIds(List<int> idList)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IReservationEngine reservationEngine = _BusinessEngineFactory.GetBusinessEngine<IReservationEngine>();
                var reservations = reservationEngine.GetReservationsByIds(idList);
                reservationEngine.PrepareReservationsForTransmition(reservations);
                return reservations;
            });
        }

        private Hotel GetHotelById(int id)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IInventoryEngine inventoryEngine = _BusinessEngineFactory.GetBusinessEngine<IInventoryEngine>();

                return inventoryEngine.GetHotelById(id);
            });
        }

        private RoomType GetRoomTypeById(int id)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IInventoryEngine inventoryEngine = _BusinessEngineFactory.GetBusinessEngine<IInventoryEngine>();

                return inventoryEngine.GetRoomTypeById(id);
            });
        }

        private Optional GetOptionalById(int id)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IInventoryEngine inventoryEngine = _BusinessEngineFactory.GetBusinessEngine<IInventoryEngine>();

                return inventoryEngine.GetOptionalById(id);
            });
        }

        private TourType GetTourTypeById(int id)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IInventoryEngine inventoryEngine = _BusinessEngineFactory.GetBusinessEngine<IInventoryEngine>();

                return inventoryEngine.GetTourTypeById(id);
            });
        }

        private Operator GetOperatorById(int id)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IInventoryEngine inventoryEngine = _BusinessEngineFactory.GetBusinessEngine<IInventoryEngine>();

                return inventoryEngine.GetOperatorById(id);
            });
        }

        private Agency GetAgencyById(int id)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IInventoryEngine inventoryEngine = _BusinessEngineFactory.GetBusinessEngine<IInventoryEngine>();

                return inventoryEngine.GetAgencyById(id);
            });
        }

    }
}
