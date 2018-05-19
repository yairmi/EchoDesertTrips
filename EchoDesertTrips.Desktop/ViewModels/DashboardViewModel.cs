using Core.Common.Contracts;
using Core.Common.UI.Core;
using System.ComponentModel.Composition;
using System;
using SimpleWPFReporting;
using EchoDesertTrips.Client.Contracts;
using System.Linq;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using EchoDesertTrips.Client.Entities;
using System.Xml.XPath;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering;
using System.Diagnostics;

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DashboardViewModel : ViewModelBase
    {
        private IServiceFactory _serviceFactory;
        [ImportingConstructor]
        public DashboardViewModel(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
            PrintCommand = new DelegateCommand<object>(OnPrintCommand);
        }

        private void OnPrintCommand(object obj)
        {
            /*WithClient(_serviceFactory.CreateClient<IOrderService>(), orderClient =>
            {
                var reservation = orderClient.GetReservationByGroupId(_groupId);

                string fileName = @"D:\DocXExample.docx";
                var doc = DocX.Create(fileName);

                var JordanId = string.Format("Jordan ID: {0}", reservation.GroupID);
                doc.InsertParagraph(JordanId);
                Table table = doc.InsertTable(reservation.Customers.Count, 8);
                table.TableCaption = "PAX";
                table.SetTableCellMargin(TableCellMarginType.top, 10);
                //Header
                table.Rows[0].Cells[0].Paragraphs.First().Append("First Name");
                table.Rows[0].Cells[1].Paragraphs.First().Append("Last Name");
                table.Rows[0].Cells[2].Paragraphs.First().Append("Date Of Birdth");
                table.Rows[0].Cells[3].Paragraphs.First().Append("Passport");
                table.Rows[0].Cells[4].Paragraphs.First().Append("Issue Date");
                table.Rows[0].Cells[5].Paragraphs.First().Append("Expiry Date");
                table.Rows[0].Cells[6].Paragraphs.First().Append("Nationality");
                table.Rows[0].Cells[7].Paragraphs.First().Append("Phone1");
                int index = 1;
                reservation.Customers.ForEach((customer) =>
                {
                    table.Rows[index].Cells[0].Paragraphs.First().Append(customer.FirstName);
                    table.Rows[index].Cells[1].Paragraphs.First().Append(customer.LastName);
                    table.Rows[index].Cells[2].Paragraphs.First().Append(customer.DateOfBirdth.ToShortTimeString());
                    table.Rows[index].Cells[3].Paragraphs.First().Append(customer.PassportNumber);
                    table.Rows[index].Cells[4].Paragraphs.First().Append(customer.IssueData.ToShortTimeString());
                    table.Rows[index].Cells[5].Paragraphs.First().Append(customer.ExpireyDate.ToShortTimeString());
                    table.Rows[index].Cells[6].Paragraphs.First().Append(customer.Nationality);
                    table.Rows[index].Cells[7].Paragraphs.First().Append(customer.Phone1);

                    index++;
                });
            doc.Save();
        });*/
            WithClient(_serviceFactory.CreateClient<IOrderService>(), orderClient =>
            {
                var reservations = orderClient.GetReservationsByGroupId(_groupId);
                Document document = new Document();
                document.Info.Title = "Reservation";
                document.Info.Subject = "Reservation Info.";
                document.Info.Author = "Yair Mitzner";
                CreatePage(document, reservations);
                //FillContent(reservations);

                PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
                renderer.Document = document;

                renderer.RenderDocument();

                string filename = "Reservation.pdf";
                try
                {
                    renderer.PdfDocument.Save(filename);
                }
                catch(Exception ex)
                {
                    log.Error("Failed to save " + filename + ". Exception: " + ex.Message);
                }

                Process.Start(filename);


            });

        }

        public DelegateCommand<object> PrintCommand { get; private set; }

        public override string ViewTitle => "Dashboard";

        protected override void OnViewLoaded()
        {
        }

        private int _groupId;

        public int GroupId
        {
            get
            {
                return _groupId;
            }
            set
            {
                _groupId = value;
                OnPropertyChanged(() => GroupId);
            }
        }

        private void CreatePage(Document document, Reservation[] reservations)
        {
            // Each MigraDoc document needs at least one section. 
            Section section = document.AddSection();

            // Create footer
            Paragraph paragraphFooter = section.Footers.Primary.AddParagraph();
            Font fontFooter = new Font("Arial", 8);
            fontFooter.Bold = true;
            paragraphFooter.AddFormattedText("Eco Desert Tours");
            paragraphFooter.Format.Alignment = ParagraphAlignment.Center;


            Paragraph paragraphHeader = section.Headers.Primary.AddParagraph();
            paragraphHeader.AddFormattedText("Jordan ID: " + reservations[0].GroupID);
            // Create the text frame for the address
            //addressFrame = section.AddTextFrame();
            //addressFrame.Height = "3.0cm";
            //addressFrame.Width = "7.0cm";
            //addressFrame.Left = ShapePosition.Left;
            //addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            //addressFrame.Top = "5.0cm";
            //addressFrame.RelativeVertical = RelativeVertical.Page;

            // Put sender in address frame
            //paragraph = addressFrame.AddParagraph("PowerBooks Inc · Sample Street 42 · 56789 Cologne");
            //paragraph.Format.Font.Name = "Times New Roman";
            //paragraph.Format.Font.Size = 7;
            //paragraph.Format.SpaceAfter = 3;

            // Add the print date field
            //paragraph = section.AddParagraph();
            //paragraph.Format.SpaceBefore = "8cm";
            //paragraph.Style = "Reference";
            //paragraph.AddFormattedText("RESERVATION", TextFormat.Bold);
            //paragraph.AddTab();
            //paragraph.AddText("Report Issue Day, ");
            //paragraph.AddDateField("dd.MM.yyyy");

            // Create the item table
            //table = section.AddTable();
            //table.Style = "Table";
            //table.Borders.Color = Colors.Black;
            //table.Borders.Width = 0.25;
            //table.Borders.Left.Width = 0.5;
            //table.Borders.Right.Width = 0.5;
            //table.Rows.LeftIndent = 0;

            // Before you can add a row, you must define the columns
            Font fontBigTitle = new Font("Arial", 10);
            fontBigTitle.Bold = true;
            Font fontTitle = new Font("Arial", 8);
            fontTitle.Bold = true;
            Font fontDescription = new Font("Arial", 7);

            reservations.ToList().ForEach((reservation) =>
            {
                //var reservationSection = document.AddSection();

                // Add the print date field
                var paragraph = section.AddParagraph();
                paragraph.Format.SpaceBefore = "2cm";
                //paragraph.Style = "Reference";
                paragraph.AddFormattedText("Tours", fontBigTitle);
                paragraph.AddLineBreak();
                paragraph.AddLineBreak();
                reservation.Tours.ForEach((tour) =>
                {
                    paragraph.AddFormattedText(tour.TourType.TourTypeName, fontTitle);
                    paragraph.AddLineBreak();
                    paragraph.AddLineBreak();
                    int dayIndex = 1;
                    tour.TourType.TourTypeDescriptions.ForEach((description) =>
                    {
                        var tourDescription = string.Format("Day {0}: {1}", dayIndex++, description.Description);
                        paragraph.AddFormattedText(tourDescription, fontDescription);
                        paragraph.AddLineBreak();
                        paragraph.AddLineBreak();
                    });
                    paragraph.AddLineBreak();
                    paragraph.AddLineBreak();
                });
                //var paragraphCustomers = reservationSection.AddParagraph();
                paragraph.AddFormattedText("Reservation Customers", fontBigTitle);
                paragraph.AddLineBreak();
                paragraph.AddLineBreak();
                Table customersTable = section.AddTable();
                DefineTable(customersTable);
                FillContent(customersTable, reservation);
                section = document.AddSection();
            });

            //row = table.AddRow();
            //row.HeadingFormat = true;
            //row.Format.Alignment = ParagraphAlignment.Center;
            //row.Format.Font.Bold = true;
            //row.Shading.Color = TableBlue;
            //row.Cells[1].AddParagraph("Quantity");
            //row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            //row.Cells[2].AddParagraph("Unit Price");
            //row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            //row.Cells[3].AddParagraph("Discount (%)");
            //row.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            //row.Cells[4].AddParagraph("Taxable");
            //row.Cells[4].Format.Alignment = ParagraphAlignment.Left;


        }

        private void FillContent(Table table, Reservation reservation)
        {
            // Fill address in address text frame
            /*XPathNavigator item = SelectItem("/invoice/to");
            Paragraph paragraph = addressFrame.AddParagraph();
            paragraph.AddText(GetValue(item, "name/singleName"));
            paragraph.AddLineBreak();
            paragraph.AddText(GetValue(item, "address/line1"));
            paragraph.AddLineBreak();
            paragraph.AddText(GetValue(item, "address/postalCode") + " " + GetValue(item, "address/city"));*/

            // Iterate the invoice items
            //double totalExtendedPrice = 0;
            //XPathNodeIterator iter = this.navigator.Select("/invoice/items/*");
            int index = 1;
            reservation.Customers.ForEach((customer) =>
            {
                // Each item fills two rows
                Row row1 = table.AddRow();
                row1.TopPadding = 1.5;
                row1.Cells[0].Shading.Color = Colors.Gray;
                row1.Cells[0].VerticalAlignment = VerticalAlignment.Center;
                //row1.Cells[0].MergeDown = 1;
                //row1.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                //row1.Cells[1].MergeRight = 3;
                row1.Cells[1].Shading.Color = Colors.Gray;
                row1.Cells[1].VerticalAlignment = VerticalAlignment.Center;
                //row1.Cells[5].MergeDown = 1;
                row1.Cells[2].Shading.Color = Colors.Gray;
                row1.Cells[2].VerticalAlignment = VerticalAlignment.Center;

                row1.Cells[3].Shading.Color = Colors.Gray;
                row1.Cells[3].VerticalAlignment = VerticalAlignment.Center;

                row1.Cells[4].Shading.Color = Colors.Gray;
                row1.Cells[4].VerticalAlignment = VerticalAlignment.Center;

                row1.Cells[5].Shading.Color = Colors.Gray;
                row1.Cells[5].VerticalAlignment = VerticalAlignment.Center;

                row1.Cells[6].Shading.Color = Colors.Gray;
                row1.Cells[6].VerticalAlignment = VerticalAlignment.Center;
                //row1.Cells[0].AddParagraph(GetValue(item, "itemNumber"));
                var paragraph = row1.Cells[0].AddParagraph();
                paragraph.Format.Font.Size = fontSize;
                paragraph.AddFormattedText(index.ToString(), TextFormat.Bold);
                index++;
                paragraph = row1.Cells[1].AddParagraph();
                paragraph.Format.Font.Size = fontSize;
                paragraph.AddFormattedText(string.Format("{0} {1}", customer.FirstName, customer.LastName), TextFormat.Bold);

                paragraph = row1.Cells[2].AddParagraph();
                paragraph.Format.Font.Size = fontSize;
                paragraph.AddFormattedText(customer.DateOfBirdth.ToShortDateString(), TextFormat.Bold);

                paragraph = row1.Cells[3].AddParagraph();
                paragraph.Format.Font.Size = fontSize;
                paragraph.AddFormattedText(customer.PassportNumber, TextFormat.Bold);

                paragraph = row1.Cells[4].AddParagraph();
                paragraph.Format.Font.Size = fontSize;
                paragraph.AddFormattedText(customer.IssueData.ToShortDateString(), TextFormat.Bold);

                paragraph = row1.Cells[5].AddParagraph();
                paragraph.Format.Font.Size = fontSize;
                paragraph.AddFormattedText(customer.ExpireyDate.ToShortDateString(), TextFormat.Bold);

                paragraph = row1.Cells[6].AddParagraph();
                paragraph.Format.Font.Size = fontSize;
                paragraph.AddFormattedText(customer.Phone1, TextFormat.Bold);

                table.SetEdge(0, table.Rows.Count - 2, 6, 2, Edge.Box, BorderStyle.Single, 0.75);
            });
            // Add the notes paragraph
            //paragraph = this.document.LastSection.AddParagraph();
            //paragraph.Format.SpaceBefore = "1cm";
            //paragraph.Format.Borders.Width = 0.75;
            //paragraph.Format.Borders.Distance = 3;
            //paragraph.Format.Borders.Color = TableBorder;
            //paragraph.Format.Shading.Color = TableGray;
            //item = SelectItem("/invoice");
            //paragraph.AddText(GetValue(item, "notes"));
        }

        private void DefineTable(Table table)
        {
            
            table.Style = "Table";
            table.Borders.Color = Colors.Black;
            table.Borders.Width = 0.25;
            table.Borders.Left.Width = 0.5;
            table.Borders.Right.Width = 0.5;
            table.Rows.LeftIndent = 0;

            // Before you can add a row, you must define the columns
            Column column = table.AddColumn("1cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Left;

            column = table.AddColumn("2cm");
            column.Format.Alignment = ParagraphAlignment.Left;

            column = table.AddColumn("2cm");
            column.Format.Alignment = ParagraphAlignment.Left;

            column = table.AddColumn("2cm");
            column.Format.Alignment = ParagraphAlignment.Left;

            column = table.AddColumn("2cm");
            column.Format.Alignment = ParagraphAlignment.Left;

            column = table.AddColumn("2cm");
            column.Format.Alignment = ParagraphAlignment.Left;

            // Create the header of the table
            Row row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Shading.Color = Colors.Blue;
            row.Cells[0].AddParagraph("#");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Center;

            row.Cells[1].AddParagraph("Customer Name");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Center;

            row.Cells[2].AddParagraph("Date Of Birdth");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[2].VerticalAlignment = VerticalAlignment.Center;

            row.Cells[3].AddParagraph("Passport");
            row.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[3].VerticalAlignment = VerticalAlignment.Center;

            row.Cells[4].AddParagraph("Issue Date");
            row.Cells[4].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[4].VerticalAlignment = VerticalAlignment.Center;

            row.Cells[5].AddParagraph("Expiry Date");
            row.Cells[5].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[5].VerticalAlignment = VerticalAlignment.Center;

            row.Cells[6].AddParagraph("Phone");
            row.Cells[6].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[6].VerticalAlignment = VerticalAlignment.Center;

            table.SetEdge(0, 0, 7, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
        }

        private const int fontSize = 7;

    }
}
