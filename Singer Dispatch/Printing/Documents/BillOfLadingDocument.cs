using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SingerDispatch.Utils;

namespace SingerDispatch.Printing.Documents
{
    class BillOfLadingEntity
    {
        public Dispatch Dispatch { get; set; }
        public LoadedCommodity LoadedCommodity { get; set; }

        public BillOfLadingEntity(Dispatch dispatch, LoadedCommodity commodity)
        {
            Dispatch = dispatch;
            LoadedCommodity = commodity;
        }
    }

    class BillOfLadingDocument : SingerPrintDocument
    {
        public BillOfLadingDocument()
        {
            PrintMetric = true;
            SpecializedDocument = true;        
        }
        
        public override string GenerateHTML(object entity)
        {
            if (entity == null) return "";

            if (entity is BillOfLadingEntity)
            {
                var bol = (BillOfLadingEntity)entity;

                return GenerateHTML(bol.Dispatch, bol.LoadedCommodity);
            }
            else if (entity is LoadedCommodity)
            {
                var commodity = (LoadedCommodity)entity;

                return GenerateHTML(null, commodity);
            }
            else
                return "";
        }

        public string GenerateHTML(Dispatch dispatch, LoadedCommodity commodity)
        {
            if (commodity.JobCommodity == null) return "";

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
            content.Append(GenerateBodyHTML(dispatch, commodity));

            content.Append("</body>");
            content.Append("</html>");

            return content.ToString();
        }

        public string GenerateBodyHTML(Dispatch dispatch, LoadedCommodity commodity)
        {
            if (commodity.JobCommodity == null) return "";

            var content = new StringBuilder();
            string documentNumber;

            if (dispatch != null && dispatch.Load != null)
                documentNumber = string.Format("{0}-{1:D2}-{2}", dispatch.Load.Job.Number, dispatch.Load.Number, commodity.ID);
            else
                documentNumber = string.Format("{0}-00-{1:D2}", commodity.JobCommodity.Job.Number, commodity.ID);            


            content.Append(@"<div class=""bol_doc"">");
            content.Append(GetHeader(documentNumber));
            content.Append(GetReferenceTable(dispatch, commodity));
            content.Append(GetGlassLiability());
            content.Append(GetCommodityDetails(commodity));
            content.Append(GetAdditionalInfo(commodity));
            content.Append(GetSignatures());
            content.Append(@"</div>");

            return content.ToString();
        }

        private static string GetTitle(string title)
        {
            return "<title>" + title + "</title>";
        }
              
        private string GetStyles()
        {
            var content = @"
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
                    
                    body
                    {
                        font-size: 8pt;
                        font-family: sans-serif;
                        padding: 1em;
                    } 
            
                    /*******/

                    %BOL_SCREEN%
                </style>
                <style type=""text/css"" media=""print"">
                    body
                    {
                    	font-size: 8pt;
                        padding: 0;
                    }

                    %BOL_PRINT%
                </style>
            ";

            content = content.Replace("%BOL_SCREEN%", GetDocSpecificScreenStyles()).Replace("%BOL_PRINT%", GetDocSpecificPrintStyles());

            return content;
        }

