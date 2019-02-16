using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Core.Common.Contracts;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using EchoDesertTrips.Desktop.Support;
using Core.Common.Utils;
using EchoDesertTrips.Desktop.CustomEventArgs;
using static Core.Common.Core.Const;

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class TourTypeViewModel : ViewModelBase
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IMessageDialogService _messageDialogService;

        [ImportingConstructor]
        public TourTypeViewModel(IServiceFactory serviceFactory,
                    IMessageDialogService messageDialogService)
        {
            _serviceFactory = serviceFactory;
            _messageDialogService = messageDialogService;
            EditTourTypeCommand = new DelegateCommand<TourType>(OnEditTourTypeCommand);
            AddTourTypeCommand = new DelegateCommand<object>(OnAddTourTypeCommand);
        }

        private void OnAddTourTypeCommand(object obj)
        {
            CurrentTourTypeViewModel = new EditTourTypeViewModel(_serviceFactory, _messageDialogService, null);
            RegisterEvents();
        }

        private void OnEditTourTypeCommand(TourType obj)
        {
            CurrentTourTypeViewModel = new EditTourTypeViewModel(_serviceFactory, _messageDialogService, obj);
            RegisterEvents();
        }

        public DelegateCommand<TourType> EditTourTypeCommand { get; private set; }
        public DelegateCommand<object> AddTourTypeCommand { get; private set; }

        private EditTourTypeViewModel _editTourTypeViewModel;

        public EditTourTypeViewModel CurrentTourTypeViewModel
        {
            get { return _editTourTypeViewModel; }
            set
            {
                if (_editTourTypeViewModel != value)
                {
                    _editTourTypeViewModel = value;
                    OnPropertyChanged(() => CurrentTourTypeViewModel, false);
                }
            }
        }

        public override string ViewTitle => "Tour-Types";

        //protected override void OnViewLoaded()
        //{
        //    try
        //    {
        //        WithClient<IInventoryService>(_serviceFactory.CreateClient<IInventoryService>(), inventoryClient =>
        //        {
        //            TourTypes.Clear();
        //            InventoryData inventoryData = inventoryClient.GetInventoryData();
        //            foreach (var tourType in inventoryData.TourTypes)
        //                TourTypes.Add(AutoMapperUtil.Map<TourType, TourTypeWrapper>(tourType));
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Exception load TourTypes/TourDestinations: " + ex.Message);
        //    }
        //}

        private void CurrentTourTypeViewModel_TourTypeUpdated(object sender, TourTypeEventArgs e)
        {
            //var tourType_e = e.TourType;
            if (!e.IsNew)
            {
                //This is done in order to update the Grid. Remember that in EditTripViewModel the updated trip
                //Is a temporary object and it is not part of the Grid collection trips.
                var tourType = TourTypes.FirstOrDefault(item => item.TourTypeId == e.TourType.TourTypeId);
                if (tourType != null)
                {
                    var index = TourTypes.IndexOf(tourType);
                    TourTypes[index] = e.TourType;// tourType_e;
                }
            }
            else
            {
                TourTypes.Add(e.TourType);
            }

            NotifyServer("CurrentTourTypeViewModel_TourTypeUpdated", eInventoryTypes.E_TOUR_TYPE, e.TourType.TourTypeId);
            CurrentTourTypeViewModel = null;
        }

        private void CurrentTourTypeViewModel_TourTypeCancelled(object sender, TourTypeEventArgs e)
        {
            CurrentTourTypeViewModel = null;
        }

        private void RegisterEvents()
        {
            CurrentTourTypeViewModel.TourTypeUpdated -= CurrentTourTypeViewModel_TourTypeUpdated;
            CurrentTourTypeViewModel.TourTypeUpdated += CurrentTourTypeViewModel_TourTypeUpdated;
            CurrentTourTypeViewModel.TourTypeCancelled -= CurrentTourTypeViewModel_TourTypeCancelled;
            CurrentTourTypeViewModel.TourTypeCancelled += CurrentTourTypeViewModel_TourTypeCancelled;
        }
    }
}
