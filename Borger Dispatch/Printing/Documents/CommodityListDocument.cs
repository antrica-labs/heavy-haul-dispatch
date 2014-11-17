using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingerDispatch.Printing.Documents
{
    class CommodityListDocument : SingerPrintDocument
    {
        public CommodityListDocument()
        {
            PrintMetric = true;
            SpecializedDocument = true;
        }

        public override string GenerateHTML(object entity)
        {
            return GenerateHTML((Company)entity);
        }

        public string GenerateHTML(Company company)
        {
            var content = new StringBuilder();

            content.Append(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd""><html>");
            content.Append("<head>");
            content.Append(@"<meta http-equiv=""Content-Type"" content=""text/html;charset=utf-8"">");
            content.Append(GetTitle("Commodity List"));
            content.Append(GetStyles());
            content.Append("</head>");
            content.Append("<body>");
            content.Append(@"<div class=""commodity_list"">");
            content.Append(@"<h1>" + company.Name +  " - Commodity List - " + DateTime.Today.ToString(SingerConfigs.PrintedDateFormatString) + "</h1>");
            content.Append(GetBodyHTML(company));
            content.Append("</div>");
            content.Append("</body>");
            content.Append("</html>");

            return content.ToString();
        }

        private string GetBodyHTML(Company company)
        {
            var table = @"
                <table class=""commodities"">
                    <thead>
                        <tr>
                            <th class=""unit"">Unit #</th>
                            <th class=""commodity"">Commodity</th>
                            <th class=""serial"">Serial</th>
                            <th class=""location"">Last known location</th>                            
                        </tr>
                    </thead>
                    <tbody>
                        {0}
                    </tbody>                            
                </table>
            ";
            var row = @"
                <tr {4}>
                    <td class=""unit"">{0}</td>
                    <td class=""commodity"">{1}</td>
                    <td class=""serial"">{2}</td>
                    <td class=""location"">{3}</td>
                </tr>
            ";

            var rows = new StringBuilder();
            var total = company.Commodities.Count();
            var i = 0;

            foreach (var commodity in company.Commodities)
            {
                var replacements = new String[5];
                string location;
                
                commodity.LastLocation = (commodity.LastLocation ?? "").Trim();
                commodity.LastAddress = (commodity.LastAddress ?? "").Trim();

                if (commodity.LastLocation.Length > 0 && commodity.LastAddress.Length > 0)
                    location = string.Format("{0} - {1}", commodity.LastLocation, commodity.LastAddress);
                else
                    location = string.Format("{0} {1}", commodity.LastLocation, commodity.LastAddress).Trim();


                replacements[0] = commodity.Unit;
                replacements[1] = commodity.Name;
                replacements[2] = commodity.Serial;
                replacements[3] = location;

                if (i == total - 1)
                    replacements[4] = @"class=""last""";
                else
                    replacements[4] = "";

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

                    table.commodities
                    {
                        width: 100%;
                        border-collapse: collapse;
                    }
                    
                    table.commodities td, table.commodities th
                    {
                        text-align: left;
                    }
                    
                    table.commodities th
                    {
                        border-top: 1px #000000 solid;
                        border-bottom: 1px #000000 solid;
                        padding: 0.2em 0;
                    }
                    
                    table.commodities td                    
                    {
                        border-bottom: 1px #000000 dotted;
                        padding: 0.5em 0.5em 0.5em 0;
                    }
                    
                    table.commodities tr.last td
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
}