        public static string GetDocSpecificScreenStyles()
        {
            var styles = @"                    
                    div.bol_doc span.heading
                    {
                        display: block;
                        font-weight: bold;
                        text-decoration: underline;
                    }  
            
                    div.bol_doc p
                    {
                        margin-bottom: 0.25em;
                    }
            
                    div.bol_doc div.header table 
                    {
                        width: 100%;
                        border-collapse: collapse;
                    }
                    
                    div.bol_doc div.header td
                    {                
                        vertical-align: top;
                        padding: 0.5em;
                    }
                    
                    div.bol_doc div.header td.logo_col
                    {
                        width: 300px;
                        
                    }
                    
                    div.bol_doc div.header td.id_col
                    {             
                        text-align: center;
                        font-weight: bold;
                        line-height: 1.35em;
                    }
                    
                    div.bol_doc div.header span
                    {
                        display: block;
                    }
                    
                    div.bol_doc div.header span.title
                    {
                        display: block;
                        font-weight: bold;
                        font-size: 1.4em;
                        padding: 0.5em 0.3em;
                        text-align: center;
                        border-top: 1px #000000 solid;
                        border-bottom: 1px #000000 solid;
                    }
            
                    div.bol_doc div.reference
                    {
                        border-left: 1px #000000 solid;
                        border-right: 1px #000000 solid;
                    }             
            
                    div.bol_doc div.reference table
                    {
                        width: 100%;
                        border-collapse: collapse;
                    }
                    
                    div.bol_doc div.reference td.legal
                    {
                        width: 60%;
                        border-left: 1px #000000 solid;
                        font-size: 0.7em;
                    }    
            
                    div.bol_doc div.reference td.recipient div.shipper span, 
                    div.bol_doc  div.reference td.recipient div.consignee span
                    {
                        display: block;                
                    }
            
                    div.bol_doc div.reference td.recipient span.date, 
                    div.bol_doc div.reference td.recipient span.customer, 
                    div.bol_doc div.reference td.recipient span.equipment
                    {
                        display: block;
                        font-weight: bold;
                        padding: 0.25em 0.4em;
                        border-bottom: 1px #000000 solid;
                    }
            
                    div.bol_doc div.reference td.recipient span.date span, 
                    div.bol_doc div.reference td.recipient span.customer span, 
                    div.bol_doc div.reference td.recipient span.equipment span
                    {
                        font-weight: normal;
                    }
            
                    div.bol_doc div.reference td.recipient span.unit, 
                    div.bol_doc div.reference td.recipient span.trailer
                    {
                        font-weight: bold !important;
                    }
            
                    div.bol_doc div.reference td.recipient div.shipper 
                    {
                        padding: 0.25em 0.4em;
                        border-bottom: 1px #000000 solid;
                    }
            
                    div.bol_doc div.reference td.recipient div.consignee
                    {
                        padding: 0.25em 0.4em;
                    }
            
                    div.bol_doc div.reference td.legal div
                    {                
                        padding: 0.3em 0.4em;
                    }
            
                    div.bol_doc div.glass_damage
                    {
                        clear: both;
                        text-align: center;
                        text-transform: uppercase;
                        padding: 0.1em;
                        border: 1px #000000 solid;                
                    }
            
                    div.bol_doc div.commodity
                    {               
                
                        border-left: 1px #000000 solid;
                        border-right: 1px #000000 solid;
                        border-bottom: 1px #000000 solid;
                    }

                    div.bol_doc div.commodity table.commodity_details
                    {
                        width: 100%;
                        border-collapse: collapse;
                    }
            
                    div.bol_doc div.commodity table.commodity_details td.charges
                    {
                        width: 33%;
                    }
            
                    div.bol_doc div.commodity div.identification,
                    div.bol_doc div.commodity div.dangerous_goods,
                    div.bol_doc div.commodity div.weight
                    {
                        padding: 0.3em;                
                    }

                    div.bol_doc div.commodity div.identification,
                    div.bol_doc div.commodity div.weight
                    {
                        border-bottom: 1px #000000 solid;
                    }

                    div.bol_doc div.commodity div.identification span.heading,
                    div.bol_doc div.commodity div.weight span.heading
                    {
                        display: inline;
                    }
            
                    div.bol_doc div.commodity div.dangerous_goods p
                    {
                        font-weight: bold;
                        margin: 0.25em 0 0.8em 0;
                    }
            
                    div.bol_doc div.commodity table.dimensions
                    {
                        width: 100%;
                    }
            
                    div.bol_doc div.commodity table.dimensions td
                    {
                        padding-bottom: 0.25em;
                    }
            
                    div.bol_doc div.commodity table.dimensions td.heading
                    {
                        width: 13%;
                    }
            
                    div.bol_doc div.commodity table.dimensions span.name,
                    div.bol_doc div.commodity table.dimensions span.length,
                    div.bol_doc div.commodity table.dimensions span.width,
                    div.bol_doc div.commodity table.dimensions span.height
                    {
                        font-weight: bold;
                    }
            
                    div.bol_doc div.commodity td.charges
                    {
                        padding: 0.3em;
                        border-left: 1px #000000 solid;
                    }
            
                    div.bol_doc div.commodity table.payment_types
                    {
                        width: 100%;
                        margin-top: 0.25em;
                    }
            
                    div.bol_doc div.commodity table.payment_types span.checkbox span.ballet
                    {
                        font-size: 1.2em;
                    }
            
                    div.bol_doc div.commodity span.disclaimer
                    {
                        display: block;
                        font-size: 0.8em;
                        margin: 0.25em 0;
                    }
                       
                    div.bol_doc div.commodity table.amounts
                    {
                        width: 100%;
                        border-collapse: collapse;
                    }
            
                    div.bol_doc div.commodity table.amounts th
                    {
                        text-decoration: underline;
                        width: 25%;
                        border-right: 1px #000000 dotted;
                        border-top: 1px #000000 dotted;
                        padding: 0.3em;
                        font-weight: normal;
                    }
            
                    div.bol_doc div.commodity table.amounts td
                    {
                        border-top: 1px #000000 dotted;
                
                    }
            
                    div.bol_doc table.additional_info
                    {
                        width: 100%;
                        border-collapse: collapse;
                        border-left: 1px #000000 solid;
                        border-right: 1px #000000 solid;
                        border-bottom: 1px #000000 solid;
                    }
            
                    div.bol_doc table.additional_info td
                    {                
                        vertical-align: top;
                    }

                    div.bol_doc table.additional_info td div.content
                    {
                        padding: 0.3em;
                        min-height: 1.5em;
                    }

                    div.bol_doc td.dangerous_goods, 
                    div.bol_doc td.shippers_weight, 
                    div.bol_doc td.notice_of_claim, 
                    div.bol_doc td.loading_declaration
                    {
                        border-right: 1px #000000 solid;
                    }
            
                    div.bol_doc td.dangerous_goods, 
                    div.bol_doc td.comments_and_initials, 
                    div.bol_doc td.shippers_weight, 
                    div.bol_doc td.shipper_per, 
                    div.bol_doc td.notice_of_claim,
                    div.bol_doc td.declared_value, 
                    div.bol_doc td.shipper_originating
                    {
                        width: 50%;
                        border-bottom: 1px #000000 solid;
                    }

                    div.bol_doc td.comments_and_initials p
                    {
                        margin-top: 0.3em;
                    }
                    
                    div.bol_doc td.comments_and_initials div.initials
                    {
                        float: right;
                        padding: 1em 0;
                        width: 15%;
                        margin-bottom: 0.1em;
                        border: 1px #000000 solid;
                    }
                   
            
                    div.bol_doc td.shippers_weight p,
                    div.bol_doc td.notice_of_claim p,
                    div.bol_doc td.declared_value span.subtext
                    {
                        font-size: 0.7em;
                    }
            
                    div.bol_doc td.declared_value span.value
                    {
                        float: right;
                        font-weight: bold;
                        font-size: 1.1em;
                    }
            
                    div.bol_doc td.declared_value span.subtext
                    {
                        display: block;
                        width: 70%;
                    }
            
                    div.bol_doc span.signline
                    {
                        display: block;
                        border-bottom: 1px dotted #000000;
                        font-weight: bold;
                        padding: 0.3em 0;
                        margin: 0.3em;
                    }
            
                    div.bol_doc div.declared_value
                    {
                
                    }
            
                    div.bol_doc span.signline span.subtext
                    {
                        font-size: 0.7em;
                        font-weight: normal;
                        float: right;
                        color: #676767;
                    }

                    div.bol_doc table.declaration
                    {
                        width: 100%;
                        margin-bottom: 0.5em;
                    }            
            
                    div.bol_doc table.declaration th.date, 
                    div.bol_doc table.declaration th.time 
                    {
                        width: 28%;
                        font-weight: normal;
                    }           
            
                    div.bol_doc table.declaration td span
                    {
                        display: block;
                        padding: 0.3em;
                    }
            
                    div.bol_doc table.declaration td.title
                    {
                        border-bottom: 1px #000000 dotted;
                    }
            
            
                    div.bol_doc table.declaration td.date
                    {
                        
                    }
            
                    div.bol_doc table.declaration td.fill
                    {                
                        border-left: 1px #000000 dotted; 
                        border-bottom: 1px #000000 dotted;
                    }
            
                    div.bol_doc table.declaration td.total
                    {
                        text-align: right;
                    }
            
                    div.bol_doc table.signatures
                    {
                        width: 100%;
                        border: 1px #000000 solid;
                    }
            

                    div.page_break
                    {
                        display: block;
                        margin: 35px;
                        height: 1px;
                        border-top: 1px #454545 solid;
                    }
            

                    div.page_break
                    {
                        display: block;
                        margin: 35px;
                        height: 1px;
                        border-top: 1px #454545 solid;
                    }
            ";

            return styles;
        }

