﻿using Core.Common.Contracts;
using Core.Common.Core;
using Core.Common.UI.Core;
using Core.Common.UI.CustomEventArgs;
using Core.Common.UI.PubSubEvent;
using Core.Common.Utils;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using EchoDesertTrips.Desktop.Support;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace EchoDesertTrips.Desktop.ViewModels
{
    public class EditTourTypeViewModel : ViewModelBase
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IMessageDialogService _messageDialogService;

        public EditTourTypeViewModel(IServiceFactory serviceFactory,
            IMessageDialogService messageDialogService,
            TourType tourType)
        {
            _serviceFactory = serviceFactory;
            _messageDialogService = messageDialogService;
            TourType = tourType;
            SaveCommand = new DelegateCommand<object>(OnSaveCommand, OnSaveCommandCanExecute);
            CancelCommand = new DelegateCommand<object>(OnCancelCommand);
        }

        public DelegateCommand<object> SaveCommand { get; private set; }
        public DelegateCommand<object> CancelCommand { get; private set; }

        //After pressing the 'Save' button
        private bool OnSaveCommandCanExecute(object obj)
        {
            return IsTourTypeDirty();
        }

        private void OnSaveCommand(object obj)
        {
            if (IsTourTypeDirty())
            {
                ValidateModel();
            }
            if (TourType.IsValid)
            {
                TourType.Destinations = string.Empty;
                byte days = 0;
                foreach (var destination in Destinations)
                {
                    if (days >= TourType.Days)
                        break;
                    TourType.Destinations += destination.Serialize();
                    days++;
                }

                TourType.AdultPrices = string.Empty;
                foreach (var adultPrice in AdultPrices)
                {
                    TourType.AdultPrices += adultPrice.Serialize();
                }

                TourType.ChildPrices = string.Empty;
                foreach (var childPrice in ChildPrices)
                {
                    TourType.ChildPrices += childPrice.Serialize();
                }

                TourType.InfantPrices = string.Empty;
                foreach (var infantPrice in InfantPrices)
                {
                    TourType.InfantPrices += infantPrice.Serialize();
                }

                TourType savedTourType = null;
                bool bIsNew = TourType.TourTypeId == 0;

                try
                {
                    WithClient(_serviceFactory.CreateClient<IInventoryService>(), inventoryClient =>
                    {
                        savedTourType = inventoryClient.UpdateTourType(TourType);
                    });
                }
                catch(Exception ex)
                {
                    log.Error($"Failed to Save/Update Agent.\n{ex.StackTrace}");
                    _messageDialogService.ShowInfoDialog("Failed to save Tour Type", "Error!");
                }

                _eventAggregator.GetEvent<TourTypeUpdatedEvent>().Publish(new TourTypeEventArgs(savedTourType, bIsNew));
            }
        }


        private void OnCancelCommand(object obj)
        {
            if (IsTourTypeDirty())
            {
                var Result = _messageDialogService.ShowOkCancelDialog((string)Application.Current.FindResource("AreYouSureMessage"), "Question");
                if (Result == MessageDialogResult.CANCEL)
                    return;
            }
            _eventAggregator.GetEvent<TourTypeCancelledEvent>().Publish(new TourTypeEventArgs(null, true));
        }

        private bool IsTourTypeDirty()
        {
            return IsAnythingDirty();
        }

        protected override void AddModels(List<ObjectBase> models)
        {
            models.Add(TourType);
        }

        protected override void OnViewLoaded()
        {
            Destinations = new ObservableCollection<Destination>();
            AdultPrices = new ObservableCollection<Prices>();
            ChildPrices = new ObservableCollection<Prices>();
            InfantPrices = new ObservableCollection<Prices>();
            if (TourType != null && TourType.TourTypeId > 0 )
            {
                string[] words = SimpleSplitter.Split(TourType.Destinations);
                foreach (var word in words)
                {
                    var destination = new Destination();
                    destination.Deserialize(word);
                    Destinations.Add(destination);
                }

                string[] separators = { ";" };
                string[] pairs = SimpleSplitter.Split(TourType.AdultPrices, separators);
                foreach (var pair in pairs)
                {
                    var prices = new Prices();
                    prices.Deserialize(pair);
                    AdultPrices.Add(prices);
                }

                pairs = SimpleSplitter.Split(TourType.ChildPrices, separators);
                foreach (var pair in pairs)
                {
                    var prices = new Prices();
                    prices.Deserialize(pair);
                    ChildPrices.Add(prices);
                }

                pairs = SimpleSplitter.Split(TourType.InfantPrices, separators);
                foreach (var pair in pairs)
                {
                    var prices = new Prices();
                    prices.Deserialize(pair);
                    InfantPrices.Add(prices);
                }
            }
            else
            {
                TourType = new TourType();
            }

            Days = TourType.Days;

            CleanAll();
        }

        private TourType _tourType;

        public TourType TourType
        {
            get
            {
                return _tourType;
            }
            set
            {
                _tourType = value;
                OnPropertyChanged(() => TourType, true);
            }
        }

        public ObservableCollection<Destination> Destinations { get; set; }
        public ObservableCollection<Prices> AdultPrices { get; set; }
        public ObservableCollection<Prices> ChildPrices { get; set; }
        public ObservableCollection<Prices> InfantPrices { get; set; }

        private byte _days;

        public byte Days
        {
            get
            {
                return _days;
            }

            set
            {
                if (_days != value)
                {
                    _days = value;
                    TourType.Days = _days;
                    SetTourTypeDescriptionCount();
                    OnPropertyChanged(() => Days);
                }
            }
        }

        private void SetTourTypeDescriptionCount()
        {
            int currentCount = TourType.TourTypeDescriptions.Count;
            if (currentCount > 0 && currentCount == _days)
            {
                int i = 0;
                foreach(var description in TourType.TourTypeDescriptions)
                {
                    description.DayNumber = $"Day {i}";
                    i++;
                }
            }
            else
            //User incrament day count
            if (currentCount < _days)
            {
                for (int i = currentCount+1; i <= _days; i++)
                {
                    TourType.TourTypeDescriptions.Add(new TourTypeDescription() { DayNumber=$"Day {i}" });
                }
            }
            else
            {
                for (int i = 0; i < currentCount - _days; i++)
                {
                    TourType.TourTypeDescriptions.Remove(TourType.TourTypeDescriptions[TourType.TourTypeDescriptions.Count - 1 + i]);
                }
            }
        }
    }
}
