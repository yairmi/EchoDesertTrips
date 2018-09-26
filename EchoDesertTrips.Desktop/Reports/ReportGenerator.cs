using System;
using System.Collections.Generic;
using System.Linq;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using EchoDesertTrips.Client.Entities;
using MigraDoc.Rendering;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace EchoDesertTrips.Desktop.Reports
{
    public class ReportGenerator
    {
        public ReportGenerator()
        {
            _document = new Document();
            _document.Info.Title = "Reservation";
            _document.Info.Subject = "Reservation Info";
            _document.Info.Author = "Yair Mitzner";
        }

        public void GenerateReport(Reservation[] reservations, int GroupID)
        {
            CreatePages(reservations);
            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
            renderer.Document = _document;

            renderer.RenderDocument();
            string filename = string.Format("Reservation_JordanID_{0}.pdf", GroupID);
            renderer.PdfDocument.Save(filename);
            Process.Start(filename);
        }

        private void CreatePages(Reservation[] reservations)
        {
            /*var _arial10 = new Font("Arial", 10);
            _arial10.Bold = true;
            var _arial8 = new Font("Arial", 8);
            _arial8.Bold = true;
            var _arial7 = new Font("Arial", 7);

            // Each MigraDoc document needs at least one section. 
            Section section = _document.AddSection();

            // Create footer
            Paragraph pFooter = section.Footers.Primary.AddParagraph();
            pFooter.AddFormattedText("Eco Desert Tours");
            pFooter.Format.Alignment = ParagraphAlignment.Center;

            Paragraph pHeader = section.Headers.Primary.AddParagraph();
            pHeader.AddFormattedText(string.Format("Jordan ID: {0} {1}",reservations[0].GroupID, reservations[0].Group.Updated == true? "(Updated)" : ""));

            // Before you can add a row, you must define the columns
            reservations.ToList().ForEach((reservation) =>
            {
                 // Add the print date field
                var paragraph = section.AddParagraph();
                paragraph.Format.SpaceBefore = "2cm";
                //paragraph.Style = "Reference";
                paragraph.AddFormattedText("Tours", _arial10);
                paragraph.AddLineBreak();
                paragraph.AddLineBreak();
                reservation.Tours.ForEach((tour) =>
                {
                    paragraph.AddFormattedText(tour.TourType.TourTypeName, _arial8);
                    paragraph.AddLineBreak();
                    paragraph.AddLineBreak();
                    int dayIndex = 1;
                    tour.TourType.TourTypeDescriptions.ForEach((description) =>
                    {
                        var tourDescription = string.Format("Day {0}: {1}", dayIndex++, description.Description);
                        paragraph.AddFormattedText(tourDescription, _arial7);
                        paragraph.AddLineBreak();
                        paragraph.AddLineBreak();
                    });
                    paragraph.AddLineBreak();
                    paragraph.AddLineBreak();
                });
                //var paragraphCustomers = reservationSection.AddParagraph();
                paragraph.AddFormattedText("Reservation Customers", _arial10);
                paragraph.AddLineBreak();
                paragraph.AddLineBreak();
                Table customersTable = section.AddTable();
                DefineTable(customersTable);
                FillContent(customersTable, reservation);
                section = _document.AddSection();
            });*/
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

        private void FillContent(Table table, Reservation reservation)
        {
            /*// Fill address in address text frame
            int index = 1;
            reservation.Customers.ForEach((customer) =>
            {
                // Each item fills two rows
                Row row1 = table.AddRow();
                row1.TopPadding = 1.5;
                row1.Cells[0].Shading.Color = Colors.Gray;
                row1.Cells[0].VerticalAlignment = VerticalAlignment.Center;

                row1.Cells[1].Shading.Color = Colors.Gray;
                row1.Cells[1].VerticalAlignment = VerticalAlignment.Center;

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

                var paragraph = row1.Cells[0].AddParagraph();
                paragraph.Format.Font.Size = _fontSizeSmall;
                paragraph.AddFormattedText(index.ToString(), TextFormat.Bold);
                index++;
                paragraph = row1.Cells[1].AddParagraph();
                paragraph.Format.Font.Size = _fontSizeSmall;
                paragraph.AddFormattedText(string.Format("{0} {1}", customer.FirstName, customer.LastName), TextFormat.Bold);

                paragraph = row1.Cells[2].AddParagraph();
                paragraph.Format.Font.Size = _fontSizeSmall;
                paragraph.AddFormattedText(customer.DateOfBirdth.ToShortDateString(), TextFormat.Bold);

                paragraph = row1.Cells[3].AddParagraph();
                paragraph.Format.Font.Size = _fontSizeSmall;
                paragraph.AddFormattedText(customer.PassportNumber, TextFormat.Bold);

                paragraph = row1.Cells[4].AddParagraph();
                paragraph.Format.Font.Size = _fontSizeSmall;
                paragraph.AddFormattedText(customer.IssueData.ToShortDateString(), TextFormat.Bold);

                paragraph = row1.Cells[5].AddParagraph();
                paragraph.Format.Font.Size = _fontSizeSmall;
                paragraph.AddFormattedText(customer.ExpireyDate.ToShortDateString(), TextFormat.Bold);

                paragraph = row1.Cells[6].AddParagraph();
                paragraph.Format.Font.Size = _fontSizeSmall;
                paragraph.AddFormattedText(customer.Phone1, TextFormat.Bold);

                table.SetEdge(0, table.Rows.Count - 2, 6, 2, Edge.Box, BorderStyle.Single, 0.75);
            });*/
        }

        private const int _fontSizeSmall = 7;
        private Document _document;
    }
}
