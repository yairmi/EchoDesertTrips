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
    public class TourDestinationViewModel : ViewModelBase
    {
        private IServiceFactory _serviceFactory;

        [ImportingConstructor]
        public TourDestinationViewModel(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
            TourDestinations = new ObservableCollection<TourDestination>();
            //DeleteTourDestinationCommand = new DelegateCommand<TourDestinationWrapper>(OnDeleteTourDestinationCommand);
            SaveTourDestinationCommand = new DelegateCommand<TourDestination>(OnSaveCommand);
            RowEditEndingCommand = new DelegateCommand<TourDestination>(OnRowEditEndingCommand);
        }

        public override string ViewTitle => "Tour Destinations";

        public DelegateCommand<TourDestination> RowEditEndingCommand { get; set; }

        private void OnRowEditEndingCommand(TourDestination tourDestination)
        {
            if (tourDestination.IsDirty)
                OnSaveCommand(tourDestination);
        }

        //public DelegateCommand<TourDestinationWrapper> DeleteTourDestinationCommand { get; set; }

        //private void OnDeleteTourDestinationCommand(TourDestination obj)
        //{
        //    WithClient(_serviceFactory.CreateClient<IInventoryService>(), inventoryClient =>
        //    {
        //        inventoryClient.DeleteTourDestination(obj);
        //        TourDestinations.Remove(obj);
        //    });
        //}

        public DelegateCommand<TourDestination> SaveTourDestinationCommand { get; set; }

        private TourDestination LastUpdatedTourDestination;

        private void OnSaveCommand(TourDestination tourDestination)
        {
            LastUpdatedTourDestination = tourDestination;
            ValidateModel();
            if (tourDestination.IsValid)
            {
                WithClient(_serviceFactory.CreateClient<IInventoryService>(), inventoryClient =>
                {
                    bool bIsNew = tourDestination.TourDestinationId == 0;
                    var savedTourDestination = inventoryClient.UpdateTourDestination(tourDestination);
                    if (bIsNew)
                        TourDestinations[TourDestinations.Count - 1].TourDestinationId = savedTourDestination.TourDestinationId;
                });
            }
        }

        protected override void AddModels(List<ObjectBase> models)
        {
            models.Add(LastUpdatedTourDestination);
        }

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
                    TourDestination[] tourDestinations = inventoryClient.GetAllTourDestinations();
                    foreach(var tourDestination in tourDestinations)
                    TourDestinations.Add(tourDestination);
                });
            }
            catch (Exception ex)
            {
                log.Error("Exception load tour destinations: " + ex.Message);
            }
        }
    }

    public class TourDestinationValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value,
            System.Globalization.CultureInfo cultureInfo)
        {
            TourDestination tourDestination = (value as BindingGroup).Items[0] as TourDestination;
            if (tourDestination.TourDestinationName == string.Empty)
            {
                return new ValidationResult(false,
                    "Tour Destination name should not be empty");
            }
            else
            {
                var validationResult = ValidationResult.ValidResult;
                return validationResult;
            }
        }
    }
}
