using AutoMapper;
using Core.Common.Contracts;
using Core.Common.Core;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using EchoDesertTrips.Desktop.Support;
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
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Hotel, Hotel>();
            });

            if (agency != null)
            {

                IMapper iMapper = config.CreateMapper();
                Agency = iMapper.Map<Agency, Agency>(agency);
            }
            else
                Agency = new Agency();

            CleanAll();

            SaveCommand = new DelegateCommand<object>(OnSaveCommand, OnSaveCommandCanExecute);
            CancelCommand = new DelegateCommand<object>(OnCancelCommand);
        }

        public DelegateCommand<object> SaveCommand { get; private set; }
        public DelegateCommand<object> CancelCommand { get; private set; }

        //After pressing the 'Save' button
        private bool OnSaveCommandCanExecute(object obj)
        {
            return IsHotelDirty();
        }

        private void OnSaveCommand(object obj)
        {
            if (IsHotelDirty())
            {
                ValidateModel();
            }
            if (Agency.IsValid)
            {
                WithClient(_serviceFactory.CreateClient<IInventoryService>(), inventoryClient =>
                {
                    bool bIsNew = Agency.AgencyId == 0;
                    var savedAgency = inventoryClient.UpdateAgency(Agency);
                    AgencyUpdated?.Invoke(this, new AgencyEventArgs(savedAgency, null, bIsNew));
                });
            }
        }


        private void OnCancelCommand(object obj)
        {
            if (IsHotelDirty())
            {
                var Result = _messageDialogService.ShowOkCancelDialog((string)Application.Current.FindResource("AreYouSureMessage"), "Question");
                if (Result == MessageDialogResult.CANCEL)
                    return;
            }
            AgencyCancelled?.Invoke(this, new AgencyEventArgs(null, null, false));
        }

        private bool IsHotelDirty()
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

        public event EventHandler<AgencyEventArgs> AgencyUpdated;
        public event EventHandler<AgencyEventArgs> AgencyCancelled;
    }
}
