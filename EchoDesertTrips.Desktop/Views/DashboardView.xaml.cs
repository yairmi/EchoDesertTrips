using Core.Common.Contracts;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using EchoDesertTrips.Client.Proxies;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EchoDesertTrips.Desktop.Views
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControlViewBase
    {
        private IEnumerable<Reservation> Reservations;
        private List<Customer> Customers;
        private IServiceFactory _serviceFactory;

        public DashboardView()
        {
            
            InitializeComponent();
        }

        private void _selectedDate_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _serviceFactory = new ServiceFactory();
            var orderClient = _serviceFactory.CreateClient<IOrderService>();
            Reservations = orderClient.GetReservationsByGroupId(3);
            var reservationsSource = new ReportDataSource { Name = "Reservations", Value = Reservations };
            _reportViewer.LocalReport.DataSources.Add(reservationsSource);
            _reportViewer.LocalReport.ReportEmbeddedResource = "My.Assembly.ReportName.rdlc";
            /*var report = new LocalReport();
            report.DataSources.Add(reservationsSource);
            report.ReportEmbeddedResource = "My.Assembly.ReportName.rdlc";
            const string DeviceInfo = "<DeviceInfo>" +
                          "  <OutputFormat>EMF</OutputFormat>" +
                          "  <PageWidth>210mm</PageWidth>" +
                          "  <PageHeight>297mm</PageHeight>" +
                          "  <MarginTop>10mm</MarginTop>" +
                          "  <MarginLeft>10mm</MarginLeft>" +
                          "  <MarginRight>10mm</MarginRight>" +
                          "  <MarginBottom>10mm</MarginBottom>" +
                          "</DeviceInfo>";
            string mimeType;
            string encoding;
            string fileNameExt;
            string[] outStreams;
            Warning[] warinngs;

            //report.Render returns a byte array of the PDF document
            report.Render("PDF", DeviceInfo, out mimeType, out encoding, out fileNameExt, out outStreams, out warinngs);*/




        }

        private void btnSelect_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _serviceFactory = new ServiceFactory();
            Customers = new List<Customer>();
            var orderClient = _serviceFactory.CreateClient<IOrderService>();
            Reservations = orderClient.GetReservationsByGroupId(Int32.Parse(tbGroupID.Text));
            var reservationsSource = new ReportDataSource { Name = "Reservation", Value = Reservations };
            _reportViewer.LocalReport.DataSources.Add(reservationsSource);
            Reservations.ToList().ForEach(reservation =>
            {
                Customers.AddRange(reservation.Customers);
            });
            var customerSource = new ReportDataSource { Name = "Customers", Value = Customers };
            _reportViewer.LocalReport.DataSources.Add(customerSource);
            _reportViewer.LocalReport.ReportPath = "..\\..\\RDLC\\ReservationReport.rdlc";
            _reportViewer.RefreshReport();
            //_reportViewer.LocalReport.ReportEmbeddedResource = "Report.rdlc";
            /*var report = new LocalReport();
            report.DataSources.Add(reservationsSource);
            report.ReportEmbeddedResource = "My.Assembly.ReportName.rdlc";
            const string DeviceInfo = "<DeviceInfo>" +
                          "  <OutputFormat>EMF</OutputFormat>" +
                          "  <PageWidth>210mm</PageWidth>" +
                          "  <PageHeight>297mm</PageHeight>" +
                          "  <MarginTop>10mm</MarginTop>" +
                          "  <MarginLeft>10mm</MarginLeft>" +
                          "  <MarginRight>10mm</MarginRight>" +
                          "  <MarginBottom>10mm</MarginBottom>" +
                          "</DeviceInfo>";
            string mimeType;
            string encoding;
            string fileNameExt;
            string[] outStreams;
            Warning[] warinngs;

            //report.Render returns a byte array of the PDF document
            report.Render("PDF", DeviceInfo, out mimeType, out encoding, out fileNameExt, out outStreams, out warinngs);*/

        }
    }


}
