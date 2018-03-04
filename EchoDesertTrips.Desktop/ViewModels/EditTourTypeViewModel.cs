using Core.Common.Contracts;
using Core.Common.Core;
using Core.Common.UI.Core;
using Core.Common.Utils;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using EchoDesertTrips.Desktop.Support;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Windows;

namespace EchoDesertTrips.Desktop.ViewModels
{
    public class EditTourTypeViewModel : ViewModelBase
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IMessageDialogService _messageDialogService;

        public EditTourTypeViewModel(IServiceFactory serviceFactory,
            IMessageDialogService messageDialogService,
            TourTypeWrapper tourType)
        {
            _serviceFactory = serviceFactory;
            _messageDialogService = messageDialogService;
            Destinations = new ObservableCollection<Destination>();
            AdultPrices = new ObservableCollection<Prices>();
            ChildPrices = new ObservableCollection<Prices>();

            if (tourType != null)
            {
                TourType = TourTypeHelper.CreateTourTypeWrapper(tourType);
                string[] words = SimpleSplitter.Split(TourType.Destinations);
                foreach (var word in words)
                {
                    var destination = new Destination();
                    destination.Deserialize(word);
                    Destinations.Add(destination);
                }

                string[] separators = { ";" };
                string[] pairs = SimpleSplitter.Split(TourType.AdultPrices, separators);
                foreach(var pair in pairs)
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
            }
            else
            {
                TourType = new TourTypeWrapper();
            }

            Days = TourType.Days;

            CleanAll();

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
                WithClient(_serviceFactory.CreateClient<IInventoryService>(), inventoryClient =>
                {
                    bool bIsNew = TourType.TourTypeId == 0;

                    TourType.Destinations = string.Empty;
                    foreach (var destination in Destinations)
                        TourType.Destinations += destination.Serialize();

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

                    var tourType = AutoMapperUtil.Map<TourTypeWrapper, TourType>(TourType);
                    var savedTourType = AutoMapperUtil.Map<TourType, TourTypeWrapper>(inventoryClient.UpdateTourType(tourType));
                    TourTypeUpdated?.Invoke(this, new TourTypeEventArgs(savedTourType, bIsNew));
                });
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
            TourTypeCancelled?.Invoke(this, new TourTypeEventArgs(null, true));
        }

        private bool IsTourTypeDirty()
        {
            return IsAnythingDirty();
        }

        protected override void AddModels(List<ObjectBase> models)
        {
            models.Add(TourType);
        }

        private TourTypeWrapper _tourType;

        public TourTypeWrapper TourType
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

        private ObservableCollection<Destination> _Destinations;

        public ObservableCollection<Destination> Destinations
        {
            get
            {
                return _Destinations;
            }
            set
            {
                _Destinations = value;
                OnPropertyChanged(() => Destinations);
            }
        }

        private ObservableCollection<Prices> _adultPrices;

        public ObservableCollection<Prices> AdultPrices
        {
            get
            {
                return _adultPrices;
            }
            set
            {
                _adultPrices = value;
                OnPropertyChanged(() => AdultPrices);
            }
        }

        private ObservableCollection<Prices> _childPrices;

        public ObservableCollection<Prices> ChildPrices
        {
            get
            {
                return _childPrices;
            }
            set
            {
                _childPrices = value;
                OnPropertyChanged(() => ChildPrices);
            }
        }

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
            ObservableCollection<TourTypeDescription> tourTypeDescriptions =
                new ObservableCollection<TourTypeDescription>();
            int currentCount = TourType.TourTypeDescriptions.Count;
            if (currentCount == _days && currentCount > 0)
            {
                int i = 0;
                foreach(var description in TourType.TourTypeDescriptions)
                {
                    description.DayNumber = String.Format("Day {0}", i + 1);
                    i++;
                }
            }
            else
            //User incrament day count
            if (currentCount < _days)
            {
                for (int i = currentCount+1; i <= _days; i++)
                {
                    TourType.TourTypeDescriptions.Add(new TourTypeDescription() { DayNumber=String.Format("Day {0}", i)});
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

        public event EventHandler<TourTypeEventArgs> TourTypeUpdated;
        public event EventHandler<TourTypeEventArgs> TourTypeCancelled;
    }
}
