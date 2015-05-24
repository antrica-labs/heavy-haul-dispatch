using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingerDispatch.Printing.Excel
{
    public abstract class SingerExcelDocument : IExcelDocument
    {
        public bool PrintMetric { get; set; }
        public bool SpecializedDocument { get; set; }

        public abstract XLWorkbook GenerateExcel(string filename, object entity);
    }
}
