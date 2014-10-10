using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingerDispatch.Printing.Documents
{
    class QuoteListDocument : SingerPrintDocument
    {
        public QuoteListDocument()
        {
            PrintMetric = true;
            SpecializedDocument = true;
        }

        public override string GenerateHTML(object entity)
        {
            return GenerateHTML((IEnumerable<Quote>)entity);
        }

        public string GenerateHTML(IEnumerable<Quote> quotes)
        {
            var content = new StringBuilder();

            content.Append(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd""><html>");
            content.Append("<head>");
            content.Append(@"<meta http-equiv=""Content-Type"" content=""text/html;charset=utf-8"">");
            content.Append(GetTitle("Quote List"));
            content.Append(GetStyles());
            content.Append("</head>");
            content.Append("<body>");
            content.Append(@"<div class=""quote_list"">");
            content.Append(@"<h1>Quote List - " + DateTime.Today.ToString(SingerConfigs.PrintedDateFormatString) + "</h1>");
            content.Append(GetBodyHTML(quotes));
            content.Append("</div>");
            content.Append("</body>");
            content.Append("</html>");

            return content.ToString();
        }

        private string GetBodyHTML(IEnumerable<Quote> quotes)
        {
            var table = @"
                <table class=""items"">
                    <thead>
                        <tr>
                            <th class=""quoteid"">Quote ID</th>
                            <th class=""company"">Company</th>
                            <th class=""careof"">Care of Company</th>
                            <th class=""price"">Price</th>
                            <th class=""commodities"">Commodities</th>                                                              
                        </tr>
                    </thead>
                    <tbody>
                        {0}
                    </tbody>                            
                </table>
            ";
            var row = @"
                <tr {0}>
                    <td class=""quoteid"">{1}</td>
                    <td class=""company"">{2}</td>
                    <td class=""careof"">{3}</td>
                    <td class=""price"">{4}</td>
                    <td class=""commodites"">{5}</td>
                </tr>
            ";

            var rows = new StringBuilder();
            var total = quotes.Count();
            var i = 0;

            foreach (var quote in quotes)
            {
                var replacements = new String[6];

                var company = quote.Company.Name;
                var careof = (quote.CareOfCompany != null) ? quote.CareOfCompany.Name : "";

                if (!string.IsNullOrEmpty(quote.Company.AccPacVendorCode))
                    company = string.Format("{0} ({1})", company, quote.Company.AccPacVendorCode);

                if (quote.CareOfCompany != null && !string.IsNullOrEmpty(quote.CareOfCompany.AccPacVendorCode))
                    careof = string.Format("{0} ({1})", careof, quote.CareOfCompany.AccPacVendorCode);

                replacements[1] = quote.NumberAndRev;
                replacements[2] = company;
                replacements[3] = careof;
                replacements[4] = string.Format("{0:C}", quote.Price);
                replacements[5] = Quote.PrintCommodityList(quote);

                if (i == total - 1)
                    replacements[0] = @"class=""last""";
                else
                    replacements[0] = "";

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

                    table.items
                    {
                        width: 100%;
                        border-collapse: collapse;
                    }
                    
                    table.items td, table.jobs th
                    {
                        text-align: left;
                    }
                    
                    table.items th
                    {
                        border-top: 1px #000000 solid;
                        border-bottom: 1px #000000 solid;
                        padding: 0.2em 0;
                    }
                    
                    table.items td                    
                    {
                        border-bottom: 1px #000000 dotted;
                        padding: 0.5em 0.5em 0.5em 0;
                    }
                    
                    table.items tr.last td
                    {
                        border-bottom: none;
                    }

                    table.items td.quoteid
                    {
                        white-space: nowrap;
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
}
