using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingerDispatch.Printing.Documents
{
    class DispatchListDocument : SingerPrintDocument
    {
        public DispatchListDocument()
        {
            PrintMetric = true;
            SpecializedDocument = true;
        }

        public override string GenerateHTML(object entity)
        {
            return GenerateHTML((IEnumerable<Dispatch>)entity);
        }

        public string GenerateHTML(IEnumerable<Dispatch> dispatches)
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
            content.Append(@"<h1>Current Dispatches - " + DateTime.Today.ToString(SingerConfigs.PrintedDateFormatString) + "</h1>");
            content.Append(GetBodyHTML(dispatches));
            content.Append("</div>");
            content.Append("</body>");
            content.Append("</html>");

            return content.ToString();
        }

        private string GetBodyHTML(IEnumerable<Dispatch> dispatches)
        {
            var openingHtml = @"<div class=""jobs"">";
            var closingHtml = @"</div>";
            var companyHtml = @"<div class=""company""><span class=""company_name"">{0}</span>{1}</div>";
            var jobHtml = @"<div class=""job""><span class=""job"">{0}</span>{1}</div>";
            var loadHtml = @"<span class=""load"">{0}</span><div class=""load_details"">{1} <span class=""sub_heading"">Dispatches</span><ul class=""dispatches"">{2}</ul></div>";
            var commoditiesHtml = @"<ul class=""commodity_list"">{0}</ul>";
            var commodityHtml = @"<li><span class=""commodity"">{0}</span> : <span class=""location"">{1} to {2}</span></li>";
            var dispatchHtml = @"<li><span class=""dispatch_date"">{0}</span> <span class=""dispatch_name"">{1}</span> <span class=""dispatch_employee"">{2}</span></li>";
            
            var document = new StringBuilder();

            document.Append(openingHtml);

            var dispatchList = new StringBuilder();
            var loadList = new StringBuilder();
            var jobList = new StringBuilder();
            var companyList = new StringBuilder();

            Company currentCompany = null;
            Job currentJob = null;
            Load currentLoad = null;
            Dispatch currentDispatch = null;

            var size = dispatches.Count();

            for (var i = 0; i < size; i++)
            {
                currentDispatch = dispatches.ElementAt(i);

                currentLoad = currentLoad ?? currentDispatch.Load;
                currentJob = currentJob ?? currentLoad.Job;
                currentCompany = currentCompany ?? currentJob.Company;
                
                var dispatchDate = string.Format("{0} {1}", currentDispatch.MeetingDate.Value.ToString(SingerConfigs.PrintedDateFormatString), currentDispatch.MeetingTime);
                var dispatchName = string.Format("Dispatch #{0}: {1} - {2}", currentDispatch.Number, (currentDispatch.EquipmentType == null ? "N/A" : currentDispatch.EquipmentType.Name), (currentDispatch.Equipment == null ? "N/A" : currentDispatch.Equipment.UnitNumber));
                var dispatchEmployee = currentDispatch.Employee == null ? "Unknown employee" : currentDispatch.Employee.Name;

                dispatchList.Append(string.Format(dispatchHtml, dispatchDate, dispatchName, dispatchEmployee));

                var nextDispatch = dispatches.ElementAtOrDefault(i+1);
                
                if (nextDispatch == null || nextDispatch.Load != currentLoad)
                {
                    var commodityList = new StringBuilder();
                    foreach (var commodity in currentDispatch.Load.LoadedCommodities)
                    {
                        var loading = string.Format("{0} {1}", commodity.LoadLocation, commodity.LoadAddress).Trim();
                        var unloading = string.Format("{0} {1}", commodity.UnloadLocation, commodity.UnloadAddress).Trim();

                        commodityList.Append(string.Format(commodityHtml, commodity.JobCommodity.NameAndUnit, loading, unloading));
                    }

                    loadList.Append(string.Format(loadHtml, string.Format("Load #{0}", currentDispatch.Load.Number), string.Format(commoditiesHtml, commodityList.ToString()), dispatchList.ToString()));

                    dispatchList = new StringBuilder();
                    currentLoad = nextDispatch != null ? nextDispatch.Load : null;
                }

                if (nextDispatch == null || currentJob != nextDispatch.Load.Job)
                {
                    jobList.Append(string.Format(jobHtml, string.Format("{0} - {1}", currentDispatch.Load.Job.Number, currentDispatch.Load.Job.Name), loadList.ToString()));

                    loadList = new StringBuilder();
                    currentJob = nextDispatch != null ? nextDispatch.Load.Job : null;
                }

                if (nextDispatch == null || currentCompany != nextDispatch.Load.Job.Company)
                {
                    companyList.Append(string.Format(companyHtml, currentDispatch.Load.Job.Company.Name, jobList.ToString()));

                    jobList = new StringBuilder();
                    currentCompany = nextDispatch != null ? nextDispatch.Load.Job.Company : null;
                }
            }   

            document.Append(companyList);

            document.Append(closingHtml);

            return document.ToString();
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

                    div.company
                    {
                        border-top: 1px #000000 solid;   
                        margin-bottom: 2em;
                    }

                    span.company_name 
                    {
                        display: block;
                        font-size: 2em;
                        line-height: 1.2em;                     
                        font-weight: bold;
                    }

                    div.job
                    {
                        padding: 1em;
                    }

                    span.job
                    {
                        display: block;
                        font-size: 1.2em;
                        margin-bottom: 0.5em;
                        font-weight: bold;
                    }

                    span.load
                    {
                        display: block;
                        font-weight: bold;
                    }

                    div.load_details
                    {
                        padding: 0 1em;
                        padding-top: 1em;
                    }

                    ul.commodity_list
                    {
                        margin-bottom: 1em;
                    }

                    span.sub_heading
                    {
                        display: block;
                        font-weight: bold;
                    }

                    ul.dispatches
                    {
                        padding: 0.5em 1em;
                    }

                    ul.dispatches li 
                    {
                        margin-bottom: 1em;
                    }

                    ul.dispatches span.dispatch_name 
                    {
                        display: block;
                    }

                    ul.dispatches span.dispatch_date
                    {
                        display: block;
                        font-weight: bold;
                    }

                    ul.dispatches span.dispatch_employee
                    {
                        display: block;                
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
