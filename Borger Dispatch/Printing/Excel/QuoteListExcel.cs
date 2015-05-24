using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingerDispatch.Printing.Excel
{
    class QuoteListExcel : SingerExcelDocument
    {
        public override XLWorkbook GenerateExcel(string filename, object entity)
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("All Quotes");

            ws.Cell("A1").Value = "All Quotes - " + DateTime.Today.ToString(SingerConfigs.PrintedDateFormatString);
            ws.Cell("A1").Style.Font.Bold = true;

            InsertHeader(ws);
            FillTable(ws, (IEnumerable<Quote>)entity);

            ws.Columns().AdjustToContents();
            return wb;
        }

        private void InsertHeader(IXLWorksheet ws)
        {
            ws.Cell("A3").Value = "Quote #";
            ws.Cell("B3").Value = "Company";
            ws.Cell("C3").Value = "Care of";
            ws.Cell("D3").Value = "Price";
            ws.Cell("E3").Value = "Commodities";

            var rng = ws.Range("A3:E3").Style.Font.Bold = true;
        }

        private void FillTable(IXLWorksheet ws, IEnumerable<Quote> quotes)
        {
            var row = 4;

            foreach (var quote in quotes)
            {
                var company = quote.Company.Name;
                var careof = (quote.CareOfCompany != null) ? quote.CareOfCompany.Name : "";

                if (!string.IsNullOrEmpty(quote.Company.AccPacVendorCode))
                    company = string.Format("{0} ({1})", company, quote.Company.AccPacVendorCode);

                if (quote.CareOfCompany != null && !string.IsNullOrEmpty(quote.CareOfCompany.AccPacVendorCode))
                    careof = string.Format("{0} ({1})", careof, quote.CareOfCompany.AccPacVendorCode);

                ws.Cell(row, "A").Value = quote.NumberAndRev;
                ws.Cell(row, "B").Value = company;
                ws.Cell(row, "C").Value = careof;
                ws.Cell(row, "D").Value = string.Format("{0:C}", quote.Price);
                ws.Cell(row, "E").Value = Quote.PrintCommodityList(quote);
            }
        }
    }
}
