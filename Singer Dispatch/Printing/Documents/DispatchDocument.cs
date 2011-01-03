using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data.Linq;
using SingerDispatch.Utils;

// 

namespace SingerDispatch.Printing.Documents
{
    class DispatchDocument : SingerPrintDocument
    {
        private const string PageBreak = @"<div class=""page_break""></div>";
        
        public bool IncludeFileCopy { get; set; }
        
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

        public override string GenerateHTML(object entity)
        {
            if (entity is List<Dispatch>)
                return GenerateHTML((List<Dispatch>)entity);

            return GenerateHTML((Dispatch)entity);
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
            output.Append(GetDimensions(dispatch.Load));
            output.Append(GetLoadCommodities(dispatch.Load.LoadedCommodities));
            output.Append(GetTractors(from t in dispatch.Load.Dispatches where t.Equipment != null && t.Equipment.EquipmentType != null && t.Equipment.EquipmentType.EquipmentClass != null && t.Equipment.EquipmentType.EquipmentClass.Name == "Tractor" select t));
            output.Append(GetSingerPilots(from p in dispatch.Load.Dispatches where p.Equipment != null && p.Equipment.EquipmentType != null && p.Equipment.EquipmentType.EquipmentClass != null && p.Equipment.EquipmentType.EquipmentClass.Name == "Pilot" select p));
            output.Append(GetThirdPartyPilots(from s in dispatch.Load.ThirdPartyServices where s.ServiceType != null && s.ServiceType.Name.Contains("Pilot") select s));
            output.Append(GetThridPartyServices(from s in dispatch.Load.ThirdPartyServices where s.ServiceType == null || (!s.ServiceType.Name.Contains("Pilot") && !s.ServiceType.Name.Contains("Wirelift")) select s));
            output.Append(GetWireLiftInfo(from wl in dispatch.Load.ThirdPartyServices where wl.ServiceType != null && wl.ServiceType.Name.Contains("Wirelift") select wl));
            output.Append(GetPermits(dispatch.Load.Permits));
            output.Append(GetOtherInfo(dispatch));
            output.Append("</div>");

            if (dispatch.Load != null && dispatch.Equipment != null && dispatch.Equipment == dispatch.Load.Equipment)
            {
                int copies;

                if (copyType == "Driver Copy")
                {
                    if (SpecializedDocument)
                        copies = Convert.ToInt32(SingerConfigs.GetConfig("Dispatch-SingerBoLDriverCopies") ?? "1");
                    else
                        copies = Convert.ToInt32(SingerConfigs.GetConfig("Dispatch-EnterpriseBoLDriverCopies") ?? "1");
                }
                else
                {
                    if (SpecializedDocument)
                        copies = Convert.ToInt32(SingerConfigs.GetConfig("Dispatch-SingerBoLFileCopies") ?? "1");
                    else
                        copies = Convert.ToInt32(SingerConfigs.GetConfig("Dispatch-EnterpriseBoLFileCopies") ?? "1");
                }

                for (var i = 0; i < copies; i++)
                {
                    output.Append(PageBreak);
                    output.Append(GetBillOfLadingDocs(dispatch));
                }
            }

            return output.ToString();
        }

