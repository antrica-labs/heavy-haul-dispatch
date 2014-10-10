using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingerDispatch.Printing.Documents
{
    class JobListDocument : SingerPrintDocument
    {
        public JobListDocument()
        {
            PrintMetric = true;
            SpecializedDocument = true;
        }

        public override string GenerateHTML(object entity)
        {
            return GenerateHTML((IEnumerable<Job>)entity);
        }

        public string GenerateHTML(IEnumerable<Job> jobs)
        {
            var content = new StringBuilder();

            content.Append(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd""><html>");
            content.Append("<head>");
            content.Append(@"<meta http-equiv=""Content-Type"" content=""text/html;charset=utf-8"">");
            content.Append(GetTitle("Job List"));
            content.Append(GetStyles());
            content.Append("</head>");
            content.Append("<body>");
            content.Append(@"<div class=""job_list"">");
            content.Append(@"<h1>Job List - " + DateTime.Today.ToString(SingerConfigs.PrintedDateFormatString) + "</h1>");
            content.Append(GetBodyHTML(jobs));
            content.Append("</div>");
            content.Append("</body>");
            content.Append("</html>");

            return content.ToString();
        }

        private string GetBodyHTML(IEnumerable<Job> jobs)
        {
            var table = @"
                <table class=""jobs"">
                    <thead>
                        <tr>
                            <th class=""jobid"">Job ID</th>
                            <th class=""company"">Company</th>
                            <th class=""careof"">Care of Company</th>
                            <th class=""project"">Project name</th>                                    
                        </tr>
                    </thead>
                    <tbody>
                        {0}
                    </tbody>                            
                </table>
            ";
            var row = @"
                <tr {4}>
                    <td class=""jobid"">{0}</td>
                    <td class=""company"">{1}</td>
                    <td class=""careof"">{2}</td>
                    <td class=""project"">{3}</td>
                </tr>
            ";

            var rows = new StringBuilder();
            var total = jobs.Count();
            var i = 0;

            foreach (var job in jobs)
            {
                var replacements = new String[5];

                var company = job.Company.Name;
                var careof = (job.CareOfCompany != null) ? job.CareOfCompany.Name : "";
                
                if (!string.IsNullOrEmpty(job.Company.AccPacVendorCode))
                    company = string.Format("{0} ({1})", company, job.Company.AccPacVendorCode);

                if (job.CareOfCompany != null && !string.IsNullOrEmpty(job.CareOfCompany.AccPacVendorCode))
                    careof = string.Format("{0} ({1})", careof, job.CareOfCompany.AccPacVendorCode);

                replacements[0] = job.Number.ToString();
                replacements[1] = company;
                replacements[2] = careof;
                replacements[3] = job.Name;

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

                    table.jobs
                    {
                        width: 100%;
                        border-collapse: collapse;
                    }
                    
                    table.jobs td, table.jobs th
                    {
                        text-align: left;
                    }
                    
                    table.jobs th
                    {
                        border-top: 1px #000000 solid;
                        border-bottom: 1px #000000 solid;
                        padding: 0.2em 0;
                    }
                    
                    table.jobs td                    
                    {
                        border-bottom: 1px #000000 dotted;
                        padding: 0.5em 0.5em 0.5em 0;
                    }
                    
                    table.jobs tr.last td
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
