using ClosedXML.Excel;
using SingerDispatch.Printing.Documents;
using SingerDispatch.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingerDispatch.Printing.Excel
{
    class OutOfProvinceExcel : SingerExcelDocument
    {
        public override XLWorkbook GenerateExcel(string filename, object entity)
        {
            var details = (OPReportDetails)entity;
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Out of Province");

            ws.Cell("A1").Value = "Out of Province Report - "  + string.Format("{0} to {1}", details.StartDate.ToString(SingerConfigs.PrintedDateFormatString), details.EndDate.ToString(SingerConfigs.PrintedDateFormatString));
            ws.Cell("A1").Style.Font.Bold = true;

            InsertHeader(ws);
            FillTable(ws, details);

            ws.Columns().AdjustToContents();
            return wb;
        }

        private void InsertHeader(IXLWorksheet ws)
        {
            ws.Cell("A3").Value = "Unit";
            ws.Cell("B3").Value = "Job";
            ws.Cell("C3").Value = "Load - Dispatch";
            ws.Cell("D3").Value = "Province/State";
            ws.Cell("E3").Value = "Distance";

            

            var rng = ws.Range("A3:E3").Style.Font.Bold = true;
        }

        private void FillTable(IXLWorksheet ws, OPReportDetails details)
        {
            var row = 4;
            var unit = (PrintMetric) ? MeasurementFormater.UKilometres : MeasurementFormater.UMiles;

            foreach (var dispatch in details.Dispatches)
            {
                var count = 0;

                foreach (var op in dispatch.OutOfProvinceTravels)
                {
                    ws.Cell(row, "A").Value = (dispatch.Equipment != null) ? dispatch.Equipment.UnitNumber : "";
                    ws.Cell(row, "B").Value = (dispatch.Load != null && dispatch.Load.Job != null) ? dispatch.Load.Job.Number + " - " + dispatch.Load.Job.Name : "";
                    ws.Cell(row, "C").Value = (dispatch.Load != null) ? string.Format("'{0} - {1}", dispatch.Load.Number, dispatch.Number) : dispatch.Number.ToString();
                    ws.Cell(row, "D").Value = (op.ProvinceOrState != null) ? op.ProvinceOrState.Name : "";
                    ws.Cell(row, "E").Value = MeasurementFormater.FromMetres(op.Distance * 1000, unit);

                    row++;
                }

                if (count == 0) // An out of province load exists for this job, but no distance has been recorded... print something to show this.
                {
                    ws.Cell(row, "A").Value = (dispatch.Equipment != null) ? dispatch.Equipment.UnitNumber : "";
                    ws.Cell(row, "B").Value = (dispatch.Load != null && dispatch.Load.Job != null) ? dispatch.Load.Job.Number + " - " + dispatch.Load.Job.Name : "";
                    ws.Cell(row, "C").Value = (dispatch.Load != null) ? string.Format("'{0} - {1}", dispatch.Load.Number, dispatch.Number) : dispatch.Number.ToString();
                    ws.Cell(row, "D").Value = "Not recorded".ToUpper();
                    ws.Cell(row, "E").Value = "";

                    row++;
                }
            }
        }
    }
}
