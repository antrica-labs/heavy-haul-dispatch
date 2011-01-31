using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SingerDispatch.Utils;

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
            return GenerateHTML((OPReportDetails)entity);
        }

        public string GenerateHTML(OPReportDetails details)
        {
            var content = new StringBuilder();

            content.Append(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd""><html>");
            content.Append("<head>");
            content.Append(@"<meta http-equiv=""Content-Type"" content=""text/html;charset=utf-8"">");
            content.Append(GetTitle("Out of Province Report"));
            content.Append(GetStyles());
            content.Append("</head>");
            content.Append("<body>");
            content.Append(@"<div class=""distance_list"">");
            content.Append(@"<h1>Out of Province Report - " + string.Format("{0} to {1}", details.StartDate.ToString(SingerConfigs.PrintedDateFormatString), details.EndDate.ToString(SingerConfigs.PrintedDateFormatString))  + "</h1>");
            content.Append(GetBodyHTML(details.Dispatches));
            content.Append("</div>");
            content.Append("</body>");
            content.Append("</html>");

            return content.ToString();
        }

        private static string GetTitle(string title)
        {
            return "<title>" + title + "</title>";
        }

        private string GetBodyHTML(IEnumerable<Dispatch> dispatches)
        {
            var table = @"
                <table class=""items"">
                    <thead>
                        <tr>
                            <th class=""unit"">Unit</th>
                            <th class=""job"">Job Num - Name</th>
                            <th class=""dispatch"">Load - Dispatch</th>
                            <th class=""province"">Province/State</th>
                            <th class=""distance"">Distance</th>                                    
                        </tr>
                    </thead>
                    <tbody>
                        {0}
                    </tbody>                            
                </table>
            ";
            var row = @"
                <tr {0}>
                    <td class=""unit"">{1}</td>
                    <td class=""job"">{2}</td>
                    <td class=""dispatch"">{3}</td>
                    <td class=""province"">{4}</td>
                    <td class=""distance"">{5}</td>
                </tr>
            ";

            var rows = new StringBuilder();
            var unit = (PrintMetric) ? MeasurementFormater.UKilometres : MeasurementFormater.UMiles;

            foreach (var dispatch in dispatches)
            {
                var count = 0;

                foreach (var op in dispatch.OutOfProvinceTravels)
                {
                    var replacements = new object[6];

                    replacements[1] = (dispatch.Equipment != null) ? dispatch.Equipment.UnitNumber : "";
                    replacements[2] = (dispatch.Load != null && dispatch.Load.Job != null) ? dispatch.Load.Job.Number + " - " + dispatch.Load.Job.Name : "";
                    replacements[3] = (dispatch.Load != null) ? string.Format("{0} - {1}", dispatch.Load.Number, dispatch.Number) : dispatch.Number.ToString();
                    replacements[4] = (op.ProvinceOrState != null) ? op.ProvinceOrState.Name : "";
                    replacements[5] = MeasurementFormater.FromMetres(op.Distance * 1000, unit);

                    rows.Append(string.Format(row, replacements));

                    count++;
                }

                if (count == 0) // An out of province load exists for this job, but no distance has been recorded... print something to show this.
                {
                    var replacements = new object[6];

                    replacements[1] = (dispatch.Equipment != null) ? dispatch.Equipment.UnitNumber : "";
                    replacements[2] = (dispatch.Load != null && dispatch.Load.Job != null) ? dispatch.Load.Job.Number + " - " + dispatch.Load.Job.Name : "";
                    replacements[3] = (dispatch.Load != null) ? string.Format("{0} - {1}", dispatch.Load.Number, dispatch.Number) : dispatch.Number.ToString();
                    replacements[4] = "Not recorded".ToUpper();
                    replacements[5] = "";

                    rows.Append(string.Format(row, replacements));
                }
            }

            return string.Format(table, rows.ToString());
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

    public class OPReportDetails
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<Dispatch> Dispatches { get; set; }
    }
}
