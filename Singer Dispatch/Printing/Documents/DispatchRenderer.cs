using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Linq;

namespace SingerDispatch.Printing.Documents
{
    class DispatchRenderer : IRenderer
    {
        private const string PageBreak = @"<div class=""page_break""></div>";
        private bool IncludeFileCopy { get; set; }

        public DispatchRenderer()
        {
            IncludeFileCopy = false;
        }

        public DispatchRenderer(bool includeDriverCopy)
        {
            IncludeFileCopy = includeDriverCopy;
        }

        public string GenerateHTML(object dispatch)
        {
            if (dispatch is List<Dispatch>)
                return GenerateHTML((List<Dispatch>)dispatch);
            
            return GenerateHTML((Dispatch)dispatch);
        }

        public string GenerateHTML(Dispatch dispatch)
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

        public string GenerateHTML(List<Dispatch> dispatches)
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

                if ((i + 1) != dispatches.Count)
                    content.Append(PageBreak);
            }

            content.Append("</body>");
            content.Append("</html>");

            return content.ToString();
        }

        private static string FillDispatchBody(Dispatch dispatch, string copyType)
        {
            var output = new StringBuilder();

            output.Append(GetHeader(dispatch, copyType));
            output.Append(GetDetails(dispatch));
            output.Append(GetDescription(dispatch.Description));
            output.Append(GetEquipment());
            output.Append(GetSchedule(dispatch));
            output.Append(GetLoadCommodities(dispatch.Load.JobCommodities));
            output.Append(GetDimensions(dispatch.Load));
            output.Append(GetTractors());
            output.Append(GetSingerPilots());
            output.Append(GetThirdPartyPilots());
            output.Append(GetThridPartyServices());
            output.Append(GetWireLiftInfo());
            output.Append(GetPermits());
            output.Append(GetOtherInfo(dispatch));

            return output.ToString();
        }

        private static string GetTitle(string title)
        {
            return "<title>" + title + "</title>";
        }

        private static string GetStyles()
        {
            const string content = @"
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
                    
                    
                    /*******/
                    
                    body
                    {                        
                        font-size: 10pt;
                        font-family: Verdana, Arial, Helvetica, sans-serif;
                        padding: 10px;
                    }

                    th
                    {
                        text-align: left;
                    }

                    span.field_name
                    {
                        font-weight: bold;
                    }

                    div.header
                    {
                        
                    }
                    
                    div.header table 
                    {
                        width: 100%;
                        border-collapse: collapse;
                    }
                    
                    div.header td
                    {                
                        vertical-align: top;
                        padding: 10px;
                    }
                    
                    div.header td.logo_col
                    {
                        width: 200px;
                        
                    }
                    
                    div.header td.address_col
                    {
                        
                    }
                    
                    div.header td.id_col
                    {             
                        text-align: center;
                        font-weight: bold;
                        line-height: 1.35em;                     
                    }
                    
                    div.header td.id_col span.copy_type, div.header td.id_col span.number
                    {
                    	font-size: 1.25em;
                    }
                    
                    div.header span
                    {
                        display: block;
                    }
                    
                    div.header span.title
                    {
                        display: block;
                        font-weight: bold;
                        font-size: 1.5em;
                        padding: 0.5em 0.3em;
                        text-align: center;
                        border-top: 2px #808080 solid;
                        border-bottom: 2px #808080 solid;
                    }
                    
                    div.details
                    {
                        padding: 10px;                
                    }            
                    
                    div.details table.dispatch_info, div.details table.departure_info
                    {   
                        margin-bottom: 10px;
                    }
                    
                    div.details td.field_name
                    {
                        font-weight: bold;
                        white-space: nowrap;
                        padding-bottom: 5px;
                        padding-right: 10px;
                    }
                    
                    div.details td.value
                    {
                        padding-right: 15px;
                    }

                    div.details table.dispatch_info td.value
                    {
                        padding-right: 35px;
                    }
                    
                    div.section
                    {
                        padding: 10px;
                        margin-top: 2px;
                        border-top: 2px #808080 solid;
                    }
                    
                    div.section span.heading
                    {
                    	text-decoration: underline;
                        font-weight: bold;
                        display: block;                
                        margin-bottom: 10px;
                    }

                    div.section span.subheading
                    {
                        font-weight: bold;
                        display: block;
                    }

                    div.load_and_unload div.commodity
                    {
                    	margin: 5px 0;
                    	padding: 15px;                    	
                    }

                    hr
                    {
                    	border: 0;
                    	height: 1px;
                    	background-color: #D9D9D9;
                    }

                    div.load_and_unload span.commodity_name
                    {
                        font-weight: bold;
                        padding-bottom: 5px;          
                    }

                    div.load_and_unload div.loading, div.load_and_unload div.unloading
                    {
                        margin-top: 15px;
                    }
                    
                    div.load_and_unload div.loading span.subheading, div.load_and_unload div.unloading span.subheading
                    {
                        text-decoration: underline;
                        margin-bottom: 5px;
                    }

                    div.load_and_unload td
                    {
                        padding: 3px 10px;
                    }

                    div.load_and_unload table.details
                    {
                    	width: 100%;
                    }
                    
                    div.load_and_unload table.details td
                    {
                    	border: solid 1px #A9A9A9;
                    }

                    div.load_and_unload table.details td span
                    {
                    	display: block;
                    }

                    div.load_and_unload table.details td.date
                    {
                    	width: 60px;
                    }
                    
                    div.load_and_unload table.details td.time
                    {
                    	width: 45px;
                    }

                    div.load_and_unload table.details td.contact
                    {
                    	width: 130px;
                    }
                    
                    div.load_and_unload table.details td.company
                    {
                    	width: 120px;
                    }

                    div.load_and_unload table.instructions
                    {
                        margin-top: 10px;
                        width: 100%;
                    }

                    div.load_and_unload table.instructions td
                    {
                        width: 33%;
                    }

                    div.dimensions span
                    {
                        padding-right: 15px;
                    }

                    div.dimensions table.dimensions
                    {
                        width: 100%;
                        margin-bottom: 10px;
                    }                    
                    
                    div.dimensions table.weights th
                    {
                        text-align: center;
                    }
                    
                    div.dimensions table.weights td
                    {
                        text-align: center;
                        border: solid 1px #A9A9A9;
                    }
                    
                    
                    div.dimensions table.weights td
                    {
                    	width: 75px;
                    }
                    
                    div.dimensions table.weights td.row_name
                    {
                        text-align: right;
                        width: auto;
                        font-weight: bold;
                        border: none;
                        padding-right: 3px;
                    }
                                        
                    div.tractors td, div.other_equipment td
                    {
                        padding-right: 10px;
                        padding-bottom: 5px;
                    }

                    div.third_party_pilot table,
                    div.thid_party_services table,
                    div.wire_lifts table,
                    div.permits table
                    {
                        width: 100%;
                        border-collapse: collapse;
                    }

                    div.third_party_pilot th, 
                    div.thid_party_services th,
                    div.wire_lifts th,
                    div.permits th
                    {
                        padding-bottom: 10px;
                    }
                    
                    div.third_party_pilot tr, 
                    div.thid_party_services tr,
                    div.wire_lifts tr,
                    div.permits tr
                    {
                    	
                    }

                    div.third_party_pilot td, 
                    div.thid_party_services td,
                    div.wire_lifts td,
                    div.permits td
                    {
                    	
                    }

                    div.third_party_pilot tr.details td, 
                    div.thid_party_services tr.details td,
                    div.wire_lifts tr.details td,
                    div.permits tr.details td
                    {
                    	border-top: 1px solid #E9E9E9;  
                    	padding-top: 10px;                  	
                    }

                    div.third_party_pilot tr.comments td, 
                    div.thid_party_services tr.comments td,
                    div.wire_lifts tr.comments td,
                    div.permits tr.comments td
                    {
                        padding: 5px 10px;
                        padding-bottom: 10px;                        
                    }
                    
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
                    	font-size: 12pt;
                        padding: 0;
                    }

                    div.page_break
                    {
                        border: none;
                        display: block;
                        page-break-before: always;
                        margin: 0;
                    }
                </style>
            ";

            return content;
        }

        private static string GetHeader(Dispatch dispatch, string copyType)
        {           
            var content = @"
                <div class=""header"">
                    <table>
                        <tr>
                            <td class=""logo_col"">
                                <span class=""logo""><img src=""%HEADER_IMG%"" alt=""Singer Specialized""></span>
                            </td>
                            <td class=""address_col"">
                                <span>%COMPANY_NAME%</span>
                                <span>%STREET_ADDRESS%</span>
                                <span>%CITY%</span>
                                <span>Phone: %PHONE%</span>
                            </td>
                            <td class=""id_col"">
                                <span class=""copy_type"">%COPY%</span>
                                <span>Dispatch #:</span>
                                <span class=""number"">%DISPATCH_NUMBER%</span>
                            </td>
                        </tr>
                    </table>
                    
                    <span class=""title"">Dispatch Order</span>            
                </div>
            ";

            var company = SingerConstants.GetConfig("SingerName");
            var address = SingerConstants.GetConfig("SingerAddress-StreetAddress");
            var city = SingerConstants.GetConfig("SingerAddress-City");
            var phone = SingerConstants.GetConfig("SingerAddress-Phone");

            var process = System.Diagnostics.Process.GetCurrentProcess();
            var img = SingerConstants.GetConfig("Documents-HeaderImg");

            if (img == null && process.MainModule != null)
            {
                img = "file:///" + System.IO.Path.Combine(System.IO.Path.GetDirectoryName(process.MainModule.FileName), @"Images\Header.png");
            }

            var name = (dispatch != null) ? dispatch.Name : "UNKNOWN";
            
            content = content.Replace("%HEADER_IMG%", img).Replace("%COPY%", copyType).Replace("%DISPATCH_NUMBER%", name);
            content = content.Replace("%COMPANY_NAME%", company).Replace("%STREET_ADDRESS%", address).Replace("%CITY%", city).Replace("%PHONE%", phone);

            return content;
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

            var replacements = new string[10];

            replacements[0] = DateTime.Now.ToString(SingerConstants.PrintedDateFormatString);
            replacements[1] = dispatch.Job.Company.Name;
            replacements[2] = (dispatch.Equipment != null) ? dispatch.Equipment.UnitNumber : "";
            replacements[3] = (dispatch.Load != null && dispatch.Load.Rate != null) ? dispatch.Load.Rate.Name + " - " : "";

            if (dispatch.Load != null && dispatch.Load.TrailerCombination != null)
                replacements[3] += dispatch.Load.TrailerCombination.Combination;

            replacements[4] = (dispatch.Employee != null) ? dispatch.Employee.Name : "";
            replacements[5] = "-- not implemented --";
            replacements[6] = (dispatch.MeetingTime != null) ? dispatch.MeetingTime.Value.ToString(SingerConstants.PrintedDateFormatString) + " " + dispatch.MeetingTime.Value.ToString(SingerConstants.PrintedTimeFormatString) : "";
            replacements[7] = dispatch.DepartingLocation;
            replacements[8] = dispatch.DepartingUnits;            
            
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

        private static string GetEquipment()
        {
            // Fill this section with all of the ExtraEquipement entities attached to the dispatch's load.

            const string content = @"
                <div class=""equipment_requirements section"">
                    <span class=""heading"">Equipment Required Information</span>
                    
                </div>
            ";

            return content;
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

        private static string GetLoadCommodities(EntitySet<JobCommodity> commodities)
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
                                    <td class=""date"">{4}</td>
                                    <td class=""time"">{5}</td>
                                    <td class=""location"">{6}</td>
                                    <td class=""contact"">{7}</td>
                                    <td class=""company"">{8}</td>
                                    <td class=""contact"">{9}</td>
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
                                    <td class=""date"">{13}</td>
                                    <td class=""time"">{14}</td>
                                    <td class=""location"">{15}</td>
                                    <td class=""contact"">{16}</td>
                                    <td class=""company"">{17}</td>
                                    <td class=""contact"">{18}</td>
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
                                    <td>{19}</td>
                                    <td>{20}</td>
                                    <td>{21}</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            ";

            var html = new StringBuilder();

            html.Append(head);

            for (var i = 0; i < commodities.Count; i++)
            {
                var item = commodities[i];
                var reps = new string[22];

                var length = (item.Length != null) ? item.Length.Value : 0.00;
                var width = (item.Width != null) ? item.Width.Value : 0.00;
                var height = (item.Height != null) ? item.Height.Value : 0.00;
                
                reps[0] = item.Name;
                reps[1] = (item.Unit != null) ? string.Format("Unit {0}", item.Unit) : "";
                reps[2] = string.Format("{0:0,0.00}kg", item.Weight);
                reps[3] = string.Format("{0:0,0.00}m x {1:0,0.00}m x {2:0,0.00}m (LxWxH)", item.Length, item.Width, item.Height);

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
                reps[7] = (item.LoadContact != null) ? string.Format("<span>{0}</span><span>{1}</span>", item.LoadContact.Name, item.LoadContact.PrimaryPhone) : "";
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
                reps[16] = (item.UnloadContact != null) ? string.Format("<span>{0}</span><span>{1}</span>", item.UnloadContact.Name, item.UnloadContact.PrimaryPhone) : "";
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

        private static string GetDimensions(Load load)
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
                            <td class=""row_name"">Estimated</th>                            
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
                            <td class=""row_name"">Scaled</th>                            
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

            var replacements = new String[28];

            replacements[0] = Convert.ToString(load.LoadedLength);
            replacements[1] = Convert.ToString(load.LoadedWidth);
            replacements[2] = Convert.ToString(load.LoadedHeight);
            replacements[3] = Convert.ToString(load.GrossWeight);
            replacements[4] = Convert.ToString(load.EWeightSteer);
            replacements[5] = Convert.ToString(load.EWeightDrive);
            replacements[6] = Convert.ToString(load.EWeightGroup1);
            replacements[7] = Convert.ToString(load.EWeightGroup2);
            replacements[8] = Convert.ToString(load.EWeightGroup3);
            replacements[9] = Convert.ToString(load.EWeightGroup4);
            replacements[10] = Convert.ToString(load.EWeightGroup5);
            replacements[11] = Convert.ToString(load.EWeightGroup6);
            replacements[12] = Convert.ToString(load.EWeightGroup7);
            replacements[13] = Convert.ToString(load.EWeightGroup8);
            replacements[14] = Convert.ToString(load.EWeightGroup9);
            replacements[15] = Convert.ToString(load.EWeightGroup10);
            replacements[16] = Convert.ToString(load.SWeightSteer);
            replacements[17] = Convert.ToString(load.SWeightDrive);
            replacements[18] = Convert.ToString(load.SWeightGroup1);
            replacements[19] = Convert.ToString(load.SWeightGroup2);
            replacements[20] = Convert.ToString(load.SWeightGroup3);
            replacements[21] = Convert.ToString(load.SWeightGroup4);
            replacements[22] = Convert.ToString(load.SWeightGroup5);
            replacements[23] = Convert.ToString(load.SWeightGroup6);
            replacements[24] = Convert.ToString(load.SWeightGroup7);
            replacements[25] = Convert.ToString(load.SWeightGroup8);
            replacements[26] = Convert.ToString(load.SWeightGroup9);
            replacements[27] = Convert.ToString(load.SWeightGroup10);

            return string.Format(content, replacements);
        }

        private static string GetTractors()
        {
            const string content = @"
                <div class=""tractors section"">
                    <span class=""heading"">Tractors (Singer Service)</span>
                    
                    <table>
                        <thead>
                            <tr>
                                <th>Unit</th>
                                <th>Contact</th>
                                <th>Trailer Combination</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td>03-27</td>
                                <td>Chad Congdon (403) 863-7209</td>
                                <td>18-76</td>
                            </tr>
                            <tr>
                                <td>03-22</td>
                                <td>Corey Cuthill (403) 999-5859</td>
                                <td>26-77</td>
                            </tr>
                            <tr>
                                <td>03-08</td>
                                <td>Chris Beutler (403) 852-9726</td>
                                <td>26-77</td>
                            </tr>
                            <tr>                    
                                <td>03-12</td>
                                <td>John Hall - (403) 861-7568</td>
                                <td>35-81,35-67,29-67</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            ";

            return content;
        }

        private static string GetSingerPilots()
        {
            const string content = @"
                <div class=""other_equipment section"">
                    <span class=""heading"">Pilot Car and Other Equipment (Singer Service)</span>
                    
                    <table>
                        <thead>
                            <tr>
                                <th>Unit</th>
                                <th>Contact</th>                        
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td>01-47</td>
                                <td>Cody LaFrance - (403) 796-3636</td>                        
                            </tr>
                            <tr>
                                <td>01-55</td>
                                <td>Jordy Cropley - (403) 816-1645 - Supervisor</td>
                            </tr>
                            <tr>
                                <td>99-09</td>
                                <td>Wyatt Singer - (403) 816-1640</td>
                            </tr>
                        </tbody>
                   </table>
                </div>
            ";

            return content;
        }

        private static string GetThirdPartyPilots()
        {
            const string content = @"
                <div class=""third_party_pilot section"">
                    <span class=""heading"">Pilot Car (Thrid Party)</span>
                </div>
            ";

            return content;
        }

        private static string GetThridPartyServices()
        {
            const string content = @"
                <div class=""thid_party_services section"">
                    <span class=""heading"">Third Party Services</span>
                    
                    <table>
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
                            <tr class=""details"">
                                <td>Jan 7, 2009 09:00</td>
                                <td>Light Swinging</td>
                                <td>City of Airdrie</td>
                                <td>Gayle</td>
                                <td>404 East Lake Rd, Airdrie</td>
                                <td>(403) 948-8415</td>
                                <td>Booked</td>
                            </tr>
                            <tr class=""comments"">
                                <td colspan=""7"">
                                    <span class=""field_name"">Comments:</span> Booked by Gayle for escort and light swinging
                                </td>
                            </tr>
                        </tbody>                
                    </table>
                </div>
            ";

            return content;
        }

        private static string GetWireLiftInfo()
        {
            const string content = @"
                <div class=""wire_lifts section"">
                    <span class=""heading"">Wire Lift Information</span>
                    
                    <table>
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
                            <tr class=""details"">
                                <td>Jan 7, 2009 08:00</td>
                                <td>Fortis Alberta</td>
                                <td>24 Hour Dispatch</td>
                                <td>404 East Lake Rd, Airdrie</td>
                                <td>(888) 251-3907</td>
                                <td>60066597</td>
                            </tr>
                            <tr class=""comments"">
                                <td colspan=""6"">
                                    <span class=""field_name"">Comments: </span> Fortis to meet, measure, and escort in Airdrie, and then escort in Sundre area
                                </td>
                            </tr>
                            <tr class=""details"">
                                <td>Jan 7, 2009 9:51</td>
                                <td>Telus Communications</td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr class=""comments"">
                                <td colspan=""6"">
                                    <span class=""field_name"">Comments: </span> Cleared by Trevor
                                </td>
                            </tr>
                            <tr class=""details"">
                                <td>Jan 7, 2009 09:52</td>
                                <td>Shaw Cable</td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr class=""comments"">
                                <td colspan=""6"">
                                    <span class=""field_name"">Comments: </span> Cleared by Earl
                                </td>
                            </tr>
                            <tr class=""details"">
                                <td>Jan 7, 2009</td>
                                <td>Central Alberta REA</td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr class=""comments"">
                                <td colspan=""6"">
                                    <span class=""field_name"">Comments: </span> Cleared by CAREA
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            ";

            return content;
        }

        private static string GetPermits()
        {
            const string content = @"
                <div class=""permits section"">
                    <span class=""heading"">Permit Information</span>
                    
                    <table>
                        <thead>
                            <tr>
                                <th>Date</th>
                                <th>Issuer</th>
                                <th>Type</th>
                                <th>Number</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr class=""details"">
                                <td>Jan 7, 2009 to Jan 13, 2009</td>
                                <td>Alberta Transportation</td>
                                <td>Empty Trailer - Overweight and Overdimensional</td>
                                <td>08-162-4298 - To Aidrie</td>
                            </tr>
                            <tr class=""comments"">
                                <td colspan=""4"">
                                    <span class=""field_name"">Conditions:</span> See permit
                                </td>
                            </tr>
                            <tr class=""details"">
                                <td>Jan 7, 2009 to Jan 13, 2009</td>
                                <td>Alberta Transport</td>
                                <td>Overweight and Overdimensional</td>
                                <td>08-162-8380 - Loaded</td>
                            </tr>
                            <tr class=""comments"">
                                <td colspan=""4"">
                                    <span class=""field_name"">Conditions:</span> See permit
                                </td>
                            </tr>
                            <tr class=""details"">
                                <td>Jan 7, 2009 to Jan 13, 2009</td>
                                <td>Alberta Transportation</td>
                                <td>Empty Trailer - Overweight and Overdimensional</td>
                                <td>08-162-4397 - Return</td>
                            </tr>
                            <tr class=""comments"">
                                <td colspan=""4"">
                                    <span class=""field_name"">Conditions:</span> See permit
                                </td>
                            </tr>
                            <tr class=""details"">
                                <td>Jan 7, 2009 to Jan 13, 2009</td>
                                <td>County 17 Mountainview</td>
                                <td>Overweight and Overdimensional</td>
                                <td>PENDING</td>
                            </tr>
                            <tr class=""comments"">
                                <td colspan=""4"">
                                    <span class=""field_name"">Conditions:</span> For use of Twp320 and RR402
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            ";

            return content;
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


