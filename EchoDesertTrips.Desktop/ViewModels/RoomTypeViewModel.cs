using Core.Common.Contracts;
using Core.Common.Core;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Data;

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class RoomTypeViewModel : ViewModelBase
    {
        private IServiceFactory _serviceFactory;

        [ImportingConstructor]
        public RoomTypeViewModel(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
            DeleteRoomTypeCommand = new DelegateCommand<RoomType>(OnDeleteRoomTypeCommand);
            SaveRoomTypeCommand = new DelegateCommand<RoomType>(OnSaveCommand);
            RowEditEndingCommand = new DelegateCommand<RoomType>(OnRowEditEndingCommand);
        }

        public override string ViewTitle => "Room Types";

        public DelegateCommand<RoomType> RowEditEndingCommand { get; set; }

        private void OnRowEditEndingCommand(RoomType roomType)
        {
            if (roomType.IsDirty)
                OnSaveCommand(roomType);
        }

        public DelegateCommand<RoomType> DeleteRoomTypeCommand { get; set; }

        private void OnDeleteRoomTypeCommand(RoomType obj)
        {
            WithClient(_serviceFactory.CreateClient<IInventoryService>(), inventoryClient =>
            {
                inventoryClient.DeleteRoomType(obj);
                RoomTypes.Remove(obj);
            });
        }

        public DelegateCommand<RoomType> SaveRoomTypeCommand { get; set; }

        private RoomType LastUpdatedRoomType;

        private void OnSaveCommand(RoomType roomType)
        {
            LastUpdatedRoomType = roomType;
            ValidateModel();
            if (roomType.IsValid)
            {
                WithClient(_serviceFactory.CreateClient<IInventoryService>(), inventoryClient =>
                {
                    bool bIsNew = roomType.RoomTypeId == 0;
                    var savedRoomType = inventoryClient.UpdateRoomType(roomType);
                    if (bIsNew)
                        RoomTypes[RoomTypes.Count - 1].RoomTypeId = savedRoomType.RoomTypeId;

                });
            }
        }

        protected override void AddModels(List<ObjectBase> models)
        {
            models.Add(LastUpdatedRoomType);
        }

        //private ObservableCollection<RoomType> _roomTypes;

        //public ObservableCollection<RoomType> RoomTypes
        //{
        //    get
        //    {
        //        return _roomTypes;
        //    }

        //    set
        //    {
        //        _roomTypes = value;
        //        OnPropertyChanged(() => RoomTypes, false);
        //    }
        //}

        protected override void OnViewLoaded()
        {
            try
            {
                WithClient<IInventoryService>(_serviceFactory.CreateClient<IInventoryService>(), inventoryClient =>
                {
                    RoomType[] roomTypes = inventoryClient.GetAllRoomTypes();
                    RoomTypes = new ObservableCollection<RoomType>(roomTypes);
                });
            }
            catch (Exception ex)
            {
                log.Error("Exception load roomTypes: " + ex.Message);
            }
        }
    }

    public class RoomTypeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value,
            System.Globalization.CultureInfo cultureInfo)
        {
            RoomType roomType = (value as BindingGroup).Items[0] as RoomType;
            if (roomType.RoomTypeName == string.Empty)
            {
                return new ValidationResult(false,
                    "Room Type name should not be empty");
            }
            else
            {
                var validationResult = ValidationResult.ValidResult;
                return validationResult;
            }
        }
    }
}

