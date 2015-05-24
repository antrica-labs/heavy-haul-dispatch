using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingerDispatch.Printing.Excel
{
    class CommodityListExcel : SingerExcelDocument
    {
        public override XLWorkbook GenerateExcel(string filename, object entity)
        {
            var company = (Company)entity;
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Commoditites");

            ws.Cell("A1").Value = company.Name +  " - Commodity List - " + DateTime.Today.ToString(SingerConfigs.PrintedDateFormatString);
            ws.Cell("A1").Style.Font.Bold = true;

            InsertHeader(ws);
            FillTable(ws, company.Commodities);

            ws.Columns().AdjustToContents();
            return wb;
        }

        private void InsertHeader(IXLWorksheet ws)
        {
            ws.Cell("A3").Value = "Unit #";
            ws.Cell("B3").Value = "Commodity";
            ws.Cell("C3").Value = "Serial";
            ws.Cell("D3").Value = "Last Known Location";

            var rng = ws.Range("A3:D3").Style.Font.Bold = true;
        }

        private void FillTable(IXLWorksheet ws, IEnumerable<Commodity> commodities)
        {
            var row = 4;
            var total = commodities.Count();
            
            foreach (var commodity in commodities)
            {
                string location;

                commodity.LastLocation = (commodity.LastLocation ?? "").Trim();
                commodity.LastAddress = (commodity.LastAddress ?? "").Trim();

                if (commodity.LastLocation.Length > 0 && commodity.LastAddress.Length > 0)
                    location = string.Format("{0} - {1}", commodity.LastLocation, commodity.LastAddress);
                else
                    location = string.Format("{0} {1}", commodity.LastLocation, commodity.LastAddress).Trim();

                
                ws.Cell(row, "A").SetValue(commodity.Unit);
                ws.Cell(row, "B").SetValue(commodity.Name);
                ws.Cell(row, "C").SetValue(commodity.Serial);
                ws.Cell(row, "D").SetValue(location);

                row++;
            }
        }
    }
}
