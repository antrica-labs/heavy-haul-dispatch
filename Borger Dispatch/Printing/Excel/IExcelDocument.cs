using ClosedXML.Excel;

namespace SingerDispatch.Printing.Excel
{
    public interface IExcelDocument
    {
        bool PrintMetric { get; set; }
        bool SpecializedDocument { get; set; }

        XLWorkbook GenerateExcel(string filename, object entity);
    }
}
