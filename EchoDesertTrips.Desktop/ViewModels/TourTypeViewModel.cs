using System;
using System.ComponentModel.Composition;
using System.Linq;
using Core.Common.Contracts;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Entities;
using static Core.Common.Core.Const;
using Core.Common.UI.PubSubEvent;
using Core.Common.UI.CustomEventArgs;

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
            _eventAggregator.GetEvent<TourTypeUpdatedEvent>().Subscribe(TourTypeUpdated);
            _eventAggregator.GetEvent<TourTypeCancelledEvent>().Subscribe(TourTypeCancelled);
        }

        private void OnAddTourTypeCommand(object obj)
        {
            CurrentTourTypeViewModel = new EditTourTypeViewModel(_serviceFactory, _messageDialogService, null);
        }

        private void OnEditTourTypeCommand(TourType obj)
        {
            CurrentTourTypeViewModel = new EditTourTypeViewModel(_serviceFactory, _messageDialogService, obj);
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

        private void TourTypeUpdated(TourTypeEventArgs e)
        {
            Inventories.Update(e.TourType);
            try
            {
                Client.NotifyServer(
                    SerializeInventoryMessage(eInventoryTypes.E_TOUR_TYPE, eOperation.E_UPDATED, e.TourType.TourTypeId), eMsgTypes.E_INVENTORY);
            }
            catch (Exception ex)
            {
                log.Error("Notify Server Error: " + ex.Message);
            }
            CurrentTourTypeViewModel = null;
        }

        private void TourTypeCancelled(TourTypeEventArgs obj)
        {
            CurrentTourTypeViewModel = null;
        }
    }
}
