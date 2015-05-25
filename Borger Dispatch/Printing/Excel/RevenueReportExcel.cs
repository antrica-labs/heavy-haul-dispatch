using ClosedXML.Excel;
using SingerDispatch.Printing.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingerDispatch.Printing.Excel
{
    class RevenueReportExcel : SingerExcelDocument
    {
        public override XLWorkbook GenerateExcel(string filename, object entity)
        {
            var details = (RevenueReportDetails)entity;
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Invoices");

            ws.Cell("A1").Value = "Revenue Report - " + string.Format("{0} to {1}", details.StartDate.ToString(SingerConfigs.PrintedDateFormatString), details.EndDate.ToString(SingerConfigs.PrintedDateFormatString));
            ws.Cell("A1").Style.Font.Bold = true;

            InsertHeader(ws);
            FillTable(ws, details);

            ws.Columns().AdjustToContents();
            return wb;
        }

        private void InsertHeader(IXLWorksheet ws)
        {
            ws.Cell("A3").Value = "Invoice #";
            ws.Cell("B3").Value = "Company";
            ws.Cell("C3").Value = "Care of";
            ws.Cell("D3").Value = "Job #";
            ws.Cell("E3").Value = "Billed Hours";
            ws.Cell("F3").Value = "Subtotal";
            ws.Cell("G3").Value = "Tax rate";
            ws.Cell("H3").Value = "Total";

            // Bold the header
            ws.Range("A3:H3").Style.Font.Bold = true;            
        }

        private void FillTable(IXLWorksheet ws, RevenueReportDetails details)
        {           
            var row = 4;

            foreach (var invoice in details.Invoices)
            {
                var job = invoice.Job;
                var company = invoice.Company.Name;
                var careof = (job != null && job.CareOfCompany != null) ? job.CareOfCompany.Name : "";

                var tax = invoice.TaxRate ?? SingerConfigs.GST;
             
                invoice.UpdateTotalHours();
                invoice.UpdateTotalCost();

                ws.Cell(row, "A").Value = invoice.NumberAndRev;
                ws.Cell(row, "B").Value = company;
                ws.Cell(row, "C").Value = careof;
                ws.Cell(row, "D").Value = (job != null) ? job.Number.ToString() : "";
                ws.Cell(row, "E").Value = invoice.TotalHours;
                ws.Cell(row, "F").Value = invoice.TotalCost;
                ws.Cell(row, "G").Value = tax;
                ws.Cell(row, "H").Value = invoice.TotalCost * (1 + tax);
                               

                row++;
            }

            // Format numbers http://closedxml.codeplex.com/wikipage?title=NumberFormatId%20Lookup%20Table
            ws.Range(string.Format("F4:F{0}", row)).Style.NumberFormat.NumberFormatId = 7; // currency
            ws.Range(string.Format("G4:G{0}", row)).Style.NumberFormat.NumberFormatId = 9; // percent
            ws.Range(string.Format("H4:H{0}", row)).Style.NumberFormat.NumberFormatId = 7; // currency
        }
    }
}
