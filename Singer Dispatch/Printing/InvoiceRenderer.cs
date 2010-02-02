using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingerDispatch.Printing
{
    class InvoiceRenderer : Renderer
    {
        public string GenerateHTML(object invoice)
        {
            return GenerateHTML((Invoice)invoice);
        }

        public string GenerateHTML(Invoice invoice)
        {
            var content = new StringBuilder();

            content.Append("&nbsp;");

            return content.ToString();
        }

    }
}
