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

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class TourTypeViewModel : ViewModelBase
    {
        private IServiceFactory _serviceFactory;
        private IMessageDialogService _messageDialogService;

        [ImportingConstructor]
        public TourTypeViewModel(IServiceFactory serviceFactory,
                    IMessageDialogService messageDialogService)
        {
            _serviceFactory = serviceFactory;
            _messageDialogService = messageDialogService;
            EditTourTypeCommand = new DelegateCommand<TourTypeWrapper>(OnEditTourTypeCommand);
            AddTourTypeCommand = new DelegateCommand<object>(OnAddTourTypeCommand);
            //TourTypesW = new ObservableCollection<TourTypeWrapper>();
        }

        private void OnAddTourTypeCommand(object obj)
        {
            CurrentTourTypeViewModel = new EditTourTypeViewModel(_serviceFactory, _messageDialogService, null);
            RegisterEvents();
        }

        private void OnEditTourTypeCommand(TourTypeWrapper obj)
        {
            CurrentTourTypeViewModel = new EditTourTypeViewModel(_serviceFactory, _messageDialogService, obj);
            RegisterEvents();
        }

        public DelegateCommand<TourTypeWrapper> EditTourTypeCommand { get; private set; }
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
        //public DelegateCommand<TourType> DeleteTourTypeCommand { get; set; }

        //private void OnDeleteTourTypeCommand(TourType obj)
        //{
        //    WithClient(_serviceFactory.CreateClient<IInventoryService>(), inventoryClient =>
        //    {
        //        inventoryClient.DeleteTourType(obj);
        //        TourTypes.Remove(obj);
        //    });
        //}

        //public DelegateCommand<TourType> SaveTripTypeCommand { get; set; }

        //private TourType LastUpdatedTourType;

        //private void OnSaveCommand(TourType tourType)
        //{
        //    LastUpdatedTourType = tourType;
        //    ValidateModel();
        //    if (tourType.IsValid)
        //    {
        //        WithClient(_serviceFactory.CreateClient<IInventoryService>(), inventoryClient =>
        //        {
        //            bool bIsNew = tourType.TourTypeId == 0;
        //            var savedTourType = inventoryClient.UpdateTourType(tourType);
        //            if (bIsNew)
        //                TourTypes[TourTypes.Count - 1].TourTypeId = savedTourType.TourTypeId;
        //        });
        //    }
        //}

        //protected override void AddModels(List<ObjectBase> models)
        //{
        //    models.Add(LastUpdatedTourType);
        //}

        //public DelegateCommand<TourType> RowEditEndingCommand { get; set; }

        //private void OnRowEditEndingCommand(TourType tourType)
        //{
        //    if (tourType.IsDirty)
        //        OnSaveCommand(tourType);
        //}

        public override string ViewTitle => "Tour-Types";

        //private ObservableCollection<TourTypeWrapper> _tourTypesW;

        //public ObservableCollection<TourTypeWrapper> TourTypesW
        //{
        //    get
        //    {
        //        return _tourTypesW;
        //    }

        //    set
        //    {
        //        _tourTypesW = value;
        //        OnPropertyChanged(() => TourTypesW, false);
        //    }
        //}

        //private ObservableCollection<TourDestination> _tourDestinations;

        //public ObservableCollection<TourDestination> TourDestinations
        //{
        //    get
        //    {
        //        return _tourDestinations;
        //    }

        //    set
        //    {
        //        _tourDestinations = value;
        //        OnPropertyChanged(() => TourDestinations, false);
        //    }
        //}

        protected override void OnViewLoaded()
        {
            try
            {
                WithClient<IInventoryService>(_serviceFactory.CreateClient<IInventoryService>(), inventoryClient =>
                {
                    TourTypes.Clear();
                    InventoryData inventoryData = inventoryClient.GetInventoryData();
                    foreach (var tourType in inventoryData.TourTypes)
                        TourTypes.Add(AutoMapperUtil.Map<TourType, TourTypeWrapper>(tourType));
                });
            }
            catch (Exception ex)
            {
                log.Error("Exception load TourTypes/TourDestinations: " + ex.Message);
            }
        }

        //private void CurrentTourTypeViewModel_TourTypeUpdated(object sender, TourTypeEventArgs e)
        //{
        //    var mappedTourType = AutoMapperUtil.Map<TourTypeWrapper, TourTypeWrapper>(e.TourType);
        //    if (!e.IsNew)
        //    {
        //        //This is done in order to update the Grid. Remember that in EditTripViewModel the updated trip
        //        //Is a temporary object and it is not part of the Grid collection trips.
        //        var tourType = TourTypesW.FirstOrDefault(item => item.TourTypeId == e.TourType.TourTypeId);
        //        if (tourType != null)
        //        {
        //            var index = TourTypesW.IndexOf(tourType);
        //            TourTypesW[index] = mappedTourType;
        //        }
        //    }
        //    else
        //    {
        //        TourTypesW.Add(e.TourType);
        //    }

        //    NotifyServer("CurrentTourTypeViewModel_TourTypeUpdated");
        //    CurrentTourTypeViewModel = null;
        //}

        private void CurrentTourTypeViewModel_TourTypeUpdated(object sender, TourTypeEventArgs e)
        {
            var mappedTourType = AutoMapperUtil.Map<TourTypeWrapper, TourTypeWrapper>(e.TourType);
            if (!e.IsNew)
            {
                //This is done in order to update the Grid. Remember that in EditTripViewModel the updated trip
                //Is a temporary object and it is not part of the Grid collection trips.
                var tourType = TourTypes.FirstOrDefault(item => item.TourTypeId == e.TourType.TourTypeId);
                if (tourType != null)
                {
                    var index = TourTypes.IndexOf(tourType);
                    TourTypes[index] = mappedTourType;
                }
            }
            else
            {
                TourTypes.Add(e.TourType);
            }

            NotifyServer("CurrentTourTypeViewModel_TourTypeUpdated", 2);
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

/*    public class TourTypeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value,
            System.Globalization.CultureInfo cultureInfo)
        {
            TourType tourType = (value as BindingGroup).Items[0] as TourType;
            if (tourType.TourTypeName == string.Empty)
            {
                return new ValidationResult(false,
                    "Tour Type name should not be empty");
            }
            else if (tourType.TourDestination.TourDestinationId == 0)
            {
                return new ValidationResult(false,
                    "Tour Destination should not be empty");
            }
            else
            {
                var validationResult = ValidationResult.ValidResult;
                return validationResult;
            }
        }
    }*/
}
