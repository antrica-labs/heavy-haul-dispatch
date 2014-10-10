using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SingerDispatch.Utils;

namespace SingerDispatch.Printing.Documents
{
    class StorageListDocument : SingerPrintDocument
    {
        public StorageListDocument()
        {
            PrintMetric = true;
            SpecializedDocument = true;
        }

        public override string GenerateHTML(object entity)
        {
            return GenerateHTML((IEnumerable<StorageItem>)entity);
        }

        public string GenerateHTML(IEnumerable<StorageItem> items)
        {
            var content = new StringBuilder();

            content.Append(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd""><html>");
            content.Append("<head>");
            content.Append(@"<meta http-equiv=""Content-Type"" content=""text/html;charset=utf-8"">");
            content.Append(GetTitle("Storage List"));
            content.Append(GetStyles());
            content.Append("</head>");
            content.Append("<body>");
            content.Append(@"<div class=""storage_list"">");
            content.Append(@"<h1>Storage List - " + DateTime.Today.ToString(SingerConfigs.PrintedDateFormatString) + "</h1>");
            content.Append(GetBodyHTML(items));
            content.Append("</div>");
            content.Append("</body>");
            content.Append("</html>");

            return content.ToString();
        }

        private string GetBodyHTML(IEnumerable<StorageItem> items)
        {
            var table = @"
                <table class=""items"">
                    <thead>
                        <tr>                            
                            <th class=""company"">Company</th>
                            <th class=""contact"">Contact</th>
                            <th class=""arrival"">Arrival</th>
                            <th class=""depart"">Depart</th>
                            <th class=""commodity"">Commodity</th>
                            <th class=""dimensions"">Dimensions</th>
                            <th class=""rate"">Rate</th>
                            <th class=""job"">Job</th>
                        </tr>
                    </thead>
                    <tbody>
                        {0}
                    </tbody>                            
                </table>
            ";
            var row = @"
                <tr {0}>
                    <td class=""company"">{1}</td>
                    <td class=""contact"">{2}</td>
                    <td class=""arrival"">{3}</td>
                    <td class=""depart"">{4}</td>
                    <td class=""commodity"">{5}</td>
                    <td class=""dimensions"">{6}</td>
                    <td class=""rate"">{7}</td>
                    <td class=""job"">{8}</td>
                </tr>
            ";

            var rows = new StringBuilder();
            var total = items.Count();
            var i = 0;

            foreach (var item in items)
            {
                if (item.JobCommodity == null) continue;

                var replacements = new String[9];
                var units = (PrintMetric != true) ? MeasurementFormater.UFeet : MeasurementFormater.UMetres;
                var weights = (PrintMetric != true) ? MeasurementFormater.UPounds : MeasurementFormater.UKilograms;

                var company = (item.JobCommodity.Owner != null) ? item.JobCommodity.Owner.Name : item.Job.Company.Name;
                var contact = (item.Contact != null) ? string.Format("{0}<br>{1}", item.Contact.Name, item.Contact.PrimaryPhone) : "";
                var arrival = (item.DateEntered != null) ? item.DateEntered.Value.ToString(SingerConfigs.PrintedDateFormatString) : "";
                var depart = (item.DateRemoved != null) ? item.DateRemoved.Value.ToString(SingerConfigs.PrintedDateFormatString) : "";
                var commodity = item.JobCommodity.NameAndUnit;
                var dimensions = string.Format("{0} x {1} x {2}<br>{3}", new object[] { MeasurementFormater.FromMetres(item.JobCommodity.Length, units), MeasurementFormater.FromMetres(item.JobCommodity.Width, units), MeasurementFormater.FromMetres(item.JobCommodity.Height, units), MeasurementFormater.FromKilograms(item.JobCommodity.Weight, weights) });
                var rate = (item.BillingInterval == null) ? string.Format("{0:C}&nbsp;", item.BillingRate) : string.Format("{0:C} ({1})", item.BillingRate, item.BillingInterval.Name);
                var job = item.Job.Number.ToString();

                replacements[0] = (i == total - 1) ? @"class=""last""" : "";
                replacements[1] = company;
                replacements[2] = contact;
                replacements[3] = arrival;
                replacements[4] = depart;
                replacements[5] = commodity;
                replacements[6] = dimensions;
                replacements[7] = rate;
                replacements[8] = job;
                
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
                    
                    table.items td, table.items th
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
