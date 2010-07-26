using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingerDispatch.Printing.Documents
{
    class BillOfLadingDocument : IPrintDocument
    {
        public bool PrintMetric { get; set; }
        public bool SpecializedDocument { get; set; }

        public BillOfLadingDocument()
        {
            PrintMetric = true;
            SpecializedDocument = true;        
        }
        
        public string GenerateHTML(object entity)
        {
            return GenerateHTML((Commodity)entity);
        }

        public string GenerateHTML(Commodity commodity)
        {
            var content = new StringBuilder();

            content.Append(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd"">");
            content.Append("<html>");
            content.Append("<head>");
            content.Append(@"<meta http-equiv=""Content-Type"" content=""text/html;charset=utf-8"">");
            content.Append(GetTitle("Bill of Lading - Non Negotiable"));
            content.Append(GetStyles());
            content.Append("</head>");
            content.Append("<body>");

            // Fill document body...
            content.Append(GetHeader("[document number]"));


            content.Append("</body>");
            content.Append("</html>");

            return content.ToString();
        }

        private static string GetTitle(string title)
        {
            return "<title>" + title + "</title>";
        }

        private string GetStyles()
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
            
                    /*******/

                    body
                    {
                        font-size: 8pt;
                        font-family: Verdana, Arial, Helvetica, sans-serif;
                        padding: 10px;
                    } 
            
                    span.heading
                    {
                        display: block;
                        font-weight: bold;
                        text-decoration: underline;
                    }  
            
                    p
                    {
                        margin-bottom: 5px;
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
                        font-size: 1.3em;                     
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
                        border-top: 2px #000000 solid;
                        border-bottom: 1px #000000 solid;
                    }
            
                    div.reference
                    {
                        border-left: 1px #000000 solid;
                        border-right: 1px #000000 solid;
                    }             
            
                    div.reference table
                    {
                        width: 100%;
                        border-collapse: collapse;
                    }
                    
                    div.reference td.legal
                    {
                        width: 60%;
                        border-left: 1px #000000 solid;
                        font-size: 0.9em;
                    }    
            
                    div.reference td.recipient div.shipper span, div.reference td.recipient div.consignee span
                    {
                        display: block;                
                    }
            
                    div.reference td.recipient span.date, div.reference td.recipient span.customer, div.reference td.recipient span.equipment
                    {
                        display: block;
                        font-weight: bold;
                        padding: 3px 7px;
                        border-bottom: 1px #000000 solid;
                    }
            
                    div.reference td.recipient span.date span, div.reference td.recipient span.customer span, div.reference td.recipient span.equipment span
                    {
                        font-weight: normal;
                    }
            
                    div.reference td.recipient span.unit, div.reference td.recipient span.trailer
                    {
                        font-weight: bold !important;
                    }
            
                    div.reference td.recipient div.shipper 
                    {
                        padding: 3px 7px;
                        border-bottom: 1px #000000 solid;
                    }
            
                    div.reference td.recipient div.consignee
                    {
                        padding: 3px 7px;
                    }
            
                    div.reference td.legal div
                    {                
                        padding: 5px 7px;
                    }
            
                    div.glass_damage span
                    {
                        display: block;
                        clear: both;
                        text-align: center;
                        text-transform: uppercase;
                        padding: 1px;
                        border: 1px #000000 solid;                
                    }
            
                    div.commodity
                    {               
                
                        border-left: 1px #000000 solid;
                        border-right: 1px #000000 solid;
                        border-bottom: 1px #000000 solid;
                    }

                    div.commodity table.commodity_details
                    {
                        width: 100%;
                        border-collapse: collapse;
                    }
            
                    div.commodity table.commodity_details td.charges
                    {
                        width: 33%;
                    }
            
                    div.commodity div.identification,
                    div.commodity div.service_description,
                    div.commodity div.weight
                    {
                        padding: 5px;                
                    }

                    div.commodity div.identification,
                    div.commodity div.service_description
                    {
                        border-bottom: 1px #000000 solid;
                    }

                    div.commodity div.identification span.heading,
                    div.commodity div.weight span.heading
                    {
                        display: inline;
                    }
            
                    div.commodity div.service_description p
                    {
                        font-weight: bold;
                        margin: 5px 0 15px 0;
                    }
            
                    div.commodity table.dimensions
                    {
                        width: 100%;
                    }
            
                    div.commodity table.dimensions td
                    {
                        padding-bottom: 3px;
                    }
            
                    div.commodity table.dimensions td.heading
                    {
                        width: 13%;
                    }
            
                    div.commodity table.dimensions span.name,
                    div.commodity table.dimensions span.length,
                    div.commodity table.dimensions span.width,
                    div.commodity table.dimensions span.height
                    {
                        font-weight: bold;
                    }
            
                    div.commodity td.charges
                    {
                        padding: 5px;
                        border-left: 1px #000000 solid;
                    }
            
                    div.commodity table.payment_types
                    {
                        width: 100%;
                        margin-top: 3px;
                    }
            
                    div.commodity table.payment_types span.checkbox span.box
                    {
                        display: block;
                        height: 10px;
                        width: 10px;
                        border: 1px #787878 solid;
                        float: left;
                        margin-right: 5px;
                    }
            
                    div.commodity span.disclaimer
                    {
                        display: block;
                        font-size: 0.8em;
                        margin: 3px 0;
                    }
                       
                    div.commodity table.amounts
                    {
                        width: 100%;
                        border-collapse: collapse;
                    }
            
                    div.commodity table.amounts th
                    {
                        text-decoration: underline;
                        width: 25%;
                        border-right: 1px #000000 dotted;
                        border-top: 1px #000000 dotted;
                        padding: 5px;
                        font-weight: normal;
                    }
            
                    div.commodity table.amounts td
                    {
                        border-top: 1px #000000 dotted;
                
                    }
            
                    table.additional_info
                    {
                        width: 100%;
                        border-collapse: collapse;
                        border-left: 1px #000000 solid;
                        border-right: 1px #000000 solid;
                        border-bottom: 1px #000000 solid;
                    }
            
                    table.additional_info td
                    {                
                        vertical-align: top;
                    }

                    table.additional_info td div.content
                    {
                        padding: 5px;
                        min-height: 60px;
                    }

                    td.dangerous_goods, td.shippers_weight, td.notice_of_claim, td.loading_declaration
                    {
                        border-right: 1px #000000 solid;
                    }
            
                    td.dangerous_goods, td.comments_and_initials, td.shippers_weight, td.shipper_per, td.notice_of_claim,
                    td.declared_value, td.shipper_originating
                    {
                        width: 50%;
                        border-bottom: 1px #000000 solid;
                    }
            
                    td.shippers_weight p, td.notice_of_claim p, td.declared_value span.subtext
                    {
                        font-size: 0.8em;
                    }
            
                    td.declared_value span.value
                    {
                        float: right;
                        font-weight: bold;
                        font-size: 1.1em;
                    }
            
                    td.declared_value span.subtext
                    {
                        display: block;
                        width: 70%;
                    }
            
                    span.signline
                    {
                        display: block;
                        border-bottom: 1px dotted #000000;
                        font-weight: bold;
                        padding: 5px 0;
                        margin: 15px 5px;
                    }
            
                    div.declared_value
                    {
                
                    }
            
                    span.signline span.subtext
                    {
                        font-size: 0.7em;
                        font-weight: normal;
                        float: right;
                        color: #676767;
                    }

                    table.declaration
                    {
                        width: 100%;
                        margin-bottom: 10px;
                    }            
            
                    table.declaration th.date, table.declaration th.time 
                    {
                        width: 28%;
                        font-weight: normal;
                    }           
            
                    table.declaration td span
                    {
                        display: block;
                        padding: 5px;
                    }
            
                    table.declaration td.title
                    {
                        border-bottom: 1px #000000 dotted;
                    }
            
            
                    table.declaration td.date
                    {
                        width: 200px;
                    }
            
                    table.declaration td.fill
                    {                
                        border-left: 1px #000000 dotted; 
                        border-bottom: 1px #000000 dotted;
                    }
            
                    table.declaration td.total
                    {
                        text-align: right;
                    }
            
                    table.signatures
                    {
                        width: 100%;
                        border: 1px #000000 solid;
                    }
                </style>
                <style type=""text/css"" media=""print"">
                    body
                    {
                        font-size: 10pt;
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

        private string GetHeader(string documentID)
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
                                <span>Document #:</span>
                                <span class=""number"">{5}</span>
                            </td>
                        </tr>
                    </table>
                    
                    <span class=""title"">Bill of Lading - Non Negotiable</span>
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
            replacements[5] = documentID;            

            return string.Format(html, replacements);
        }

        private string GetReferenceTable()
        {
            var html = @"
                <div class=""reference"">
                    <table>
                        <tr>
                            <td class=""recipient"">
                                <div>
                                    <span class=""date"">Date: <span>{0}July 08, 2010</span></span>

                                    <span class=""customer"">Customer: <span>{1}Transtech Contracting Inc.</span></span>

                                    <span class=""equipment""><span class=""unit"">Unit #: <span>{2}03-09</span></span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <span class=""trailer"">Trailer #: <span>{3}18-01</span></span></span>

                                    <div class=""shipper"">
                                        <span class=""heading"">Shipper or Agent (Name Address)</span>

                                        <span>{4}Transtech Contracting Inc.</span>
                                        <span>{5}Suite 811 - 53016 Hwy 60</span>
                                        <span>{6}Acheson, Alberta</span>
                                        <span>{7}Canada</span>
                                        <span>{8}T7X 5A7</span>
                                    </div>

                                    <div class=""consignee"">
                                        <span class=""heading"">Consignee (Name Address)</span>

                                        <span>{9}Transtech Contracting Inc.</span>
                                        <span>{10}Suite 811 - 53016 Hwy 60</span>
                                        <span>{11}Acheson, Alberta</span>
                                        <span>{12}Canada</span>
                                        <span>{13}T7X 5A7</span>
                                    </div>
                                </div>
                            </td>
                            <td class=""legal"">
                                <div>
                                    <p>COMBINATION SHORT FORM OF STRAIGHT BILL OF LADING - EXPRESS SHIPPING CONTRACT ADOPTED BY RAIL FREIGHT AND EXPRESS CARRIER'S SUBJECT TO THE JURISDICTION OF THE CANADIAN TRANSPORT COMMISION. ISSUED AT SHIPPER'S REQUEST.</p>

                                    <p>RECIEVED AT THE POINT OF ORIGIN ON THE DATE SPECIFIED, FROM THE CONSIGNOR MENTIONED HEREIN, THE PROPERTY HEREIN DESCRIBED, IN APPARENT GOOD ORDER, EXCEPT AS NOTED (CONTENTS OF PACKAGES AND CONDITIONS OF CONTENTS ARE UNKNOWN) MARKED, CONSIGNED AND DESTINED AS INDICATED BELOW, WHICH THE CARRIER AGREES TO CARRY AND TO DELIVER TO THE CONSIGNEE AT THE SAID DESTINATION, IF ON ITS OWN AUTHORIZED ROUTE OR OTHERWISE TO CAUSE TO BE CARRIED BY ANOTHER CARRIER ON THE ROUTE TO SAID DESTINATION SUBJECT TO THE RATES AND CLASSIFICATION IN EFFECT ON THE DATE OF SHIPMENT. IT IS MUTUALLY AGREED, AS TO EACH CARRIER OF ALL OR ANY OF THE GOODS OVERALL OR ANY PORTION OF THE ROUTE TO DESTINATION; AND AS TO EACH PARTY OF ANY TIME INTERESTED IN ALL OR ANY OF THE GOODS, THAT EVERY SERVICE TO BE PERFORMED HEREUNDER SHALL BE SUBJECT TO ALL THE CONDITIONS NOTE PROHIBITED BY LAW, WHETHER PRINTED OR WRITTEN, INCLUDING CONDITIONS ON BACK HEREOF, WHICH ARE HEREBY AGREED BY THE CONSIGNOR ACCEPTED FOR HIMSELF AND HIS ASSIGNS.</p>                            
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
            ";

            return "";
        }
    }
}
