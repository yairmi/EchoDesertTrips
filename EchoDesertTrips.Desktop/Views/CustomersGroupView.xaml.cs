using Core.Common.UI.Core;

namespace EchoDesertTrips.Desktop.Views
{
    /// <summary>
    /// Interaction logic for CustomersGroup.xaml
    /// </summary>
    public partial class CustomersGroupView : UserControlViewBase
    {
        public CustomersGroupView()
        {
            InitializeComponent();
        }

        private void dataGridCustomers_LoadingRow(object sender, System.Windows.Controls.DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
