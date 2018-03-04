using System.Windows;
using Core.Common.UI.Core;
using System.Windows.Controls;
using System;
using Core.Common.Core;
using EchoDesertTrips.Desktop.ViewModels;

namespace EchoDesertTrips.Desktop.Views
{
    /// <summary>
    /// Interaction logic for HotelView.xaml
    /// </summary>
    public partial class HotelView : UserControlViewBase
    {
        public HotelView()
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

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{

        //    var viewModel = ObjectBase.Container.GetExportedValue<HotelViewModel>();
        //    viewModel.
        //    HotelDataGrid.RaiseEvent(new RowE);
        //}
    }
}
