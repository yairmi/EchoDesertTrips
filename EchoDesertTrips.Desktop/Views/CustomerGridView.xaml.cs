using System.Windows;
using System.Windows.Controls;
using Core.Common.UI.Core;

namespace EchoDesertTrips.Desktop.Views
{
    /// <summary>
    /// Interaction logic for CustomerGridView.xaml
    /// </summary>
    public partial class CustomerGridView : UserControlViewBase
    {
        public CustomerGridView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            // Do not load your data at design time.
            // if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            // {
            // 	//Load your data here and assign the result to the CollectionViewSource.
            // 	System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["Resource Key for CollectionViewSource"];
            // 	myCollectionViewSource.Source = your data
            // }
        }
    }
}
