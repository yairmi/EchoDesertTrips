using AutoMapper;
using Core.Common.Contracts;
using Core.Common.Core;
using Core.Common.UI.Core;
using Core.Common.UI.CustomEventArgs;
using Core.Common.UI.PubSubEvent;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using System;
using System.Collections.Generic;
using System.Windows;

namespace EchoDesertTrips.Desktop.ViewModels
{
    public class EditAgentsViewModel : ViewModelBase
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IMessageDialogService _messageDialogService;

        public EditAgentsViewModel(IServiceFactory serviceFactory,
            IMessageDialogService messageDialogService,
            Agency agency)
        {
            _serviceFactory = serviceFactory;
            _messageDialogService = messageDialogService;
            Agency = agency;
            CleanAll();
            SaveCommand = new DelegateCommand<object>(OnSaveCommand, OnSaveCommandCanExecute);
            CancelCommand = new DelegateCommand<object>(OnCancelCommand);
        }

        public DelegateCommand<object> SaveCommand { get; private set; }
        public DelegateCommand<object> CancelCommand { get; private set; }

        //After pressing the 'Save' button
        private bool OnSaveCommandCanExecute(object obj)
        {
            return IsAgencyDirty();
        }

        private void OnSaveCommand(object obj)
        {
            if (IsAgencyDirty())
            {
                ValidateModel();
            }
            if (Agency.IsValid)
            {
                Agency savedAgency = null;
                bool bIsNew = Agency.AgencyId == 0;

                try
                {
                    WithClient(_serviceFactory.CreateClient<IInventoryService>(), inventoryClient =>
                    {
                        
                        savedAgency = inventoryClient.UpdateAgency(Agency);
                    });
                }
                catch(Exception ex)
                {
                    log.Error(string.Empty, ex);
                }

                if (savedAgency != null)
                {
                    _eventAggregator.GetEvent<AgencyUpdatedEvent>().Publish(new AgencyEventArgs(savedAgency, bIsNew));
                }
            }
        }


        private void OnCancelCommand(object obj)
        {
            if (IsAgencyDirty())
            {
                var Result = _messageDialogService.ShowOkCancelDialog((string)Application.Current.FindResource("AreYouSureMessage"), "Question");
                if (Result == MessageDialogResult.CANCEL)
                    return;
            }
            _eventAggregator.GetEvent<AgencyCancelledEvent>().Publish(new AgencyEventArgs(null, false));
        }

        private bool IsAgencyDirty()
        {
            return Agency.IsAnythingDirty();
        }

        protected override void AddModels(List<ObjectBase> models)
        {
            models.Add(Agency);
        }

        private Agency _agency;

        public Agency Agency
        {
            get
            {
                return _agency;
            }
            set
            {
                _agency = value;
                OnPropertyChanged(() => Agency, true);
            }
        }
    }
}
