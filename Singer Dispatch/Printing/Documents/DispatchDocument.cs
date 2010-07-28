﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data.Linq;
using SingerDispatch.Utils;

// 

namespace SingerDispatch.Printing.Documents
{
    class DispatchDocument : IPrintDocument
    {
        private const string PageBreak = @"<div class=""page_break""></div>";
        
        public bool IncludeFileCopy { get; set; }
        public bool PrintMetric { get; set; }
        public bool SpecializedDocument { get; set; }

        public DispatchDocument()
        {
            IncludeFileCopy = false;
            PrintMetric = true;
            SpecializedDocument = true;
        }

        public DispatchDocument(bool includeDriverCopy)
        {
            IncludeFileCopy = includeDriverCopy;
            PrintMetric = true;
            SpecializedDocument = true;
        }

        public string GenerateHTML(object dispatch)
        {
            if (dispatch is List<Dispatch>)
                return GenerateHTML((List<Dispatch>)dispatch);
            
            return GenerateHTML((Dispatch)dispatch);
        }

        private string GenerateHTML(Dispatch dispatch)
        {            
            var content = new StringBuilder();

            content.Append(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd"">");
            content.Append("<html>");
            content.Append("<head>");
            content.Append(@"<meta http-equiv=""Content-Type"" content=""text/html;charset=utf-8"">");
            content.Append(GetTitle("Singer Specialized - Dispatch"));
            content.Append(GetStyles());
            content.Append("</head>");
            content.Append("<body>");

            content.Append(FillDispatchBody(dispatch, "Driver Copy"));            
            
            if (IncludeFileCopy == true)
            {
                content.Append(PageBreak);
                content.Append(FillDispatchBody(dispatch, "File Copy"));
            }

            content.Append("</body>");
            content.Append("</html>");

            return content.ToString();
        }

        private string GenerateHTML(List<Dispatch> dispatches)
        {
            var content = new StringBuilder();

            content.Append(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd"">");
            content.Append("<html>");
            content.Append("<head>");
            content.Append(@"<meta http-equiv=""Content-Type"" content=""text/html;charset=utf-8"">");
            content.Append(GetTitle("Singer Specialized - Dispatch"));
            content.Append(GetStyles());
            content.Append("</head>");
            content.Append("<body>");

            for (var i = 0; i < dispatches.Count; i++)
            {
                var dispatch = dispatches[i];
                                
                content.Append(FillDispatchBody(dispatch, "Driver Copy"));

                if (IncludeFileCopy == true)
                {
                    content.Append(PageBreak);
                    content.Append(FillDispatchBody(dispatch, "File Copy"));    
                }

                if (dispatch.Rate != null && dispatch.Rate.Name == "Pull Tractor")
                {
                    content.Append(PageBreak);
                    content.Append(GetBillOfLadingDocs(dispatch));
                }

                if ((i + 1) != dispatches.Count)
                    content.Append(PageBreak);
            }

            content.Append("</body>");
            content.Append("</html>");

            return content.ToString();
        }

        private string FillDispatchBody(Dispatch dispatch, string copyType)
        {
            if (dispatch.Load == null)
                return @"<span class=""error"">Dispatches must be attached to loads before they can be printed.</span>";

            var output = new StringBuilder();

            output.Append(@"<div class=""dispatch_doc"">");
            output.Append(GetHeader(dispatch, copyType));
            output.Append(GetDetails(dispatch));
            output.Append(GetDescription(dispatch.Description));
            output.Append(GetEquipment(dispatch.Load.ExtraEquipment));
            output.Append(GetSchedule(dispatch));
            output.Append(GetLoadCommodities(dispatch.Load.JobCommodities));
            output.Append(GetDimensions(dispatch.Load));
            output.Append(GetTractors(from d in dispatch.Load.Dispatches where d.Rate != null && d.Rate.Name.Contains("Tractor") select d));
            output.Append(GetSingerPilots(from p in dispatch.Load.Dispatches where p.Rate != null && p.Rate.Name.Contains("Pilot") select p));
            output.Append(GetThirdPartyPilots(from s in dispatch.Load.ThirdPartyServices where s.ServiceType != null && s.ServiceType.Name.Contains("Pilot") select s));
            output.Append(GetThridPartyServices(from s in dispatch.Load.ThirdPartyServices where s.ServiceType == null || (!s.ServiceType.Name.Contains("Pilot") && !s.ServiceType.Name.Contains("Wirelift")) select s));
            output.Append(GetWireLiftInfo(from wl in dispatch.Load.ThirdPartyServices where wl.ServiceType != null && wl.ServiceType.Name.Contains("Wirelift") select wl));
            output.Append(GetPermits(dispatch.Load.Permits));
            output.Append(GetOtherInfo(dispatch));
            output.Append("</div>");

            return output.ToString();
        }

        private string GetBillOfLadingDocs(Dispatch dispatch)
        {
            if (dispatch.Load == null) return "";

            var doc = new BillOfLadingDocument();
            var content = new StringBuilder();

            doc.PrintMetric = PrintMetric;

            for (var i = 0; i < dispatch.Load.JobCommodities.Count; i++)
            {
                var commodity = dispatch.Load.JobCommodities[i];

                content.Append(doc.GenerateBodyHTML(commodity));

                if ((i + 1) != dispatch.Load.JobCommodities.Count)
                    content.Append(PageBreak);
            }

            return content.ToString();
        }

        private static string GetTitle(string title)
        {
            return "<title>" + title + "</title>";
        }

        private static string GetStyles()
        {
            var content = @"
                <style type=""text/css"" media=""all"">
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
                    }
                    body 
                    {
                        line-height: 1.2em;
                        padding: 5px;
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
                    table
                    {             
                    }
                    td
                    {
                    }
                    body
                    {                        
                        font-size: 8pt;
                        font-family: Verdana, Arial, Helvetica, sans-serif;
                        padding: 10px;
                    }
                    
                    /*******/                    
                    div.dispatch_doc span.error
                    {
                        display: block;
                        font-weight; bold;
                        text-align: center;
                    }

                    div.dispatch_doc th
                    {
                        text-align: left;
                    }

                    div.dispatch_doc span.field_name
                    {
                        font-weight: bold;
                    }
                                                            
                    div.dispatch_doc div.header table 
                    {
                        width: 100%;
                        border-collapse: collapse;
                    }
                    
                    div.dispatch_doc div.header td
                    {                
                        vertical-align: top;
                        padding: 10px;
                    }
                    
                    div.dispatch_doc div.header td.logo_col
                    {
                        width: 200px;
                        
                    }
                    
                    div.dispatch_doc div.header td.address_col
                    {
                        
                    }
                    
                    div.dispatch_doc div.header td.id_col
                    {             
                        text-align: center;
                        font-weight: bold;
                        line-height: 1.35em;                     
                    }
                    
                    div.dispatch_doc div.header td.id_col span.copy_type, div.header td.id_col span.number
                    {
                    	font-size: 1.25em;
                    }
                    
                    div.dispatch_doc div.header span
                    {
                        display: block;
                    }
                    
                    div.dispatch_doc div.header span.title
                    {
                        display: block;
                        font-weight: bold;
                        font-size: 1.5em;
                        padding: 0.5em 0.3em;
                        text-align: center;
                        border-top: 2px #808080 solid;
                        border-bottom: 2px #808080 solid;
                    }
                    
                    div.dispatch_doc div.details
                    {
                        padding: 10px;                
                    }            
                    
                    div.dispatch_doc div.details table.dispatch_info, div.details table.departure_info
                    {   
                        margin-bottom: 10px;
                    }
                    
                    div.dispatch_doc div.details td.field_name
                    {
                        font-weight: bold;
                        white-space: nowrap;
                        padding-bottom: 5px;
                        padding-right: 10px;
                    }
                    
                    div.dispatch_doc div.details td.value
                    {
                        padding-right: 15px;
                    }

                    div.dispatch_doc div.details table.dispatch_info td.value
                    {
                        padding-right: 35px;
                    }
                    
                    div.dispatch_doc div.section
                    {
                        padding: 10px;
                        margin-top: 2px;
                        border-top: 2px #808080 solid;
                    }
                    
                    div.dispatch_doc div.section span.heading
                    {
                    	text-decoration: underline;
                        font-weight: bold;
                        display: block;                
                        margin-bottom: 10px;
                    }

                    div.dispatch_doc div.section span.subheading
                    {
                        font-weight: bold;
                        display: block;
                    }

                    div.dispatch_doc div.load_and_unload div.commodity
                    {
                    	margin: 5px 0;
                    	padding: 15px;                    	
                    }

                    div.dispatch_doc hr
                    {
                    	border: 0;
                    	height: 1px;
                    	background-color: #D9D9D9;
                    }

                    div.dispatch_doc div.load_and_unload span.commodity_name
                    {
                        font-weight: bold;
                        padding-bottom: 5px;          
                    }

                    div.dispatch_doc div.load_and_unload div.loading, div.dispatch_doc div.load_and_unload div.unloading
                    {
                        margin-top: 15px;
                    }
                    
                    div.dispatch_doc div.load_and_unload div.loading span.subheading, div.dispatch_doc div.load_and_unload div.unloading span.subheading
                    {
                        text-decoration: underline;
                        margin-bottom: 5px;
                    }

                    div.dispatch_doc div.load_and_unload td
                    {
                        padding: 3px 10px;
                    }

                    div.dispatch_doc div.load_and_unload table.details
                    {
                    	width: 100%;
                    }
                    
                    div.dispatch_doc div.load_and_unload table.details td
                    {
                    	border: solid 1px #A9A9A9;
                    }

                    div.dispatch_doc div.load_and_unload table.details td span.contact
                    {
                        display: block;
                    }

                    div.dispatch_doc div.load_and_unload table.details td.date
                    {
                    	width: 60px;
                    }
                    
                    div.dispatch_doc div.load_and_unload table.details td.time
                    {
                    	width: 45px;
                    }

                    div.dispatch_doc div.load_and_unload table.details td.contact
                    {
                    	width: 130px;
                    }
                    
                    div.dispatch_doc div.load_and_unload table.details td.company
                    {
                    	width: 120px;
                    }

                    div.dispatch_doc div.load_and_unload table.instructions
                    {
                        margin-top: 10px;
                        width: 100%;
                    }

                    div.dispatch_doc div.load_and_unload table.instructions td
                    {
                        width: 33%;
                    }

                    div.dispatch_doc div.dimensions span
                    {
                        padding-right: 15px;
                    }

                    div.dispatch_doc div.dimensions table.dimensions
                    {
                        width: 100%;
                        margin-bottom: 10px;
                    }                    
                    
                    div.dispatch_doc div.dimensions table.weights th
                    {
                        text-align: center;
                    }
                    
                    div.dispatch_doc div.dimensions table.weights td
                    {
                        text-align: center;
                        border: solid 1px #A9A9A9;
                    }
                    
                    
                    div.dispatch_doc div.dimensions table.weights td
                    {
                    	width: 75px;
                    }
                    
                    div.dispatch_doc div.dimensions table.weights td.row_name
                    {
                        text-align: right;
                        width: auto;
                        font-weight: bold;
                        border: none;
                        padding-right: 3px;
                    }
                                        
                    div.dispatch_doc table.simple_breakdown th, div.dispatch_doc table.simple_breakdown td
                    {
                        padding-right: 20px;
                        padding-bottom: 5px;
                    }

                    div.dispatch_doc table.simple_breakdown td.quantity
                    {
                    	text-align: center;
                    }

                    div.dispatch_doc table.commented_breakdown
                    {
                        width: 100%;
                        border-collapse: collapse;
                    }

                    div.dispatch_doc table.commented_breakdown th
                    {
                        padding-bottom: 10px;
                    }

                    div.dispatch_doc table.commented_breakdown tr.details td
                    {
                    	border-top: 1px solid #E9E9E9;  
                    	padding-top: 10px;                  	
                    }

                    div.dispatch_doc table.commented_breakdown tr.comments td
                    {
                        padding: 5px 10px;
                        padding-bottom: 10px;                        
                    }
                    
                    %BOL_SCREEN%

                    div.page_break
                    {
                        display: block;
                        margin: 35px;
                        height: 1px;
                        border-top: 1px #454545 solid;
                    }    
                </style>
                <style type=""text/css"" media=""print"">
                    body
                    {
                    	font-size: 10pt;
                        padding: 0;
                    }
                    
                    %BOL_PRINT%

                    div.page_break
                    {
                        border: none;
                        display: block;
                        page-break-before: always;
                        margin: 0;
                    }
                </style>
            ";

            content = content.Replace("%BOL_SCREEN%", BillOfLadingDocument.GetDocSpecificScreenStyles()).Replace("%BOL_PRINT%", BillOfLadingDocument.GetDocSpecificPrintStyles());

            return content;
        }

        private string GetHeader(Dispatch dispatch, string copyType)
        {           
            var html = @"
                <div class=""header"">
                    <table>
                        <tr>
                            <td class=""logo_col"">
                                <span class=""logo""><img src=""{0}"" alt=""Singer Specialized""></span>
                            </td>
                            <td class=""address_col"">
                                <span>{1}</span>
                                <span>{2}</span>
                                <span>{3}</span>
                                <span>Phone: {4}</span>
                            </td>
                            <td class=""id_col"">
                                <span class=""copy_type"">{5}</span>
                                <span>Dispatch #:</span>
                                <span class=""number"">{6}</span>
                            </td>
                        </tr>
                    </table>
                    
                    <span class=""title"">Dispatch Order</span>            
                </div>
            ";

            var replacements = new object[7];

            var process = System.Diagnostics.Process.GetCurrentProcess();
            string img;

            if (SpecializedDocument)
                img = SingerConstants.GetConfig("Documents-SingerHeaderImg") ?? @"Images\SingerHeader.png";
            else
                img = SingerConstants.GetConfig("Documents-MEHeaderImg") ?? @"Images\MEHeader.png";

            if (process.MainModule != null)
            {
                img = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(process.MainModule.FileName), img);
            }

            replacements[0] = img;
            replacements[1] = SingerConstants.GetConfig("SingerName") ?? "Singer Specialized";
            replacements[2] = SingerConstants.GetConfig("SingerAddress-StreetAddress");
            replacements[3] = SingerConstants.GetConfig("SingerAddress-City");
            replacements[4] = SingerConstants.GetConfig("SingerAddress-Phone");
            replacements[5] = copyType;
            replacements[6] = (dispatch != null) ? dispatch.Name : "UNKNOWN";

            return string.Format(html, replacements);
        }

        private static string GetDetails(Dispatch dispatch)
        {
            const string html = @"
                <div class=""details"">
                    <table class=""dispatch_info"">
                        <tr>
                            <td class=""field_name col1_4"">Date:</td>
                            <td class=""value col2_4"">{0}</td>
                            <td class=""field_name col3_4"">Customer:</td>
                            <td class=""value col4_4"">{1}</td>
                        </tr>
                        <tr>
                            <td class=""field_name"">Unit #:</td>
                            <td class=""value"">{2}</td>
                            <td class=""field_name"">Trailer #:</td>
                            <td class=""value"">{3}</td>
                        </tr>
                        <tr>
                            <td class=""field_name"">Driver:</td>
                            <td class=""value"">{4}</td>
                            <td class=""field_name"">Swampers:</td>
                            <td class=""value"">{5}</td>
                        </tr>
                    </table>
                    
                    <table class=""departure_info"">
                        <tr>
                            <td class=""field_name col1_2"">Dispatched By:</td>
                            <td class=""value col2_2"">{10}</td>
                        </tr>
                        <tr>
                            <td class=""field_name col1_2"">Depart Date:</td>
                            <td class=""value col2_2"">{6}</td>
                        </tr>
                        <tr>                    
                            <td class=""field_name"">Depart From:</td>
                            <td class=""value"">{7}</td>
                        </tr>
                        <tr>
                            <td class=""field_name"">Depart Units:</td>
                            <td class=""value"">{8}</td>
                        </tr>
                    </table>
                   
                    <table class=""customer_references"">
                        <tr>
                            <td class=""field_name col1_2"">Customer References:</td>
                            <td class=""value col2_2"">
                                {9}                               
                            </td>
                        </tr>
                    </table>
                </div>
            ";

            var replacements = new object[11];

            replacements[0] = DateTime.Now.ToString(SingerConstants.PrintedDateFormatString);
            replacements[1] = dispatch.Job.Company.Name;
            replacements[2] = (dispatch.Equipment != null) ? dispatch.Equipment.UnitNumber : "";
            replacements[3] = (dispatch.Load != null && dispatch.Load.Rate != null) ? dispatch.Load.Rate.Name + " - " : "";

            if (dispatch.Load != null && dispatch.Load.TrailerCombination != null)
                replacements[3] += dispatch.Load.TrailerCombination.Combination;

            replacements[4] = (dispatch.Employee != null) ? dispatch.Employee.Name : "";
            

            // Find list all of the swampers
            var swamperDispatches = from s in dispatch.Load.Dispatches where s.Rate != null && s.Rate.Name.Contains("Swamper") select s;
            var swampers = new StringBuilder();
            foreach (var item in swamperDispatches)
            {
                if (item.Employee != null)
                {
                    swampers.Append(item.Employee.Name);
                    swampers.Append("; ");
                }
            }

            replacements[5] = swampers.ToString();
            replacements[6] = (dispatch.MeetingTime != null) ? dispatch.MeetingTime.Value.ToString(SingerConstants.PrintedDateFormatString) + " " + dispatch.MeetingTime.Value.ToString(SingerConstants.PrintedTimeFormatString) : "";
            replacements[7] = dispatch.DepartingLocation;
            replacements[8] = dispatch.DepartingUnits;
            replacements[10] = (dispatch.Job.Employee != null) ? dispatch.Job.Employee.Name : "";
            
            // List any reference numbers given to this job
            var references = new StringBuilder();
            
            for (var i = 0; i < dispatch.Job.ReferenceNumbers.Count; i++) 
            {
                var item = dispatch.Job.ReferenceNumbers[i];

                references.Append(@"<span class=""reference""><span class=""field_name"">" + item.Field + @"</span>: <span class=""value"">" + item.Value + "</span></span>");

                if ((i + 1) != dispatch.Job.ReferenceNumbers.Count)
                    references.Append(", ");
            }
            
            replacements[9] = references.ToString();

            return string.Format(html, replacements);
        }

        private static string GetDescription(string description)
        {
            const string html = @"
                <div class=""description section"">
                    <span class=""heading"">Dispatch Description</span>
                    
                    <p>{0}</p>
                </div>
            ";

            return string.Format(html, description);
        }

        private static string GetEquipment(EntitySet<ExtraEquipment> equipment)
        {
            // Fill this section with all of the ExtraEquipement entities attached to the dispatch's load.

            const string html = @"
                <div class=""equipment_requirements section"">
                    <span class=""heading"">Required Equipment</span>
                    
                    {0}
                </div>
            ";
            const string table = @"
                <table class=""simple_breakdown"">
                    <thead>
                        <tr>
                            <th>Equipment</th>
                            <th>Quantity</th>
                            <th>Comments</th>
                        </tr>
                    </thead>
                    <tbody>
                        {0}
                    </tbody>
                </table>
            ";
            const string row = @"
                <tr>
                    <td>{0}</td>
                    <td class=""quantity"">{1}</td>
                    <td>{2}</td>
                </tr>
            ";

            if (equipment.Count == 0)
                return string.Format(html, "");

            var rows = new StringBuilder();

            foreach (var item in equipment)
            {
                var replacements = new object[3];

                replacements[0] = (item.ExtraEquipmentType != null) ? item.ExtraEquipmentType.Name : "";
                replacements[1] = item.Quantity;
                replacements[2] = item.Comments;

                rows.Append(string.Format(row, replacements));
            }

            return string.Format(html, string.Format(table, rows.ToString()));
        }

        private static string GetSchedule(Dispatch dispatch)
        {
            const string content = @"
                <div class=""schuedule section"">
                    <span class=""heading"">Dispatch Schedule</span>
                    
                    <p>{0}</p>
                </div>
            ";

            var schedule = dispatch.Schedule ?? "";

            return string.Format(content, schedule);
        }

        private string GetLoadCommodities(EntitySet<JobCommodity> commodities)
        {
            const string head = @"<div class=""load_and_unload section""><span class=""heading"">Load/Unload Information</span>";
            const string divider = "<hr>";
            const string foot = "</div>";
            const string commodityHtml = @"                            
                <div class=""commodity"">
                    <span class=""commodity_name"">{0}</span> 
                    <span class=""unit"">{1}</span>

                    <div class=""dimensions"">
                        <span class=""weight"">{2}</span>
                        <span class=""size"">{3}</span>                    
                    </div>
                    
                    <div class=""loading"">
                        <span class=""subheading"">Load Information</span>
                        
                        <table class=""details"">
                            <thead>
                                <tr>
                                    <th>Date</th>
                                    <th>Time</th>
                                    <th>Load Location</th>
                                    <th>Load Site Contact</th>
                                    <th>Load By</th>
                                    <th>Loading Contact</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td class=""date""><span>{4}</span></td>
                                    <td class=""time""><span>{5}</span></td>
                                    <td class=""location""><span>{6}</span></td>
                                    <td class=""contact""><span>{7}</span></td>
                                    <td class=""company""><span>{8}</span></td>
                                    <td class=""contact""><span>{9}</span></td>
                                </tr>
                            </tbody>                        
                        </table>
                        
                        <table class=""instructions"">
                            <thead>
                                <tr>
                                    <th>Load Route</th>
                                    <th>Load Instruction</th>
                                    <th>Load Placement/Orientation</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td>{10}</td>
                                    <td>{11}</td>
                                    <td>{12}</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>                  

                    <div class=""unloading"">
                        <span class=""subheading"">Unload Information</span>
                        
                        <table class=""details"">
                            <thead>
                                <tr>
                                    <th>Date</th>
                                    <th>Time</th>
                                    <th>Load Location</th>
                                    <th>Load Site Contact</th>
                                    <th>Load By</th>
                                    <th>Loading Contact</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td class=""date""><span>{13}</span></td>
                                    <td class=""time""><span>{14}</span></td>
                                    <td class=""location""><span>{15}</span></td>
                                    <td class=""contact""><span>{16}</span></td>
                                    <td class=""company""><span>{17}</span></td>
                                    <td class=""contact""><span>{18}</span></td>
                                </tr>
                            </tbody>                        
                        </table>
                        
                        <table class=""instructions"">
                            <thead>
                                <tr>
                                    <th>Unload Route</th>
                                    <th>Unload Instruction</th>
                                    <th>Unload Placement/Orientation</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td><span>{19}</span></td>
                                    <td><span>{20}</span></td>
                                    <td><span>{21}</span></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            ";

            var html = new StringBuilder();

            html.Append(head);

            string lengthUnit, weightUnit;

            if (PrintMetric != true)
            {
                lengthUnit = MeasurementFormater.UFeet;
                weightUnit = MeasurementFormater.UPounds;
            }
            else
            {
                lengthUnit = MeasurementFormater.UMetres;
                weightUnit = MeasurementFormater.UKilograms;
            }

            for (var i = 0; i < commodities.Count; i++)
            {
                var item = commodities[i];
                var reps = new object[22];

                reps[0] = item.Name;
                reps[1] = (item.Unit != null) ? string.Format("Unit {0}", item.Unit) : "";
                reps[2] = string.Format("{0}", MeasurementFormater.FromKilograms(item.Weight, weightUnit));
                reps[3] = string.Format("{0} x {1} x {2} (LxWxH)", MeasurementFormater.FromMetres(item.Length, lengthUnit), MeasurementFormater.FromMetres(item.Width, lengthUnit), MeasurementFormater.FromMetres(item.Height, lengthUnit));

                if (item.LoadDate != null)
                {
                    reps[4] = item.LoadDate.Value.ToString(SingerConstants.PrintedDateFormatString);
                    reps[5] = item.LoadDate.Value.ToString(SingerConstants.PrintedTimeFormatString);
                }
                else
                {
                    reps[4] = "";
                    reps[5] = "";
                }

                reps[6] = item.LoadLocation;
                reps[7] = (item.LoadContact != null) ? string.Format(@"<span class=""contact"">{0}</span><span class=""contact"">{1}</span>", item.LoadContact.Name, item.LoadContact.PrimaryPhone) : "";
                reps[8] = item.LoadBy;
                reps[9] = "N\\A";
                reps[10] = item.LoadRoute;
                reps[11] = item.LoadInstructions;
                reps[12] = item.LoadOrientation;


                if (item.LoadDate != null)
                {
                    reps[13] = item.UnloadDate.Value.ToString(SingerConstants.PrintedDateFormatString);
                    reps[14] = item.UnloadDate.Value.ToString(SingerConstants.PrintedTimeFormatString);
                }
                else
                {
                    reps[13] = "";
                    reps[14] = "";
                }

                reps[15] = item.UnloadLocation;
                reps[16] = (item.UnloadContact != null) ? string.Format(@"<span class=""contact"">{0}</span><span class=""contact"">{1}</span>", item.UnloadContact.Name, item.UnloadContact.PrimaryPhone) : "";
                reps[17] = item.UnloadBy;
                reps[18] = "N\\A";
                reps[19] = item.UnloadRoute;
                reps[20] = item.UnloadInstructions;
                reps[21] = item.UnloadOrientation;
                

                html.Append(string.Format(commodityHtml, reps));

                if ((i + 1) != commodities.Count)
                    html.Append(divider);
            }

            html.Append(foot);

            return html.ToString();
        }

        private string GetDimensions(Load load)
        {
            const string content = @"
                <div class=""dimensions section"">
                    <span class=""heading"">Dimensional Information</span>
                    
                    <table class=""dimensions"">
                        <tr>
                            <td><span class=""field_name"">Loaded Length:</span> <span class=""value"">{0}</span></td>
                            <td><span class=""field_name"">Loaded Width:</span> <span class=""value"">{1}</span></td>                    
                            <td><span class=""field_name"">Loaded Height:</span> <span class=""value"">{2}</span></td>
                            <td><span class=""field_name"">Gross Weight:</span> <span class=""value"">{3}</span></td>
                        </tr>
                    </table>
                    
                    <span class=""subheading"">Axle Weights</span>

                    <table class=""weights"">
                        <tr>
                            <th></th>
                            <th>Steer</th>
                            <th>Drive</th>
                            <th>Group 1</th>
                            <th>Group 3</th>
                            <th>Group 3</th>
                            <th>Group 4</th>
                            <th>Group 5</th>
                            <th>Group 6</th>
                            <th>Group 7</th>
                            <th>Group 8</th>
                            <th>Group 9</th>
                            <th>Group 10</th>                    
                        </tr>
                        <tr>
                            <td class=""row_name"">Estimated</td>                            
                            <td><span>{4}</span></td>
                            <td><span>{5}</span></td>
                            <td><span>{6}</span></td>
                            <td><span>{7}</span></td>
                            <td><span>{8}</span></td>
                            <td><span>{9}</span></td>
                            <td><span>{10}</span></td>
                            <td><span>{11}</span></td>
                            <td><span>{12}</span></td>
                            <td><span>{13}</span></td>
                            <td><span>{14}</span></td>
                            <td><span>{15}</span></td>
                        </tr>
                        <tr>
                            <td class=""row_name"">Scaled</td>                            
                            <td><span>{16}</span></td>
                            <td><span>{17}</span></td>
                            <td><span>{18}</span></td>
                            <td><span>{19}</span></td>
                            <td><span>{20}</span></td>
                            <td><span>{21}</span></td>
                            <td><span>{22}</span></td>
                            <td><span>{23}</span></td>
                            <td><span>{24}</span></td>
                            <td><span>{25}</span></td>
                            <td><span>{26}</span></td>
                            <td><span>{27}</span></td>
                        </tr>
                    </table>
                </div>
            ";

            string lengthUnit, weightUnit;

            if (PrintMetric != true)
            {
                lengthUnit = MeasurementFormater.UFeet;
                weightUnit = MeasurementFormater.UPounds;
            }
            else
            {
                lengthUnit = MeasurementFormater.UMetres;
                weightUnit = MeasurementFormater.UKilograms;
            }

            var replacements = new object[28];

            replacements[0] = MeasurementFormater.FromMetres(load.LoadedLength, lengthUnit);
            replacements[1] = MeasurementFormater.FromMetres(load.LoadedWidth, lengthUnit);
            replacements[2] = MeasurementFormater.FromMetres(load.LoadedHeight, lengthUnit);
            replacements[3] = MeasurementFormater.FromKilograms(load.GrossWeight, weightUnit);
            replacements[4] = MeasurementFormater.FromKilograms(load.EWeightSteer, weightUnit);
            replacements[5] = MeasurementFormater.FromKilograms(load.EWeightDrive, weightUnit);
            replacements[6] = MeasurementFormater.FromKilograms(load.EWeightGroup1, weightUnit);
            replacements[7] = MeasurementFormater.FromKilograms(load.EWeightGroup2, weightUnit);
            replacements[8] = MeasurementFormater.FromKilograms(load.EWeightGroup3, weightUnit);
            replacements[9] = MeasurementFormater.FromKilograms(load.EWeightGroup4, weightUnit);
            replacements[10] = MeasurementFormater.FromKilograms(load.EWeightGroup5, weightUnit);
            replacements[11] = MeasurementFormater.FromKilograms(load.EWeightGroup6, weightUnit);
            replacements[12] = MeasurementFormater.FromKilograms(load.EWeightGroup7, weightUnit);
            replacements[13] = MeasurementFormater.FromKilograms(load.EWeightGroup8, weightUnit);
            replacements[14] = MeasurementFormater.FromKilograms(load.EWeightGroup9, weightUnit);
            replacements[15] = MeasurementFormater.FromKilograms(load.EWeightGroup10, weightUnit);
            replacements[16] = MeasurementFormater.FromKilograms(load.SWeightSteer, weightUnit);
            replacements[17] = MeasurementFormater.FromKilograms(load.SWeightDrive, weightUnit);
            replacements[18] = MeasurementFormater.FromKilograms(load.SWeightGroup1, weightUnit);
            replacements[19] = MeasurementFormater.FromKilograms(load.SWeightGroup2, weightUnit);
            replacements[20] = MeasurementFormater.FromKilograms(load.SWeightGroup3, weightUnit);
            replacements[21] = MeasurementFormater.FromKilograms(load.SWeightGroup4, weightUnit);
            replacements[22] = MeasurementFormater.FromKilograms(load.SWeightGroup5, weightUnit);
            replacements[23] = MeasurementFormater.FromKilograms(load.SWeightGroup6, weightUnit);
            replacements[24] = MeasurementFormater.FromKilograms(load.SWeightGroup7, weightUnit);
            replacements[25] = MeasurementFormater.FromKilograms(load.SWeightGroup8, weightUnit);
            replacements[26] = MeasurementFormater.FromKilograms(load.SWeightGroup9, weightUnit);
            replacements[27] = MeasurementFormater.FromKilograms(load.SWeightGroup10, weightUnit);

            return string.Format(content, replacements);
        }

        private static string GetTractors(IEnumerable<Dispatch> dispatches)
        {
            const string html = @"
                <div class=""tractors section"">
                    <span class=""heading"">Tractors (Singer Service)</span>
                    
                    {0}
                </div>
            ";
            const string table = @"
                <table class=""simple_breakdown"">
                    <thead>
                        <tr>
                            <th>Unit</th>
                            <th>Contact</th>
                            <th>Trailer Combination</th>
                        </tr>
                    </thead>
                    <tbody>
                        {0}
                    </tbody>
                </table>
            ";
            const string row = @"
                <tr>
                    <td>{0}</td>
                    <td>{1}</td>
                    <td>{2}</td>
                </tr>
            ";

            if (dispatches.Count() == 0)
                return string.Format(html, "");

            var rows = new StringBuilder();
            foreach (var item in dispatches)
            {
                var replacements = new object[3];

                replacements[0] = (item.Equipment != null) ? item.Equipment.UnitNumber : "";
                replacements[1] = (item.Employee == null) ? "" : item.Employee.Name + " " + item.Employee.Phone;
                replacements[2] = (item.Load != null && item.Load.TrailerCombination != null) ? item.Load.TrailerCombination.Combination : "";

                rows.Append(string.Format(row, replacements));
            }

            return string.Format(html, string.Format(table, rows.ToString()));
        }

        private static string GetSingerPilots(IEnumerable<Dispatch> dispatches)
        {
            const string html = @"
                <div class=""other_equipment section"">
                    <span class=""heading"">Pilot Car and Other Equipment (Singer Service)</span>
                    
                    {0}
                </div>
            ";
            const string table = @"
                <table class=""simple_breakdown"">
                    <thead>
                        <tr>
                            <th>Unit</th>
                            <th>Contact</th>
                        </tr>
                    </thead>
                    <tbody>
                        {0}
                    </tbody>
                </table>
            ";
            const string row = @"
                <tr>
                    <td>{0}</td>
                    <td>{1}</td>
                </tr>
            ";

            if (dispatches.Count() == 0)
                return string.Format(html, "");

            var rows = new StringBuilder();
            foreach (var item in dispatches)
            {
                var replacements = new object[2];
                var contact = (item.Employee == null) ? "" : item.Employee.Name;

                if (item.Employee != null && !string.IsNullOrEmpty(item.Employee.Phone))
                    contact += " " + item.Employee.Phone;

                replacements[0] = (item.Equipment == null) ? "" : item.Equipment.UnitNumber;
                replacements[1] = contact;

                rows.Append(string.Format(row, replacements));
            }

            return string.Format(html, string.Format(table, rows.ToString()));
        }

        private static string GetThirdPartyPilots(IEnumerable<ThirdPartyService> pilots)
        {
            const string html = @"
                <div class=""third_party_pilot section"">
                    <span class=""heading"">Pilot Car (Thrid Party)</span>
                
                    {0}
                </div>
            ";
            const string table = @"
                <table class=""commented_breakdown"">
                    <thead>
                        <tr>
                            <th>Date &amp Time</th>
                            <th>Company</th>
                            <th>Contact</th>
                            <th>Location</th>
                            <th>Phone</th>
                            <th>Confirmation #</th>
                        </tr>
                    </thead>
                    <tbody>
                        {0}
                    </tbody>                
                </table>
            ";
            const string row = @"
                <tr class=""details"">
                    <td>{0}</td>
                    <td>{1}</td>
                    <td>{2}</td>
                    <td>{3}</td>
                    <td>{4}</td>
                    <td>{5}</td>
                </tr>
            ";
            const string commentRow = @"
                <tr class=""comments"">
                    <td colspan=""7"">
                        <span><span class=""field_name"">Comments:</span> {0}</span>
                    </td>
                </tr>
            ";

            if (pilots.Count() == 0)
                return string.Format(html, "");

            var rows = new StringBuilder();
            foreach (var item in pilots)
            {
                var replacements = new object[7];

                replacements[0] = (item.ServiceDate == null) ? "" : item.ServiceDate.Value.ToString(SingerConstants.PrintedDateFormatString) + " " + item.ServiceDate.Value.ToString(SingerConstants.PrintedTimeFormatString);                
                replacements[1] = (item.Company == null) ? "" : item.Company.Name;
                replacements[2] = (item.Contact == null) ? "" : item.Contact.Name;
                replacements[3] = item.Location;
                replacements[4] = (item.Contact == null) ? "" : item.Contact.PrimaryPhone;
                replacements[5] = item.Reference;

                rows.Append(string.Format(row, replacements));
                rows.Append(string.Format(commentRow, item.Notes));
            }

            return string.Format(html, string.Format(table, rows.ToString()));
        }

        private static string GetThridPartyServices(IEnumerable<ThirdPartyService> services)
        {
            const string html = @"
                <div class=""thid_party_services section"">
                    <span class=""heading"">Third Party Services</span>
                    
                    {0}
                </div>
            ";
            const string table = @"
                <table class=""commented_breakdown"">
                    <thead>
                        <tr>
                            <th>Date &amp Time</th>
                            <th>Service Type</th>
                            <th>Company</th>
                            <th>Contact</th>
                            <th>Location</th>
                            <th>Phone</th>
                            <th>Confirmation #</th>
                        </tr>
                    </thead>
                    <tbody>
                        {0}
                    </tbody>                
                </table>
            ";
            const string row = @"
                <tr class=""details"">
                    <td>{0}</td>
                    <td>{1}</td>
                    <td>{2}</td>
                    <td>{3}</td>
                    <td>{4}</td>
                    <td>{5}</td>
                    <td>{6}</td>
                </tr>
            ";
            const string commentRow = @"
                <tr class=""comments"">
                    <td colspan=""7"">
                        <span><span class=""field_name"">Comments:</span> {0}</span>
                    </td>
                </tr>
            ";

            if (services.Count() == 0)
                return string.Format(html, "");

            var rows = new StringBuilder();
            foreach (var item in services)
            {
                var replacements = new object[7];

                replacements[0] = (item.ServiceDate == null) ? "" : item.ServiceDate.Value.ToString(SingerConstants.PrintedDateFormatString) + " " + item.ServiceDate.Value.ToString(SingerConstants.PrintedTimeFormatString);
                replacements[1] = (item.ServiceType == null) ? "" : item.ServiceType.Name;
                replacements[2] = (item.Company == null) ? "" : item.Company.Name;
                replacements[3] = (item.Contact == null) ? "" : item.Contact.Name;
                replacements[4] = item.Location;
                replacements[5] = (item.Contact == null) ? "" : item.Contact.PrimaryPhone;
                replacements[6] = item.Reference;

                rows.Append(string.Format(row, replacements));
                rows.Append(string.Format(commentRow, item.Notes));
            }

            return string.Format(html, string.Format(table, rows.ToString()));
        }

        private static string GetWireLiftInfo(IEnumerable<ThirdPartyService> wirelifts)
        {
            const string html = @"
                <div class=""wire_lifts section"">
                    <span class=""heading"">Wire Lift Information</span>
                    
                    {0}
                </div>
            ";
            const string table = @"                
                <table class=""commented_breakdown"">
                    <thead>
                        <tr>
                            <th>Date &amp; Time</th>
                            <th>Company</th>
                            <th>Contact</th>
                            <th>Location</th>
                            <th>Phone</th>
                            <th>Confirmation #</th>                        
                        </tr>                    
                    </thead>
                    <tbody>
                        {0}
                    </tbody>
                </table>
            ";
            const string row = @"
                <tr class=""details"">
                    <td>{0}</td>
                    <td>{1}</td>
                    <td>{2}</td>
                    <td>{3}</td>
                    <td>{4}</td>
                    <td>{5}</td>
                </tr>
            ";
            const string commentRow = @"
                <tr class=""comments"">
                    <td colspan=""6"">
                        <span><span class=""field_name"">Comments: </span> {0}</span>
                    </td>
                </tr>
            ";

            if (wirelifts.Count() == 0)
                return string.Format(html, "");

            var rows = new StringBuilder();
            foreach (var item in wirelifts)
            {
                var replacements = new object[6];

                replacements[0] = (item.ServiceDate == null) ? "" : item.ServiceDate.Value.ToString(SingerConstants.PrintedDateFormatString) + " " + item.ServiceDate.Value.ToString(SingerConstants.PrintedTimeFormatString);
                replacements[1] = (item.Company == null)  ? "" : item.Company.Name;
                replacements[2] = (item.Contact == null) ? "" : item.Contact.Name;
                replacements[3] = item.Location;
                replacements[4] = (item.Contact == null) ? "" : item.Contact.PrimaryPhone;
                replacements[5] = item.Reference;

                rows.Append(string.Format(row, replacements));
                rows.Append(string.Format(commentRow, item.Notes));
            }

            return string.Format(html, string.Format(table, rows.ToString()));
        }

        private static string GetPermits(EntitySet<Permit> permits)
        {
            const string html = @"
                <div class=""permits section"">
                    <span class=""heading"">Permit Information</span>
                    
                    {0}
                </div>
            ";
            const string table = @"
                <table class=""commented_breakdown"">
                    <thead>
                        <tr>
                            <th>Date</th>
                            <th>Issuer</th>
                            <th>Type</th>
                            <th>Number</th>
                        </tr>
                    </thead>
                    <tbody>
                        {0}
                    </tbody>
                </table>
            ";
            const string row = @"
                <tr class=""details"">
                    <td>{0}</td>
                    <td>{1}</td>
                    <td>{2}</td>
                    <td>{3}</td>
                </tr>
            ";
            const string commentRow = @"
                <tr class=""comments"">
                    <td colspan=""4"">
                        <span><span class=""field_name"">Conditions:</span> {0}</span>
                    </td>
                </tr>
            ";

            if (permits.Count == 0)
                return string.Format(html, "");

            var rows = new StringBuilder();

            foreach (var item in permits)
            {
                var replacements = new object[4];

                var start = (item.StartDate != null) ? item.StartDate.Value.ToString(SingerConstants.PrintedDateFormatString) : "--";
                var end = (item.EndDate != null) ? item.EndDate.Value.ToString(SingerConstants.PrintedDateFormatString) : "--";

                replacements[0] = start + " to " + end;
                replacements[1] = (item.IssuingCompany == null) ? "" : item.IssuingCompany.Name;
                replacements[2] = (item.PermitType == null) ? "" : item.PermitType.Name;
                replacements[3] = item.Reference;

                rows.Append(string.Format(row, replacements));
                rows.Append(string.Format(commentRow, item.Conditions));
            }            
                
            return string.Format(html, string.Format(table, rows.ToString()));
        }

        private static string GetOtherInfo(Dispatch dispatch)
        {
            const string content = @"
                <div class=""other_info section"">
                    <span class=""heading"">Other Information</span>

                    <p>{0}</p>
                </div>
            ";

            return string.Format(content, dispatch.Notes);
        }
    }
}


