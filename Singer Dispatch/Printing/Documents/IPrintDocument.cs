
namespace SingerDispatch.Printing.Documents
{
    public interface IPrintDocument
    {
        bool PrintMetric { get; set; }
        string GenerateHTML(object entity);        
    }
}


