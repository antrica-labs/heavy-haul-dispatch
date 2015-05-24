using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingerDispatch.Printing.Excel
{
    public class JobListExcel : SingerExcelDocument
    {
        public override XLWorkbook GenerateExcel(string filename, object entity)
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("All Jobs");

            ws.Cell("A1").Value = "All Jobs - " + DateTime.Today.ToString(SingerConfigs.PrintedDateFormatString);
            ws.Cell("A1").Style.Font.Bold = true;

            InsertHeader(ws);
            FillTable(ws, (IEnumerable<Job>)entity);

            ws.Columns().AdjustToContents();
            return wb;
        }

        private void InsertHeader(IXLWorksheet ws)
        {
            ws.Cell("A3").Value = "Job #";
            ws.Cell("B3").Value = "Commodity";
            ws.Cell("C3").Value = "Care of";
            ws.Cell("D3").Value = "Project Name";
            ws.Cell("E3").Value = "Commodities";

            var rng = ws.Range("A3:E3").Style.Font.Bold = true;
        }

        private void FillTable(IXLWorksheet ws, IEnumerable<Job> jobs)
        {
            var row = 4;

            foreach (var job in jobs)
            {
                var company = job.Company.Name;
                var careof = (job.CareOfCompany != null) ? job.CareOfCompany.Name : "";

                if (!string.IsNullOrEmpty(job.Company.AccPacVendorCode))
                    company = string.Format("{0} ({1})", company, job.Company.AccPacVendorCode);

                if (job.CareOfCompany != null && !string.IsNullOrEmpty(job.CareOfCompany.AccPacVendorCode))
                    careof = string.Format("{0} ({1})", careof, job.CareOfCompany.AccPacVendorCode);

                ws.Cell(row, "A").Value = job.Number;
                ws.Cell(row, "B").Value = company;
                ws.Cell(row, "C").Value = careof;
                ws.Cell(row, "D").Value = job.Name;
                ws.Cell(row, "E").Value = Job.PrintCommodityList(job);

                row++;
            }
        }
    }
}