        public static string GetDocSpecificPrintStyles()
        {
            var styles = @"                    
            ";

            return styles;
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

            var replacements = new object[6];
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
            replacements[5] = documentID;            

            return string.Format(html, replacements);
        }

        private string GetReferenceTable(Dispatch dispatch, LoadedCommodity commodity)
        {
            var html = @"
                <div class=""reference"">
                    <table>
                        <tr>
                            <td class=""recipient"">
                                <div>
                                    <span class=""date"">Date: <span>{0}</span></span>

                                    <span class=""customer"">Customer: <span>{1}</span></span>

                                    <span class=""equipment""><span class=""unit"">Unit #: <span>{2}</span></span>&nbsp;&nbsp;&nbsp;&nbsp;<span class=""trailer"">Trailer #: <span>{3}</span></span></span>

                                    <div class=""shipper"">
                                        <span class=""heading"">Shipper or Agent (Name Address)</span>

                                        <span>{4}</span>
                                        <span>{5}</span>
                                        <span>{6}</span>
                                        <span>{7}</span>
                                        <span>{8}</span>
                                    </div>

                                    <div class=""consignee"">
                                        <span class=""heading"">Consignee (Name Address)</span>

                                        <span>{9}</span>
                                        <span>{10}</span>
                                        <span>{11}</span>
                                        <span>{12}</span>
                                        <span>{13}</span>
                                    </div>
                                </div>
                            </td>
                            <td class=""legal"">
                                <div>
                                    {14}                                    
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
            ";
                        
            var replacements = new object[15];

            replacements[0] = DateTime.Now.ToString(SingerConfigs.PrintedDateFormatString);
            replacements[1] = commodity.JobCommodity.Job.Company.Name;
            replacements[2] = (dispatch != null && dispatch.Load != null && dispatch.Load.Equipment != null) ? dispatch.Load.Equipment.UnitNumber : "";
            replacements[3] = (dispatch != null && dispatch.Load != null && dispatch.Load.Rate != null) ? dispatch.Load.Rate.Name + " - " : "";

            if (dispatch != null && dispatch.Load != null && dispatch.Load.TrailerCombination != null)
                replacements[3] += dispatch.Load.TrailerCombination.Combination;

            replacements[4] = (commodity.ShipperCompany != null) ? commodity.ShipperCompany.Name : "";
            
            if (commodity.ShipperAddress != null)
            {
                replacements[5] = commodity.ShipperAddress.Line1;
                replacements[6] = commodity.ShipperAddress.City;

                if (commodity.ShipperAddress.ProvincesAndState != null)
                {
                    replacements[6] += ", " + commodity.ShipperAddress.ProvincesAndState.Name;
                    replacements[7] = commodity.ShipperAddress.ProvincesAndState.Country.Name;
                }

                replacements[8] = commodity.ShipperAddress.PostalZip;
            }
            
            replacements[9] = (commodity.ConsigneeCompany != null) ? commodity.ConsigneeCompany.Name : "";
            
            if (commodity.ConsigneeAddress != null)
            {
                replacements[10] = commodity.ConsigneeAddress.Line1;
                replacements[11] = commodity.ConsigneeAddress.City;

                if (commodity.ConsigneeAddress.ProvincesAndState != null)
                {
                    replacements[11] += ", " + commodity.ConsigneeAddress.ProvincesAndState.Name;
                    replacements[12] = commodity.ConsigneeAddress.ProvincesAndState.Country.Name;
                }

                replacements[13] = commodity.ConsigneeAddress.PostalZip;
            }

            replacements[14] =  SingerConfigs.GetConfig("BillOfLading-MainLegal") ?? "";

            return string.Format(html, replacements);
        }

