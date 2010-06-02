
namespace SingerDispatch.Printing.Documents
{
    public interface IPrintDocument
    {
        string GenerateHTML(object entity, bool metric);        
    }
}


