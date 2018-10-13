using Core.Common.UI.Core;
using EchoDesertTrips.Client.Entities;
using System;
using System.Windows.Data;
using System.Windows.Forms;

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
            Close?.Invoke(this, new EventArgs());
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

        public event EventHandler Close;
    }

    public class StartAtOne : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var i = (int)value;
            return i + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