        private string GetGlassLiability()
        {
            return @"<div class=""glass_damage"">Carrier not liable for glass damage in transit</div>";
        }

        private string GetCommodityDetails(LoadedCommodity commodity)
        {
            var html = @"
                <div class=""commodity"">
                    <table class=""commodity_details"">
                        <tr>
                            <td class=""details"">
                                <div class=""identification"">
                                    <table class=""dimensions"">
                                        <tr>
                                            <td class=""heading""><span class=""heading"">Commodity:</span></td>
                                            <td colspan=""3""><span class=""name"">{0}</span></td>                                    
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td><span class=""field"">Length:</span> <span class=""length"">{1}</span></td>
                                            <td><span class=""field"">Width:</span> <span class=""width"">{2}</span></td>
                                            <td><span class=""field"">Height:</span> <span class=""height"">{3}</span></td>
                                        </tr>
                                    </table>
                                </div>

                                <div class=""weight"">
                                    <span class=""heading"">Net Weight of Shipment</span>

                                    <span class=""weight"">{4}</span>
                                </div>
                                
                                <div class=""dangerous_goods"">
                                    <span class=""heading"">Dangerous Goods Information</span>
                                    
                                    <p>{8}</p>
                                </div>
                            </td>
                            <td class=""charges"">
                                <span class=""heading"">Freight Charges</span>

                                <table class=""payment_types"">
                                    <tr>
                                        <td><span class=""checkbox""><span class=""ballet"">{5}</span> COD</span></td>
                                        <td><span class=""checkbox""><span class=""ballet"">{6}</span> Collect</span></td>
                                        <td><span class=""checkbox""><span class=""ballet"">{7}</span> PrePaid</span></td>
                                    </tr>
                                </table>

                                <span class=""disclaimer"">Freight charges will be collected unless marked prepaid.</span>

                                <table class=""amounts"">
                                    <tr>
                                        <th>Amount</th>
                                        <td><span></span></td>
                                    </tr>
                                    <tr>
                                        <th>GST</th>
                                        <td><span></span></td>
                                    </tr>
                                    <tr>
                                        <th>Total</th>
                                        <td><span></span></td>
                                    </tr>
                                </table>
                            </td>
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

            var replacements = new object[9];

            replacements[0] = commodity.JobCommodity.NameAndUnit;

            replacements[1] = MeasurementFormater.FromMetres(commodity.JobCommodity.Length, lengthUnit);
            replacements[2] = MeasurementFormater.FromMetres(commodity.JobCommodity.Width, lengthUnit);
            replacements[3] = MeasurementFormater.FromMetres(commodity.JobCommodity.Height, lengthUnit);
            replacements[4] = MeasurementFormater.FromKilograms(commodity.JobCommodity.Weight, weightUnit);

            if (commodity.Load.Job.Company.CompanyPriorityLevel != null && commodity.Load.Job.Company.CompanyPriorityLevel.Name.EndsWith("Cash on Delivery"))
                replacements[5] = "&#9745;";
            else
                replacements[5] = "&#9744;";

            replacements[6] = "&#9744;";
            replacements[7] = "&#9744;";
            replacements[8] = commodity.BoLDangerousGoodsInfo;


            return string.Format(html, replacements);
        }

