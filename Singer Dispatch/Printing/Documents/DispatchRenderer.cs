using System;
using System.Collections.Generic;
using System.Text;

namespace SingerDispatch.Printing.Documents
{
    class DispatchRenderer : IRenderer
    {
        private const string PageBreak = @"<div class=""page_break""></div>";

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

            content.Append(FillDispatchBody(dispatch));

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

                content.Append(FillDispatchBody(dispatch));

                if ((i + 1) != dispatches.Count)
                    content.Append(PageBreak);
            }

            content.Append("</body>");
            content.Append("</html>");

            return content.ToString();
        }

        private static string FillDispatchBody(Dispatch dispatch)
        {
            var output = new StringBuilder();

            output.Append(GetHeader(dispatch));
            output.Append(GetDetails(dispatch));
            output.Append(GetDescription(dispatch.Description));
            output.Append(GetEquipment());
            output.Append(GetSchedule());
            output.Append(GetLoadInstructions());
            output.Append(GetDimensions(dispatch.Load));
            output.Append(GetTractors());
            output.Append(GetSingerPilots());
            output.Append(GetThirdPartyPilots());
            output.Append(GetThridPartyServices());
            output.Append(GetWireLiftInfo());
            output.Append(GetPermits());
            output.Append(GetOtherInfo());

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

        private static string GetHeader(Dispatch dispatch)
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
                                <span class=""copy_type"">FIle Copy</span>
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
            
            content = content.Replace("%HEADER_IMG%", img).Replace("%DISPATCH_NUMBER%", name);
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
                            <td class=""value col2_4"">%CURRENT_DATE%</td>
                            <td class=""field_name col3_4"">Customer:</td>
                            <td class=""value col4_4"">%CUSTOMER%</td>
                        </tr>
                        <tr>
                            <td class=""field_name"">Unit #:</td>
                            <td class=""value"">%UNIT%</td>
                            <td class=""field_name"">Trailer #:</td>
                            <td class=""value"">%TRAILER%</td>
                        </tr>
                        <tr>
                            <td class=""field_name"">Driver:</td>
                            <td class=""value"">%DRIVER%</td>
                            <td class=""field_name"">Swampers:</td>
                            <td class=""value""></td>
                        </tr>
                    </table>
                    
                    <table class=""departure_info"">
                        <tr>
                            <td class=""field_name col1_2"">Depart Date:</td>
                            <td class=""value col2_2"">%DISPATCH_DATE%</td>
                        </tr>                
                        <tr>                    
                            <td class=""field_name"">Depart From:</td>
                            <td class=""value"">Propak Main Yard</td>
                        </tr>
                        <tr>
                            <td class=""field_name"">Depart Units:</td>
                            <td class=""value"">#12,9909,55,47,18,42</td>
                        </tr>
                    </table>
                   
                    <table class=""customer_references"">
                        <tr>
                            <td class=""field_name col1_2"">Customer References:</td>
                            <td class=""value col2_2"">
                                %REFERENCE_NUMBERS%                                
                            </td>
                        </tr>
                    </table>
                </div>
            ";

            var date = DateTime.Now;
            var dispatchDate = dispatch.MeetingTime;
            var customer = dispatch.Job.Company.Name;
            var unit = (dispatch.Equipment != null) ? dispatch.Equipment.UnitNumber : "";
            var driver = (dispatch.Employee != null) ? dispatch.Employee.Name : "";
            var trailer = (dispatch.Load != null && dispatch.Load.Rate != null) ? dispatch.Load.Rate.Name + " - " : "";

            if (dispatch.Load != null && dispatch.Load.TrailerCombination != null)
                trailer += dispatch.Load.TrailerCombination.Combination;

            var output = html.Replace("%CURRENT_DATE%", date.ToShortDateString()).Replace("%CUSTOMER%", customer).Replace("%UNIT%", unit)
                .Replace("%DRIVER%", driver).Replace("%TRAILER%", trailer).Replace("%DISPATCH_DATE%", dispatchDate.ToString());

            var references = new StringBuilder();
            
            for (var i = 0; i < dispatch.Job.ReferenceNumbers.Count; i++) 
            {
                var item = dispatch.Job.ReferenceNumbers[i];

                references.Append(@"<span class=""reference""><span class=""field_name"">" + item.Field + @"</span>: <span class=""value"">" + item.Value + "</span></span>");

                if ((i + 1) != dispatch.Job.ReferenceNumbers.Count)
                    references.Append(", ");
            }

            if (references.Length > 0)
                output = output.Replace("%REFERENCE_NUMBERS%", references.ToString());

            return output;
        }

        private static string GetDescription(string description)
        {
            const string content = @"
                <div class=""description section"">
                    <span class=""heading"">Dispatch Description</span>
                    
                    <p>%DESCRIPTION%</p>
                </div>
            ";

            return content.Replace("%DESCRIPTION%", description);
        }

        private static string GetEquipment()
        {
            const string content = @"
                <div class=""equipment_requirements section"">
                    <span class=""heading"">Equipment Required Information</span>
                    
                </div>
            ";

            return content;
        }

        private static string GetSchedule()
        {
            const string content = @"
                <div class=""schuedule section"">
                    <span class=""heading"">Dispatch Schedule</span>
                    
                    <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce quis est sapien. Donec tempor, tortor at auctor scelerisque, justo elit condimentum enim, ac tristique nunc risus eu neque. Nam scelerisque nulla vel sapien facilisis ut commodo velit feugiat. Phasellus non mi ullamcorper neque porttitor semper at at dolor. Nullam ut erat at ipsum molestie tempus in nec lectus. Morbi risus ipsum, consectetur id laoreet ut, ullamcorper id urna. Duis a iaculis quam. Donec tempor, arcu eu cursus egestas, purus purus mattis velit, in suscipit neque metus nec augue. Nunc id justo vitae massa porta placerat sed vitae tellus. Aliquam erat.</p>
                </div>
            ";

            return content;
        }

        private static string GetLoadInstructions()
        {
            const string content = @"
                <div class=""load_and_unload section"">
                    <span class=""heading"">Load/Unload Information</span>
                    
                    <div class=""commodity"">
                        <span class=""commodity_name"">1450 HP Compressor Package Skid</span>
                        
                        <div class=""dimensions"">
                            <span class=""weight"">74843kg</span>
                            <span class=""size"">11.8m x 7.24m x 6.25m (L x W x H)</span>                    
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
                                        <td class=""date"">Jan 07, 2009</td>
                                        <td class=""time"">10:00</td>
                                        <td class=""location"">Propak Systems - 404 East Lake Rd, Airdrie</td>
                                        <td class=""contact""><span>Rob Ogle</span><span>(403) 333 - 5369</span></td>
                                        <td class=""company"">Singer Specialized</td>
                                        <td class=""contact""><span>Jordy Cropley</span><span>(403) 816 - 1645</span></td>                                        
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
                                        <td>84st-16ave-Deerfoot-2-567 east-right onto Eastlake rd to Propak</td>
                                        <td>Travel to Propack and load compressor by J&amp;R and load cooler with pickers</td>
                                        <td>Load compressor evenly between necks</td>
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
                                        <td class=""date"">Jan 7, 2009</td>
                                        <td class=""time"">14:00</td>
                                        <td class=""location"">Harmattan Gas Processiong Parnership - Harmatton-9-27-31-4-W4M</td>
                                        <td class=""contact""><span>Gord Fox</span><span>(403) 335 - 7528</span></td>
                                        <td class=""company"">Singer Specialized</td>
                                        <td class=""contact""><span>Jordy Cropley</span><span>(403) 816 - 1645</span></td>
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
                                        <td>Eastlake Rd north-567 west-772 south-567 west-22 north-Twp320(Bergen Rd) eas-RR402 south to site-Enter thru the Pipe yard</td>
                                        <td>Deliver both loads to Harmatton site in convoy and offload as directed</td>
                                        <td>See Gord Fox onsite for direction</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>

                    <hr>

                    <div class=""commodity"">
                        <span class=""commodity_name"">1450 HP Compressor Package Skid</span>

                        <div class=""dimensions"">
                            <span class=""weight"">74843kg</span>
                            <span class=""size"">11.8m x 7.24m x 6.25m (L x W x H)</span>
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
                                        <td class=""date"">Jan 7, 2009</td>
                                        <td class=""time"">14:00</td>
                                        <td class=""location"">Harmattan Gas Processiong Parnership - Harmatton-9-27-31-4-W4M</td>
                                        <td class=""contact""><span>Gord Fox</span><span>(403) 335 - 7528</span></td>
                                        <td class=""company"">Singer Specialized</td>
                                        <td class=""contact""><span>Jordy Cropley</span><span>(403) 816 - 1645</span></td>
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
                                        <td>Eastlake Rd north-567 west-772 south-567 west-22 north-Twp320(Bergen Rd) eas-RR402 south to site-Enter thru the Pipe yard</td>
                                        <td>Deliver both loads to Harmatton site in convoy and offload as directed</td>
                                        <td>See Gord Fox onsite for direction</td>
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
                                        <td class=""date"">Jan 07, 2009</td>
                                        <td class=""time"">10:00</td>
                                        <td class=""location"">Propak Systems - 404 East Lake Rd, Airdrie</td>
                                        <td class=""contact""><span>Rob Ogle</span><span>(403) 333 - 5369</span></td>
                                        <td class=""company"">Singer Specialized</td>
                                        <td class=""contact""><span>Jordy Cropley</span><span>(403) 816 - 1645</span></td>
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
                                        <td>84st-16ave-Deerfoot-2-567 east-right onto Eastlake rd to Propak</td>
                                        <td>Travel to Propack and load compressor by J&amp;R and load cooler with pickers</td>
                                        <td>Load compressor evenly between necks</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            ";

            return content;
        }

        private static string GetDimensions(Load load)
        {
            const string content = @"
                <div class=""dimensions section"">
                    <span class=""heading"">Dimensional Information</span>
                    
                    <table class=""dimensions"">
                        <tr>
                            <td><span class=""field_name"">Loaded Length</span>: <span class=""value"">40.00</span></td>
                            <td><span class=""field_name"">Loaded Width</span>: <span class=""value"">7.16</span></td>                    
                            <td><span class=""field_name"">Loaded Height</span>: <span class=""value"">6.82</span></td>
                            <td><span class=""field_name"">Gross Weight</span>: <span class=""value"">128,393</span></td>
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
                            <th class=""row_name"">Estimated</th>
                            <td>7300</td>
                            <td>21000</td>
                            <td>28000</td>
                            <td>28000</td>
                            <td>28000</td>
                            <td>28000</td>
                            <td>&nbsp;</td>
                            <td>&nbsp;</td>
                            <td>&nbsp;</td>
                            <td>&nbsp;</td>
                            <td>&nbsp;</td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <th class=""vertical"">Scaled</th>
                            <td>7300</td>
                            <td>&nbsp;</td>
                            <td>&nbsp;</td>
                            <td>&nbsp;</td>
                            <td>&nbsp;</td>
                            <td>&nbsp;</td>
                            <td>&nbsp;</td>
                            <td>&nbsp;</td>
                            <td>&nbsp;</td>
                            <td>&nbsp;</td>
                            <td>&nbsp;</td>
                            <td>&nbsp;</td>
                        </tr>
                    </table>
                </div>
            ";

            return content;
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

        private static string GetOtherInfo()
        {
            const string content = @"
                <div class=""other_info section"">
                    <span class=""heading"">Other Information</span>

                    <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce quis est sapien. Donec tempor, tortor at auctor scelerisque, justo elit condimentum enim, ac tristique nunc risus eu neque. Nam scelerisque nulla vel sapien facilisis ut commodo velit feugiat. Phasellus non mi ullamcorper neque porttitor semper at at dolor. Nullam ut erat at ipsum molestie tempus in nec lectus. Morbi risus ipsum, consectetur id laoreet ut, ullamcorper id urna. Duis a iaculis quam. Donec tempor, arcu eu cursus egestas, purus purus mattis velit, in suscipit neque metus nec augue. Nunc id justo vitae massa porta placerat sed vitae tellus. Aliquam erat.</p>
                </div>
            ";

            return content;
        }
    }
}


