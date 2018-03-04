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
    public class DestinationViewModel : ViewModelBase
    {
        private IServiceFactory _serviceFactory;

        [ImportingConstructor]
        public DestinationViewModel(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
            DeleteTourDestinationCommand = new DelegateCommand<Destination>(OnDeleteTourDestinationCommand);
            SaveTourDestinationCommand = new DelegateCommand<Destination>(OnSaveCommand);
            RowEditEndingCommand = new DelegateCommand<Destination>(OnRowEditEndingCommand);
        }

        public override string ViewTitle => "Tour Destinations";

        public DelegateCommand<Destination> RowEditEndingCommand { get; set; }

        private void OnRowEditEndingCommand(Destination tourDestination)
        {
            if (tourDestination.IsDirty)
                OnSaveCommand(tourDestination);
        }

        public DelegateCommand<Destination> DeleteTourDestinationCommand { get; set; }

        private void OnDeleteTourDestinationCommand(Destination obj)
        {
            WithClient(_serviceFactory.CreateClient<IInventoryService>(), inventoryClient =>
            {
                inventoryClient.DeleteDestination(obj);
                Destinations.Remove(obj);
            });
        }

        public DelegateCommand<Destination> SaveTourDestinationCommand { get; set; }

        private Destination LastUpdatedDestination;

        private void OnSaveCommand(Destination destination)
        {
            LastUpdatedDestination = destination;
            ValidateModel();
            if (destination.IsValid)
            {
                WithClient(_serviceFactory.CreateClient<IInventoryService>(), inventoryClient =>
                {
                    bool bIsNew = destination.DestinationId == 0;
                    var savedDestination = inventoryClient.UpdateDestination(destination);
                    if (bIsNew)
                        Destinations[Destinations.Count - 1].DestinationId = savedDestination.DestinationId;
                });
            }
        }

        protected override void AddModels(List<ObjectBase> models)
        {
            models.Add(LastUpdatedDestination);
        }

        private ObservableCollection<Destination> _destinations;

        public ObservableCollection<Destination> Destinations
        {
            get
            {
                return _destinations;
            }

            set
            {
                _destinations = value;
                OnPropertyChanged(() => Destinations, false);
            }
        }

        protected override void OnViewLoaded()
        {
            try
            {
                WithClient<IInventoryService>(_serviceFactory.CreateClient<IInventoryService>(), inventoryClient =>
                {
                    Destination[] destinations = inventoryClient.GetAllTourDestinations();
                    Destinations = new ObservableCollection<Destination>(destinations);
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
            Destination destination = (value as BindingGroup).Items[0] as Destination;
            if (destination.DestinationName == string.Empty)
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