        private string GetAdditionalInfo(LoadedCommodity commodity)
        {
            var html = @"
                <table class=""additional_info"">
                    <tr>
                        <td colspan=""2"" class=""comments_and_initials"">
                            <div class=""comments_and_initials content"">
                                <span class=""heading"">Comments and Initials</span>

                                <div class=""initials""><span></span></div>

                                <p>{4}</p>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class=""shippers_weight"">
                            <div class=""shippers_weight content"">
                                <span class=""heading"">Shipper's Weight</span>                    
                                
                                {0}
                            </div>
                        </td>
                        <td class=""shipper_per"">
                            <div class=""shipper_per content"">
                                <span class=""signline"">Shipper: <span class=""subtext"">To be signed by shipper</span></span>
                                <span class=""signline"">Per:</span>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class=""notice_of_claim"" rowspan=""2"">
                            <div class=""notice_of_claim content"">
                                <span class=""heading"">Notice of Claim</span>

                                {1}
                            </div>
                        </td>
                        <td class=""declared_value"">
                            <div class=""declared_value content"">                    
                                <span class=""heading"">Declared Valuation</span>

                                <span class=""value"">{2}</span>

                                <span class=""subtext"">{3}</span>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class=""shipper_originating"">
                            <div class=""shipper_originating content"">
                                <span class=""signline"">Shipper: <span class=""subtext"">To be signed by shipper</span></span>
                                <span class=""signline"">Originating Carrier:</span>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class=""loading_declaration"">
                            <div class=""loading_declaration content"">
                                <span class=""heading"">Loading Declaration</span>

                                <table class=""declaration"">
                                    <tr>                            
                                        <th class=""other""><span></span></th>
                                        <th class=""date"">Date</th>
                                        <th class=""time"">Time</th>                            
                                    </tr>
                                    <tr>
                                        <td class=""title""><span>Departing for Load Location</span></td>
                                        <td class=""fill""><span>&nbsp;</span></td>
                                        <td class=""fill""><span>&nbsp;</span></td>                            
                                    </tr>
                                    <tr>
                                        <td class=""title""><span>Arrived Load Loaction</span></td>
                                        <td class=""fill""><span>&nbsp;</span></td>
                                        <td class=""fill""><span>&nbsp;</span></td>                            
                                    </tr>
                                    <tr>
                                        <td class=""title""><span>Departed Load Location</span></td>
                                        <td class=""fill""><span>&nbsp;</span></td>
                                        <td class=""fill""><span>&nbsp;</span></td>                            
                                    </tr>
                                    <tr>
                                        <td class=""total""><span></span></td>
                                        <td class=""total""><span>Total Hours</span></td>
                                        <td class=""fill""><span>&nbsp;</span></td>                            
                                    </tr>
                                </table>

                                <span class=""signatures"">Certification (Shipper Sign Print):</span>
                            </div>
                        </td>
                        <td class=""unloading_declaration"">
                            <div class=""unloading_declaration content"">
                                <span class=""heading"">Unloading Declaration</span>

                                <table class=""declaration"">
                                    <tr>                            
                                        <th class=""other""></th>
                                        <th class=""date"">Date</th>
                                        <th class=""time"">Time</th>                            
                                    </tr>
                                    <tr>
                                        <td class=""title""><span>Arrived at Unload Location</span></td>
                                        <td class=""fill""><span>&nbsp;</span></td>
                                        <td class=""fill""><span>&nbsp;</span></td>                            
                                    </tr>
                                    <tr>
                                        <td class=""title""><span>Departed Unload Loaction</span></td>
                                        <td class=""fill""><span>&nbsp;</span></td>
                                        <td class=""fill""><span>&nbsp;</span></td>                            
                                    </tr>
                                    <tr>
                                        <td class=""title""><span>Arrived at Terminal</span></td>
                                        <td class=""fill""><span>&nbsp;</span></td>
                                        <td class=""fill""><span>&nbsp;</span></td>                            
                                    </tr>
                                    <tr>
                                        <td class=""total""><span></span></td>
                                        <td class=""total""><span>Total Hours</span></td>
                                        <td class=""fill""><span>&nbsp;</span></td>                            
                                    </tr>
                                </table>    

                                <span class=""signatures"">Certification (Consignee Sign Print):</span>
                            </div>
                        </td>
                    </tr>
                </table>
            ";
            
            var replacements = new object[5];

            replacements[0] = SingerConfigs.GetConfig("BillOfLading-ShippersWeight") ?? "";
            replacements[1] = SingerConfigs.GetConfig("BillOfLading-NoticeOfClaim") ?? "";
            replacements[2] = string.Format("{0:C}", commodity.JobCommodity.Value);
            replacements[3] = SingerConfigs.GetConfig("BillOfLading-MaxLiability") ?? "";
            replacements[4] = commodity.BoLComments;

            return string.Format(html, replacements);
        }

        private string GetSignatures()
        {
            var html = @"
                <table class=""signatures"">
                    <tr>
                        <td>
                            <div>
                                <span class=""signline"">Carrier:</span>
                                <span class=""signline"">Per:</span>
                            </div>
                        </td>
                        <td> 
                            <div>
                                <span class=""signline"">Shipper:</span>
                                <span class=""signline"">Per:</span>
                            </div>
                        </td>
                        <td>
                            <div>
                                <span class=""signline"">Consignee:</span>
                                <span class=""signline"">Per:</span>
                            </div>
                        </td>
                    </tr>
                </table>
            ";

            return html;
        }
    }
}
