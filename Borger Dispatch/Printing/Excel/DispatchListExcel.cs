using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingerDispatch.Printing.Excel
{
    public class DispatchListExcel : SingerExcelDocument
    {
        public override XLWorkbook GenerateExcel(string filename, object entity)
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Current Jobs");

            ws.Cell("A1").Value = "Current Dispatches - " + DateTime.Today.ToString(SingerConfigs.PrintedDateFormatString);
            ws.Cell("A1").Style.Font.Bold = true;

            InsertHeader(ws);
            FillTable(ws, (IEnumerable<Dispatch>)entity);

            ws.Columns().AdjustToContents();
            
            return wb;
        }

        private void InsertHeader(IXLWorksheet ws)
        {
            ws.Cell("A3").Value = "Company";
            ws.Cell("B3").Value = "Job #";
            ws.Cell("C3").Value = "Load #";
            ws.Cell("D3").Value = "Commodity";
            ws.Cell("E3").Value = "From";
            ws.Cell("F3").Value = "To";
            ws.Cell("G3").Value = "Dispatch #";
            ws.Cell("H3").Value = "Dispatch Date";            
            ws.Cell("I3").Value = "Equipment Type";
            ws.Cell("J3").Value = "Equipment";
            ws.Cell("K3").Value = "Employee";

            var rng = ws.Range("A3:K3").Style.Font.Bold = true;
        }

        private void FillTable(IXLWorksheet ws, IEnumerable<Dispatch> dispatches)
        {
            Company currentCompany = null;
            Job currentJob = null;
            Load currentLoad = null;
            Dispatch currentDispatch = null;
            var currentRow = 4;

            var size = dispatches.Count();

            for (var i = 0; i < size; i++)
            {
                currentDispatch = dispatches.ElementAt(i);

                currentLoad = currentLoad ?? currentDispatch.Load;
                currentJob = currentJob ?? currentLoad.Job;
                currentCompany = currentCompany ?? currentJob.Company;

                var dispatchDate = string.Format("{0} {1}", currentDispatch.MeetingDate.Value.ToString(SingerConfigs.PrintedDateFormatString), currentDispatch.MeetingTime);
                var dispatchNumber = currentDispatch.Number;
                var dispatchEquipType = (currentDispatch.EquipmentType == null ? "N/A" : currentDispatch.EquipmentType.Name);
                var dispatchEquipment = (currentDispatch.Equipment == null ? "N/A" : currentDispatch.Equipment.UnitNumber);                
                var dispatchEmployee = currentDispatch.Employee == null ? "Unknown employee" : currentDispatch.Employee.Name;

                
                var nextDispatch = dispatches.ElementAtOrDefault(i + 1);

                if (nextDispatch == null || nextDispatch.Load != currentLoad)
                {                    
                    foreach (var commodity in currentDispatch.Load.LoadedCommodities)
                    {
                        var loading = string.Format("{0} {1}", commodity.LoadLocation, commodity.LoadAddress).Trim();
                        var unloading = string.Format("{0} {1}", commodity.UnloadLocation, commodity.UnloadAddress).Trim();

                        ws.Cell(currentRow, "A").Value = currentCompany.Name;
                        ws.Cell(currentRow, "B").Value = currentJob.Number;
                        ws.Cell(currentRow, "C").Value = currentLoad.Number;
                        ws.Cell(currentRow, "D").Value = commodity.JobCommodity.NameAndUnit;
                        ws.Cell(currentRow, "E").Value = loading;
                        ws.Cell(currentRow, "F").Value = unloading;
                        ws.Cell(currentRow, "G").Value = dispatchNumber;
                        ws.Cell(currentRow, "H").Value = dispatchDate;
                        ws.Cell(currentRow, "I").Value = dispatchEquipType;
                        ws.Cell(currentRow, "J").Value = dispatchEquipment;
                        ws.Cell(currentRow, "K").Value = dispatchEmployee;

                        currentRow++;
                    }
                    
                    currentLoad = nextDispatch != null ? nextDispatch.Load : null;
                }

                if (nextDispatch == null || currentJob != nextDispatch.Load.Job)                    
                    currentJob = nextDispatch != null ? nextDispatch.Load.Job : null;

                if (nextDispatch == null || currentCompany != nextDispatch.Load.Job.Company)                    
                    currentCompany = nextDispatch != null ? nextDispatch.Load.Job.Company : null;
            }
        }
    }
}
