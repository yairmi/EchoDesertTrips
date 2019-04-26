using Core.Common.Contracts;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using EchoDesertTrips.Client.Proxies;
using EchoDesertTrips.Desktop.Support;
using Microsoft.Reporting.WinForms;
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
        private IServiceFactory _serviceFactory;
        private IMessageDialogService _messageDialogService;
        private string BestRegardsFormat = "Best Regards - {0} - Desert Eco Tours";

        public DashboardView()
        {
            
            InitializeComponent();
        }

        private void btnCustomerReport_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Reservations == null)
            {
                Reservations = GetReservations();
                if (Reservations.Count() == 0)
                {
                    _messageDialogService = new MessageDialogService();
                    _messageDialogService.ShowInfoDialog("Group ID does not exist!", "Info");
                    Reservations = null;
                    return;
                }
            }
            List<CustomerForReport> Customers = new List<CustomerForReport>();
            foreach(var reservation in Reservations)
            {
                foreach (var customer in reservation.Customers)
                {
                    var customerForReport = new CustomerForReport
                    {
                        LastName = customer.LastName,
                        FirstName = customer.FirstName,
                        DateOfBirdth = customer.DateOfBirdth.ToShortDateString(),
                        Passport = customer.PassportNumber,
                        IssueDate = customer.IssueData.ToShortDateString(),
                        Expiry = customer.ExpireyDate.ToShortDateString(),
                        Nationality = customer.Nationality,
                        HasVisa = (customer.HasVisa == true ? "Free" : "No"),
                        AgeInDays = (reservation.Tours[0].StartDate - customer.DateOfBirdth).TotalDays
                    };
                    Customers.Add(customerForReport);
                }
            }
            var customersSource = new ReportDataSource { Name = "dsCustomerForReport", Value = Customers };

            _reportViewer.LocalReport.ReportPath = "..\\..\\RDLC\\CustomersReport.rdlc";
            _reportViewer.LocalReport.DataSources.Add(customersSource);
            _reportViewer.RefreshReport();
        }

        private void btnSelect_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Reservations == null)
            {
                Reservations = GetReservations();
                if (Reservations.Count() == 0)
                {
                    _messageDialogService = new MessageDialogService();
                    _messageDialogService.ShowInfoDialog("Group ID does not exist!", "Info");
                    Reservations = null;
                    return;
                }
            }
            var reservationArray = Reservations.ToArray();
            string days = string.Format("{0}", reservationArray[0].Tours[0].TourType.Days);
            string privateOrRegular = reservationArray[0].Tours[0].TourType.Private == true ? "Private" : "Regular";
            string tourDestination = reservationArray[0].Tours[0].TourType.TourTypeName;
            //Customers = new List<Customer>();//It includes the first customer of each reservation in the group
            var reservationsDataForReport = new List<ReservationGeneralData1ForReport>();
            foreach (var reservation in reservationArray)
            {
                var reservationForDataReport = new ReservationGeneralData1ForReport();
                reservationForDataReport.FirstCustomerInReservation = reservation.Customers[0].FullName;
                reservationForDataReport.Pax = reservation.ActualNumberOfCustomers;
                string resultStartDays, resultHotels, resultsRoomTypes;
                GetHotelsData(reservation, out resultStartDays, out resultHotels, out resultsRoomTypes);
                reservationForDataReport.HotelsStartDay = resultStartDays;
                reservationForDataReport.Hotels = resultHotels;
                reservationForDataReport.RoomTypes = resultsRoomTypes;
                reservationForDataReport.OperatorName = reservation.Operator.OperatorFullName;
                reservationsDataForReport.Add(reservationForDataReport);
            }



            var reservationsSource = new ReportDataSource { Name = "dsReservationGeneralData1ForReport", Value = reservationsDataForReport };
            _reportViewer.LocalReport.ReportPath = "..\\..\\RDLC\\GroupReport.rdlc";
            _reportViewer.LocalReport.DataSources.Add(reservationsSource);

            string stOptionals;
            GetOptionals(reservationArray, out stOptionals);

            ReportParameter[] p = new ReportParameter[]
            {
                new ReportParameter("pGroupID", "Reservation: " + reservationArray[0].GroupID.ToString()),
                new ReportParameter("pHiValue", "Hi: " + tbHiValue.Text),
                new ReportParameter("pStartDay", reservationArray[0].Tours[0].StartDate.ToShortDateString().ToString()),
                new ReportParameter("pText1", "Pls. provide " + days + " Days " + privateOrRegular + " tour to " + tourDestination),
                new ReportParameter("pPickupTimeValue", reservationArray[0].PickUpTime.ToString()),
                new ReportParameter("pBestRegards", string.Format(BestRegardsFormat, OperatorSingle.Instance.Operator.OperatorFullName)),
                new ReportParameter("pOptionals", stOptionals)
            };
            _reportViewer.LocalReport.SetParameters(p);
            _reportViewer.RefreshReport();
        }

        private Reservation[] GetReservations()
        {
            int nGroupID;
            int.TryParse(tbGroupID.Text, out nGroupID);
            if (nGroupID == 0)
                return null;
            _serviceFactory = new ServiceFactory();
            var orderClient = _serviceFactory.CreateClient<IOrderService>();
            return orderClient.GetReservationsByGroupId(nGroupID);
        }

        private void GetHotelsData(Reservation reservation, 
            out string resultStartDays, 
            out string resultHotels,
            out string resultsRoomTypes)
        {
            resultStartDays = string.Empty;
            resultHotels = string.Empty;
            resultsRoomTypes = string.Empty;

            foreach (var tour in reservation.Tours)
            {
                foreach (var tourHotel in tour.TourHotels)
                {
                    if (resultStartDays.Length > 0)
                    {
                        resultStartDays += "\n\n";
                    }
                    if (resultHotels.Length > 0)
                    {
                        resultHotels += "\n\n";
                    }
                    resultStartDays += tourHotel.HotelStartDay.ToShortDateString();
                    resultHotels += tourHotel.Hotel.HotelName;
                    foreach(var tourHotelRoomTypes in tourHotel.TourHotelRoomTypes)
                    {
                        if (resultsRoomTypes.Length > 0)
                        {
                            resultsRoomTypes += "\n\n";
                        }
                        resultsRoomTypes += tourHotelRoomTypes.HotelRoomType.RoomType.RoomTypeName + ", ";
                    }
                }
            }
        }

        private void GetOptionals(Reservation[] reservationArray, out string stOptionals)
        {
            var IDs = new List<int>();
            stOptionals = string.Empty;
            foreach (var reservation in reservationArray)
            {
                foreach(var tour in reservation.Tours)
                {
                    foreach(var tourOptional in tour.TourOptionals)
                    {
                        if (IDs.Exists(o => o.Equals(tourOptional.OptionalId)))
                            continue;
                        IDs.Add(tourOptional.OptionalId);
                        stOptionals += tourOptional.Optional.OptionalDescription += ", ";
                    }
                }
            }
        }
    }


}
