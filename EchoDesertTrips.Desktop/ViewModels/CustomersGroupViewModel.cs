using Core.Common.UI.Core;
using Core.Common.UI.PubSubEvent;
using EchoDesertTrips.Client.Entities;
using System;

namespace EchoDesertTrips.Desktop.ViewModels
{
    public class CustomersGroupViewModel : ViewModelBase
    {
        public CustomersGroupViewModel()
        {
            CustomersForGroup = new RangeObservableCollection<Customer>();
            CloseCommand = new DelegateCommand<Reservation>(OnCloseCommand);
        }

        public RangeObservableCollection<Customer> CustomersForGroup { get; set; }

        public DelegateCommand<Reservation> CloseCommand { get; set; }

        private void OnCloseCommand(Reservation obj)
        {
            _eventAggregator.GetEvent<CustomerGroupClosedEvent>().Publish(new EventArgs());
        }

        protected override void OnViewLoaded()
        {
            LoadingVisible = true;
        }

        private bool _loadingVisible;

        public bool LoadingVisible
        {
            get
            {
                return _loadingVisible;
            }
            set
            {
                if (_loadingVisible != value)
                {
                    _loadingVisible = value;
                    OnPropertyChanged(() => LoadingVisible);
                }
            }
        }
    }   
}
