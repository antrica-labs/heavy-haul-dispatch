using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingerDispatch.Printing.Documents
{
    class OutOfProvinceReportDocument : SingerPrintDocument
    {
        public OutOfProvinceReportDocument()
        {
            PrintMetric = true;
            SpecializedDocument = true;
        }

        public override string GenerateHTML(object entity)
        {
            return "Not implemented yet";
        }
    }
}
