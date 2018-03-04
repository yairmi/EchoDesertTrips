using Core.Common.UI.Core;

namespace EchoDesertTrips.Desktop.Views
{
    /// <summary>
    /// Interaction logic for TourDestination.xaml
    /// </summary>
    public partial class TourDestinationView : UserControlViewBase
    {
        public TourDestinationView()
        {
            InitializeComponent();
        }

        private void UserControlViewBase_Loaded(object sender, System.Windows.RoutedEventArgs e)
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
