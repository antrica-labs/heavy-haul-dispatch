using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingerDispatch.Printing.Documents
{
    public class RevenueReportDocument : SingerPrintDocument
    {
        public RevenueReportDocument()
        {
            PrintMetric = true;
            SpecializedDocument = true;
        }

        public override string GenerateHTML(object entity)
        {
            return GenerateHTML((RevenueReportDetails)entity);
        }

        public string GenerateHTML(RevenueReportDetails details)
        {
            var content = new StringBuilder();

            content.Append(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd""><html>");
            content.Append("<head>");
            content.Append(@"<meta http-equiv=""Content-Type"" content=""text/html;charset=utf-8"">");
            content.Append(GetTitle("Invoice List"));
            content.Append(GetStyles());
            content.Append("</head>");
            content.Append("<body>");
            content.Append(@"<div class=""invoice_list"">");
            content.Append(@"<h1>Invoices - " + details.StartDate.ToString(SingerConfigs.PrintedDateFormatString) + " to " + details.EndDate.ToString(SingerConfigs.PrintedDateFormatString) + "</h1>");
            content.Append(GetBodyHTML(details.Invoices));
            content.Append("</div>");
            content.Append("</body>");
            content.Append("</html>");

            return content.ToString();
        }
            
        

        private string GetBodyHTML(IEnumerable<Invoice> invoices)
        {
            var table = @"
                <table class=""invoices"">
                    <thead>
                        <tr>
                            <th class=""invoice_num"">Invoice #</th>
                            <th class=""company"">Company</th>
                            <th class=""careof"">Care of Company</th>                            
                            <th class=""job_num"">Job #</th>                            
                            <th class=""hours"">Billed Hours</th> 
                            <th class=""subtotal"">Subtotal</th>
                            <th class=""tax"">Tax Rate</th>   
                            <th class=""total"">Total</th>   
                        </tr>
                    </thead>
                    <tbody>
                        {0}
                    </tbody>                            
                </table>
            ";
            var row = @"
                <tr {8}>
                    <td class=""invoice_num"">{0}</td>
                    <td class=""company"">{1}</td>
                    <td class=""careof"">{2}</td>
                    <td class=""job_num"">{3}</td>                    
                    <td class=""hours"">{4}</td>
                    <td class=""subtotal"">{5}</td>
                    <td class=""tax"">{6}</td>
                    <td class=""total"">{7}</td>
                </tr>
            ";

            var rows = new StringBuilder();
            var total = invoices.Count();
            var i = 0;

            foreach (var invoice in invoices)
            {
                var replacements = new String[9];

                var job = invoice.Job;
                var company = invoice.Company.Name;
                var careof = (job != null && job.CareOfCompany != null) ? job.CareOfCompany.Name : "";

                var tax = invoice.TaxRate ?? SingerConfigs.GST;
                tax = tax * 100;

                invoice.UpdateTotalHours();
                invoice.UpdateTotalCost();

                replacements[0] = invoice.NumberAndRev;
                replacements[1] = company;
                replacements[2] = careof;
                replacements[3] = (job != null) ? job.Number.ToString() : "";
                replacements[4] = string.Format("{0}", invoice.TotalHours);
                replacements[5] = string.Format("{0:C}", invoice.TotalCost);
                replacements[6] = string.Format("{0:0.##}%", tax);
                replacements[7] = string.Format("{0:C}", invoice.TotalCost * (1 + (tax / 100)));

                if (i == total - 1)
                    replacements[8] = @"class=""last""";
                else
                    replacements[8] = "";

                rows.Append(string.Format(row, replacements));

                i++;
            }

            return string.Format(table, rows.ToString());
        }

        private static string GetTitle(string title)
        {
            return "<title>" + title + "</title>";
        }

        private static string GetStyles()
        {
            const string content = @"
                <style type=""text/css"">
                    /***** RESET DEFAULT BROWSER STYLES *****/
                    html, body, div, span, applet, object, iframe,
                    h1, h2, h3, h4, h5, h6, p, blockquote, pre,
                    a, abbr, acronym, address, big, cite, code,
                    del, dfn, em, font, img, ins, kbd, q, s, samp,
                    small, strike, strong, sub, sup, tt, var,
                    b, u, i, center,
                    dl, dt, dd, ol, ul, li,
                    fieldset, form, label, legend,
                    table, caption, tbody, tfoot, thead, tr, th, td 
                    {
                        margin: 0;
                        padding: 0;
                        border: 0;
                        outline: 0;
                        font-size: 100%;
                        vertical-align: baseline;
                        background: transparent;
                    }
                    body 
                    {
                        line-height: 1.2em;                        
                    }
                    ol, ul 
                    {
                        list-style: none;
                    }
                    blockquote, q 
                    {
                        quotes: none;
                    }
                    blockquote:before, blockquote:after,
                    q:before, q:after 
                    {
                        content: '';
                        content: none;
                    }

                    /* remember to define focus styles! */
                    :focus 
                    {
                        outline: 0;
                    }

                    /* remember to highlight inserts somehow! */
                    ins
                    {
                        text-decoration: none;
                    }
                    del
                    {
                        text-decoration: line-through;
                    }

                    /***** QUOTE SPECIFIC STYLES *****/

                    body
                    {
                        font-size: 12px;
                        font-family: sans-serif;
                        padding: 1em;
                    }

                    h1
                    {
                        display: block;
                        width: 100%;
                        margin-bottom: 1.0em;
                        line-height: 1.2em;
                        font-weight: bold;
                        font-size: 1.5em;
                        text-align: center;
                    }

                    table.invoices
                    {
                        width: 100%;
                        border-collapse: collapse;
                    }
                    
                    table.invoices td, table.invoices th
                    {
                        text-align: left;
                    }
                    
                    table.invoices th
                    {
                        border-top: 1px #000000 solid;
                        border-bottom: 1px #000000 solid;
                        padding: 0.2em 0;
                    }
                    
                    table.invoices td                    
                    {
                        border-bottom: 1px #000000 dotted;
                        padding: 0.5em 0.5em 0.5em 0;
                    }
                    
                    table.invoices tr.last td
                    {
                        border-bottom: none;
                    }
                </style>
                <style type=""text/css"" media=""print"">
                     body
                    {
                    	font-size: 9pt;
                        padding: 0;
                    }
                </style>
            ";

            return content;
        }
    }

    public class RevenueReportDetails
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<Invoice> Invoices { get; set; }
    }
}
