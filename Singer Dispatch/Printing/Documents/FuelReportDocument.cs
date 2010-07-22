using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingerDispatch.Printing.Documents
{
    class FuelReportDocument : IPrintDocument
    {
        public bool PrintMetric { get; set; }
        public bool SpecializedDocument { get; set; }
        
        public string GenerateHTML(object entity)
        {
            throw new NotImplementedException();
        }
    }
}