        private string GetBillOfLadingDocs(Dispatch dispatch)
        {
            if (dispatch.Load == null) return "";

            var doc = new BillOfLadingDocument();
            var content = new StringBuilder();

            doc.SpecializedDocument = SpecializedDocument;
            doc.PrintMetric = PrintMetric;

            for (var i = 0; i < dispatch.Load.LoadedCommodities.Count; i++)
            {
                var commodity = dispatch.Load.LoadedCommodities[i];

                content.Append(doc.GenerateBodyHTML(dispatch, commodity));

                if ((i + 1) != dispatch.Load.LoadedCommodities.Count)
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
                        vertical-align: top;
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
                    
                    div.dispatch_doc div.header td.cod_col span
                    {
                        display: block;
                        text-align: center;
                        font-weight: bold;
                        font-size: 2.5em;                        
                        line-height: 1.1em;
                        padding: 15px 5px;
                        border: 1px #565656 solid;
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
                    	padding: 10px;                    	
                    }

                    div.dispatch_doc hr
                    {
                    	border: 0;
                    	height: 1px;
                    	background-color: #D9D9D9;
                    }

                    div.dispatch_doc div.load_and_unload span.commodity_name
                    {
                        font-size: 1.1em;
                        text-decoration: underline;
                        display: block;
                        font-weight: bold;
                        margin-bottom: 15px;
                        margin-left: -10px;
                    }

                    div.dispatch_doc div.load_and_unload div.dimensions
                    {
                        display: block;
                        padding: 1px;
                    }                    

                    div.dispatch_doc div.load_and_unload span.commodity_name span.unit
                    {
                        font-weight: normal;          
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

                    div.dispatch_doc div.load_and_unload table.details td span.inner
                    {
                        display: block;
                        min-height: 25px;
                    }

                    div.dispatch_doc div.load_and_unload table.details td span.contact
                    {
                        display: block;
                    }

                    div.dispatch_doc div.load_and_unload table.details td.date
                    {
                    	width: 70px;
                    }
                    
                    div.dispatch_doc div.load_and_unload table.details td.time
                    {
                    	width: 45px;
                    }

                    div.dispatch_doc div.load_and_unload table.details td.location span.inner span
                    {
                        display: block;
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

                    div.dispatch_doc div.load_and_unload table.instructions td span.inner
                    {
                        display: block;
                        min-height: 25px;
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
                        padding: 5px;
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
                            <td class=""cod_col"">
                                {7}
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

            var replacements = new object[8];
            string cName, cAddress, cCity, cPhone;

            if (SpecializedDocument)
            {
                cName = "SingerName";
                cAddress = "SingerAddress-StreetAddress";
                cCity = "SingerAddress-City";
                cPhone = "SingerAddress-Phone";
            }
            else
            {
                cName = "EnterpriseName";
                cAddress = "EnterpriseAddress-StreetAddress";
                cCity = "EnterpriseAddress-City";
                cPhone = "EnterpriseAddress-Phone";
            }

            replacements[0] = GetHeaderImg();
            replacements[1] = SingerConfigs.GetConfig(cName) ?? "Singer Specialized Ltd.";
            replacements[2] = SingerConfigs.GetConfig(cAddress);
            replacements[3] = SingerConfigs.GetConfig(cCity);
            replacements[4] = SingerConfigs.GetConfig(cPhone);
            replacements[5] = copyType;
            replacements[6] = (dispatch != null) ? dispatch.Name : "UNKNOWN";

            if (dispatch.Load.Job.Company.CompanyPriorityLevel.Name.EndsWith("Cash on Delivery"))
                replacements[7] = "<span>C.O.D.<br>Collect</span>";            

            return string.Format(html, replacements);
        }

        private static string GetDetails(Dispatch dispatch)
        {
            const string html = @"
                <div class=""details"">
                    {0}
                    
                    {1}
                   
                    {2}
                </div>
            ";
            string dispatchInfo = @"
                    <table class=""dispatch_info"">
                        <tr>
                            <td class=""field_name"">Customer:</td>
                            <td class=""value"" colspan=""3"">{0}</td>                            
                        </tr>
                        <tr>
                            <td class=""field_name"">Unit #:</td>
                            <td class=""value"">{1}</td>
                            <td class=""field_name"">{5}</td>
                            <td class=""value"">{2}</td>
                        </tr>
                        <tr>
                            <td class=""field_name"">Driver:</td>
                            <td class=""value"">{3}</td>
                            <td class=""field_name"">Swampers:</td>
                            <td class=""value"">{4}</td>
                        </tr>
                    </table>
            ";
            string departureInfo = @"
                    <table class=""departure_info"">
                        {0}                        
                    </table>
            ";
            string referenceInfo = @"
                    <table class=""customer_references"">
                        {0}
                    </table>
            ";
            string rowTemplate = @"
                <tr>                    
                    <td class=""field_name"">{0}</td>
                    <td class=""value"">{1}</td>
                </tr>
            ";

            var dispatchReplacements = new object[6];

            dispatchReplacements[0] = dispatch.Load.Job.Company.Name;

            if (dispatch.Equipment != null)
            {
                dispatchReplacements[1] = dispatch.Equipment.UnitNumber;

                if (dispatch.Equipment.EquipmentType.EquipmentClass.Name == "Tractor")
                {
                    dispatchReplacements[5] = "Trailer:";
                    dispatchReplacements[2] = (dispatch.Load != null && dispatch.Load.Rate != null) ? dispatch.Load.Rate.Name + " - " : "";

                    if (dispatch.Load != null && dispatch.Load.TrailerCombination != null)
                        dispatchReplacements[2] += dispatch.Load.TrailerCombination.Combination;
                }
                else
                {
                    dispatchReplacements[5] = "Responsibility:";
                    dispatchReplacements[2] = (dispatch.EquipmentType != null) ? dispatch.EquipmentType.Name : "";
                }
            }

            dispatchReplacements[3] = (dispatch.Employee != null) ? string.Format("{0} {1}", dispatch.Employee.Name, dispatch.Employee.Phone) : "";
                        
            
            var swampers = new StringBuilder();
            foreach (var swamper in dispatch.Swampers)
            {
                if (swamper.Employee != null)
                {
                    swampers.Append(string.Format("{0} {1}", swamper.Employee.Name, swamper.Employee.Phone));
                    swampers.Append("; ");
                }
            }

            dispatchReplacements[4] = swampers.ToString();
            

            var departureReplacement = new StringBuilder();

            if (dispatch.DispatchedBy != null)
                departureReplacement.Append(string.Format(rowTemplate, "Dispatched By", dispatch.DispatchedBy.Name));

            if (dispatch.MeetingDate != null)
                departureReplacement.Append(string.Format(rowTemplate, "Departing Date", dispatch.MeetingDate.Value.ToString(SingerConfigs.PrintedDateFormatString) + " " + dispatch.MeetingTime));
            
            if (!string.IsNullOrEmpty(dispatch.DepartingLocation))
                departureReplacement.Append(string.Format(rowTemplate, "Departing Location", dispatch.DepartingLocation));

            if (!string.IsNullOrEmpty(dispatch.DepartingUnits))
                departureReplacement.Append(string.Format(rowTemplate, "Departing Units", dispatch.DepartingUnits));
            

            // List any reference numbers given to this job            
            var references = new StringBuilder();

            if (dispatch.Load.ReferenceNumbers.Count == 0)
            {
                for (var i = 0; i < dispatch.Load.Job.ReferenceNumbers.Count; i++)
                {
                    var item = dispatch.Load.Job.ReferenceNumbers[i];

                    references.Append(@"<span class=""reference""><span class=""field_name"">" + item.Field + @"</span>: <span class=""value"">" + item.Value + "</span></span>");

                    if ((i + 1) != dispatch.Load.Job.ReferenceNumbers.Count)
                        references.Append(", ");
                }
            }
            else
            {
                for (var i = 0; i < dispatch.Load.ReferenceNumbers.Count; i++)
                {
                    var item = dispatch.Load.ReferenceNumbers[i];

                    references.Append(@"<span class=""reference""><span class=""field_name"">" + item.Field + @"</span>: <span class=""value"">" + item.Value + "</span></span>");

                    if ((i + 1) != dispatch.Load.ReferenceNumbers.Count)
                        references.Append(", ");
                }
            }

            var referenceReplacement = (references.Length > 0) ? string.Format(rowTemplate, "Customer References", references.ToString()) : "";


            return string.Format(html, string.Format(dispatchInfo, dispatchReplacements), string.Format(departureInfo, departureReplacement), string.Format(referenceInfo, referenceReplacement));
        }

        private static string GetDescription(string description)
        {
            const string html = @"
                <div class=""description section"">
                    <span class=""heading"">Dispatch Description</span>
                    
                    <p>{0}</p>
                </div>
            ";

            if (string.IsNullOrEmpty(description))
                return "";

            return string.Format(html, description);
        }

        private static string GetEquipment(EntitySet<ExtraEquipment> equipment)
        {
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
                return "";

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

            if (string.IsNullOrEmpty(dispatch.Schedule))
                return "";

            return string.Format(content, dispatch.Schedule);
        }

        private string GetLoadCommodities(EntitySet<LoadedCommodity> commodities)
        {
            const string head = @"<div class=""load_and_unload section""><span class=""heading"">Load/Unload Information</span>";
            const string divider = "<hr>";
            const string foot = "</div>";
            const string commodityHtml = @"                            
                <div class=""commodity"">
                    <span class=""commodity_name"">{0} <span class=""unit"">{1}</span></span> 
                    

                    <div class=""dimensions metric"">
                        <span class=""weight"">{2}</span>
                        <span class=""size"">{3}</span>
                    </div>

                    <div class=""dimensions imperial"">
                        <span class=""weight"">{20}</span>
                        <span class=""size"">{21}</span>
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
                                    <td class=""date""><span class=""inner"">{4}</span></td>
                                    <td class=""time""><span class=""inner"">{5}</span></td>
                                    <td class=""location""><span class=""inner"">{6}</span></td>
                                    <td class=""contact""><span class=""inner"">{7}</span></td>
                                    <td class=""company""><span class=""inner"">{8}</span></td>
                                    <td class=""contact""><span class=""inner"">{9}</span></td>
                                </tr>
                            </tbody>                        
                        </table>
                        
                        <table class=""instructions"">
                            <thead>
                                <tr>
                                    <th>Load Route</th>
                                    <th>Load Instruction</th>                                    
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td><span class=""inner"">{10}</span></td>
                                    <td><span class=""inner"">{11}</span></td>                                    
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
                                    <th>Unload Location</th>
                                    <th>Unload Site Contact</th>
                                    <th>Unloaded By</th>
                                    <th>Unloading Contact</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td class=""date""><span class=""inner"">{12}</span></td>
                                    <td class=""time""><span class=""inner"">{13}</span></td>
                                    <td class=""location""><span class=""inner"">{14}</span></td>
                                    <td class=""contact""><span class=""inner"">{15}</span></td>
                                    <td class=""company""><span class=""inner"">{16}</span></td>
                                    <td class=""contact""><span class=""inner"">{17}</span></td>
                                </tr>
                            </tbody>                        
                        </table>
                        
                        <table class=""instructions"">
                            <thead>
                                <tr>
                                    <th>Unload Route</th>
                                    <th>Unload Instruction</th>                                    
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td><span class=""inner"">{18}</span></td>
                                    <td><span class=""inner"">{19}</span></td>                                    
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
                var reps = new object[22];

                reps[0] = item.JobCommodity.Name;
                reps[1] = (item.JobCommodity.Unit != null) ? string.Format("Unit {0}", item.JobCommodity.Unit) : "";

                reps[2] = string.Format("{0}", MeasurementFormater.FromKilograms(item.JobCommodity.Weight, MeasurementFormater.UKilograms));
                reps[3] = string.Format("{0} x {1} x {2} (LxWxH)", MeasurementFormater.FromMetres(item.JobCommodity.Length, MeasurementFormater.UMetres), MeasurementFormater.FromMetres(item.JobCommodity.Width, MeasurementFormater.UMetres), MeasurementFormater.FromMetres(item.JobCommodity.Height, MeasurementFormater.UMetres));
                reps[20] = string.Format("{0}", MeasurementFormater.FromKilograms(item.JobCommodity.Weight, MeasurementFormater.UPounds));
                reps[21] = string.Format("{0} x {1} x {2} (LxWxH)", MeasurementFormater.FromMetres(item.JobCommodity.Length, MeasurementFormater.UFeet), MeasurementFormater.FromMetres(item.JobCommodity.Width, MeasurementFormater.UFeet), MeasurementFormater.FromMetres(item.JobCommodity.Height, MeasurementFormater.UFeet));


                if (item.LoadDate != null)
                {
                    reps[4] = item.LoadDate.Value.ToString(SingerConfigs.PrintedDateFormatString);
                    reps[5] = item.LoadTime;
                }
                else
                {
                    reps[4] = "";
                    reps[5] = "";
                }

                

                reps[6] = string.Format("<span>{0}</span><span>{1}</span>", item.LoadLocation, item.LoadAddress);
                reps[7] = (item.LoadSiteContact != null) ? string.Format(@"<span class=""contact"">{0}</span><span class=""contact"">{1}</span>", item.LoadSiteContact.Name, item.LoadSiteContact.PrimaryPhone) : "";
                reps[8] = (item.LoadingCompany != null) ? item.LoadingCompany.Name : "";
                reps[9] = (item.LoadingContact != null) ? string.Format(@"<span class=""contact"">{0}</span><span class=""contact"">{1}</span>", item.LoadingContact.Name, item.LoadingContact.PrimaryPhone) : ""; ;
                reps[10] = item.LoadRoute;
                reps[11] = item.LoadInstructions;                


                if (item.UnloadDate != null)
                {
                    reps[12] = item.UnloadDate.Value.ToString(SingerConfigs.PrintedDateFormatString);
                    reps[13] = item.UnloadTime;
                }
                else
                {
                    reps[12] = "";
                    reps[13] = "";
                }

                reps[14] = string.Format("<span>{0}</span><span>{1}</span>", item.UnloadLocation, item.UnloadAddress);
                reps[15] = (item.UnloadSiteContact != null) ? string.Format(@"<span class=""contact"">{0}</span><span class=""contact"">{1}</span>", item.UnloadSiteContact.Name, item.UnloadSiteContact.PrimaryPhone) : "";
                reps[16] = (item.UnloadingCompany != null) ? item.UnloadingCompany.Name : "";
                reps[17] = (item.UnloadingContact != null) ? string.Format(@"<span class=""contact"">{0}</span><span class=""contact"">{1}</span>", item.UnloadingContact.Name, item.UnloadingContact.PrimaryPhone) : ""; ; ;
                reps[18] = item.UnloadRoute;
                reps[19] = item.UnloadInstructions;
                
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
            replacements[3] = MeasurementFormater.FromKilograms(load.EGrossWeight, weightUnit);
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
                return "";

            var rows = new StringBuilder();
            foreach (var item in dispatches)
            {
                var replacements = new object[3];

                replacements[0] = (item.Equipment != null) ? item.Equipment.UnitNumber : "";
                replacements[1] = (item.Employee == null) ? "" : string.Format("{0} {1}", item.Employee.Name, item.Employee.Phone);
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
                return "";

            var rows = new StringBuilder();
            foreach (var item in dispatches)
            {
                var replacements = new object[2];
                var contact = (item.Employee == null) ? "" : string.Format("{0} {1}", item.Employee.Name, item.Employee.Phone);
                                
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
                return "";

            var rows = new StringBuilder();
            foreach (var item in pilots)
            {
                var replacements = new object[7];

                replacements[0] = (item.ServiceDate == null) ? "" : item.ServiceDate.Value.ToString(SingerConfigs.PrintedDateFormatString) + " " + item.ServiceDate.Value.ToString(SingerConfigs.PrintedTimeFormatString);                
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
                return "";

            var rows = new StringBuilder();
            foreach (var item in services)
            {
                var replacements = new object[7];

                replacements[0] = (item.ServiceDate == null) ? "" : item.ServiceDate.Value.ToString(SingerConfigs.PrintedDateFormatString) + " " + item.ServiceDate.Value.ToString(SingerConfigs.PrintedTimeFormatString);
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
                return "";

            var rows = new StringBuilder();
            foreach (var item in wirelifts)
            {
                var replacements = new object[6];

                replacements[0] = (item.ServiceDate == null) ? "" : item.ServiceDate.Value.ToString(SingerConfigs.PrintedDateFormatString) + " " + item.ServiceDate.Value.ToString(SingerConfigs.PrintedTimeFormatString);
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
                </tr>
            ";
            const string commentRow = @"
                <tr class=""comments"">
                    <td colspan=""4"">
                        <span><span class=""field_name"">{0}</span> {1}</span>
                    </td>
                </tr>
            ";

            if (permits.Count == 0)
                return "";

            var rows = new StringBuilder();

            foreach (var item in permits)
            {
                var replacements = new object[4];
                
                replacements[0] = (item.IssuingCompany == null) ? "" : item.IssuingCompany.Name;
                replacements[1] = (item.PermitType == null) ? "" : item.PermitType.Name;
                replacements[2] = item.Reference;

                rows.Append(string.Format(row, replacements));

                if (!string.IsNullOrEmpty(item.Conditions))
                    rows.Append(string.Format(commentRow, "Comments:", item.Conditions));
                else
                    rows.Append(string.Format(commentRow, "", ""));
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


