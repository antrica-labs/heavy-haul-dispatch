using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SingerDispatch.Utils;

namespace SingerDispatch.Printing.Documents
{
    class StorageContractDocument : SingerPrintDocument
    {
        public StorageContractDocument()
        {
            PrintMetric = true;
            SpecializedDocument = true;
        }

        public override string GenerateHTML(object entity)
        {
            if (entity is Quote)
                return GenerateHTML((Quote)entity);
            else if (entity is StorageItem)
                return GenerateHTML((StorageItem)entity);
            else
                throw new Exception("Unknown enity type");
        }

        public string GenerateHTML(StorageItem item)
        {
            var content = new StringBuilder();

            content.Append(GenerateHeaderHTML());
            content.Append(GenerateBodyHTML(item));
            content.Append(GenerateFooterHTML());

            return content.ToString();
        }
       
        public string GenerateHTML(Quote entity)
        {
            var content = new StringBuilder();

            content.Append(GenerateHeaderHTML());            
            content.Append(GenerateBodyHTML(entity));
            content.Append(GenerateFooterHTML());

            return content.ToString();
        }

        private string GenerateHeaderHTML()
        {
            var content = new StringBuilder();

            content.Append(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd""><html>");
            content.Append("<head>");
            content.Append(@"<meta http-equiv=""Content-Type"" content=""text/html;charset=utf-8"">");
            content.Append(GetTitle("Storage Contract"));
            content.Append(GetStyles());
            content.Append("</head>");
            content.Append("<body>");

            return content.ToString();
        }

        private string GenerateFooterHTML()
        {
            var content = new StringBuilder();

            content.Append("</body>");
            content.Append("</html>");

            return content.ToString();
        }

        public string GenerateBodyHTML(StorageItem item)
        {
            var content = new StringBuilder();
            var documentNumber = string.Format("SC-{0}", item.ID);

            Address address;

            try
            {
                address = (from a in item.Company.Addresses where a.AddressType.Name == "Head Office" select a).First();
            }
            catch (Exception e)
            {
                throw new Exception("Company needs a head office address before a storage contract can be printed from here.");
            }

            content.Append(@"<div class=""storage_contract"">");
            content.Append(GetHeader(documentNumber));
            content.Append(GetReferenceTable(item.Contact, address));
            content.Append(GetCommodity(item));
            content.Append(GetLegal());
            content.Append(GetSignatures());
            content.Append("</div>");

            return content.ToString();
        }

        public string GenerateBodyHTML(Quote quote)
        {
            var content = new StringBuilder();

            var documentNumber = string.Format("QS-{0}-{1}", quote.Number, quote.Revision);

            content.Append(@"<div class=""storage_contract"">");
            content.Append(GetHeader(documentNumber));
            content.Append(GetReferenceTable(quote.Contact, quote.BillingAddress));
            content.Append(GetCommodities(quote));
            content.Append(GetLegal());
            content.Append(GetSignatures());
            content.Append("</div>");

            return content.ToString();
        }

        private static string GetTitle(string title)
        {
            return "<title>" + title + "</title>";        
        }

        private static string GetStyles()
        {
            var styles = @"
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
                        font-size: 10pt;
                        font-family: Verdana, Arial, Helvetica, sans-serif;
                        padding: 10px;
                    }

                     %BOL_SCREEN%
                </style>
                <style type=""text/css"" media=""print"">
                    body
                    {
                    	font-size: 10pt;
                        padding: 0;
                    }

                    %BOL_PRINT%
                </style>
            ";

            return styles.Replace("%BOL_SCREEN%", GetDocSpecificScreenStyles()).Replace("%BOL_PRINT%", GetDocSpecificPrintStyles()); ;
        }

        public static string GetDocSpecificScreenStyles()
        {
            var styles = @"
                div.storage_contract span.heading
                {
                    display: block;
                    font-weight: bold;
                    text-decoration: underline;
                }  
            
                div.storage_contract p
                {
                    margin-bottom: 5px;
                }
            
                div.storage_contract div.header table 
                {
                    width: 100%;
                    border-collapse: collapse;
                }
                    
                div.storage_contract div.header td
                {                
                    vertical-align: top;
                    padding: 10px;
                }
                    
                div.storage_contract div.header td.logo_col
                {
                    width: 200px;
                        
                }
                    
                div.storage_contract div.header td.id_col
                {             
                    text-align: center;
                    font-weight: bold;
                    line-height: 1.35em;                  
                }
                    
                div.storage_contract div.header span
                {
                    display: block;
                }
                    
                div.storage_contract div.header span.title
                {
                    display: block;
                    font-weight: bold;
                    font-size: 1.5em;
                    padding: 0.5em 0.3em;
                    text-align: center;
                    border-top: 2px #000000 solid;
                    border-bottom: 1px #000000 solid;
                }
            
                div.storage_contract div.reference
                {
                    border-left: 1px #000000 solid;
                    border-right: 1px #000000 solid;                    
                }             
            
                div.storage_contract div.reference table
                {
                    width: 100%;
                    border-collapse: collapse;
                }
                    
                div.storage_contract div.reference td.secondary
                {
                    width: 50%;
                    border-left: 1px #000000 solid;
                }    
            
                div.storage_contract div.reference td div.store span, 
                div.storage_contract div.reference td div.owner span,
                div.storage_contract div.reference td div.contact span
                {
                    display: block;                
                }
            
                div.storage_contract div.reference td span.date, 
                div.storage_contract div.reference td span.customer                
                {
                    display: block;
                    font-weight: bold;
                    padding: 3px 7px;
                    border-bottom: 1px #000000 solid;
                }
            
                div.storage_contract div.reference td span.date span, 
                div.storage_contract div.reference td span.customer span
                {
                    font-weight: normal;
                }
                
                div.storage_contract div.reference td div.store,
                div.storage_contract div.reference td div.owner,
                div.storage_contract div.reference td div.contact
                {
                    padding: 3px 7px;
                }
                
                div.storage_contract div.reference td div.store 
                {   
                    border-top: 1px #000000 solid;
                }
                
                div.storage_contract div.commodities
                {
                    border: 1px #000000 solid;
                    padding: 5px 7px;
                    min-height: 125px;
                }
                
                div.storage_contract div.commodities table
                {
                    width: 100%;
                }
                
                div.storage_contract div.commodities th
                {
                }
                
                div.storage_contract div.commodities th.name
                {
                    text-align: left;
                }
                
                div.storage_contract div.commodities td.dimensions,
                div.storage_contract div.commodities td.weight,
                div.storage_contract div.commodities td.price
                {
                    text-align: center;
                }
                
                div.storage_contract div.commodities tr.with_note td
                {
                    padding: 5px;
                    padding-bottom: 0;
                }
                
                div.storage_contract div.commodities tr.note td,
                div.storage_contract div.commodities tr.no_note td 
                {
                    border-bottom: 1px #000000 dashed;
                    padding: 5px;
                }
                
                div.storage_contract div.legal
                {
                    border-left: 1px #000000 solid;
                    border-right: 1px #000000 solid;
                    padding: 5px 7px;
                }
                
                div.storage_contract div.signatures
                {
                    border: 1px #000000 solid;
                }
                
                div.storage_contract div.signatures table
                {
                    width: 100%;
                    border-collapse: collapse;
                }
                
                div.storage_contract div.signatures td                
                {
                    padding: 5px;
                }
                
                div.storage_contract div.signatures td.secondary
                {
                    border-left: 1px #000000 solid;
                }
                
                div.storage_contract div.signatures span.signline
                {
                    display: block;
                    padding-top: 25px;
                    border-bottom: 1px #000000 dotted;
                }
                
                div.storage_contract div.signatures span.subtext
                {
                    display: block;
                    font-size: 0.8em;
                    padding: 0 10px;
                }
                
                div.storage_contract div.signatures span.subtext span.date
                {
                    float: right;
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
                                <span class=""logo""><img src=""{0}"" alt=""Singer""></span>
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
                    
                    <span class=""title"">General Storage Contract</span>
                </div>
            ";

            var replacements = new object[7];

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

        public string GetReferenceTable(Contact contact, Address address)
        {
            var html = @"
                <div class=""reference"">
                    <table>
                        <tr>
                            <td>
                                <div>
                                    <span class=""date"">Date: <span>{0}</span></span>

                                    <span class=""customer"">Customer: <span>{1}</span></span>

                                    <div class=""contact"">
                                        <span class=""heading"">Customer Contact</span>

                                        <span>{2}</span>
                                        <span>{3}</span>
                                        <span>{4}</span>
                                    </div>
                                
                                </div>
                            </td>
                            <td class=""secondary"">
                                <div class=""owner"">
                                    <span class=""heading"">Owner/Agent (Name Address)</span>

                                    <span>{5}</span>
                                    <span>{6}</span>
                                    <span>{7}</span>
                                    <span>{8}</span>
                                </div>
                                <div class=""store"">
                                    <span class=""heading"">Singer Storage (Name Address)</span>

                                    <span>{9}</span>
                                    <span>{10}</span>
                                    <span>{11}</span>
                                    <span>{12}</span>
                                    <span>{13}</span>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
            ";

            var replacements = new string[14];

            replacements[0] = DateTime.Now.ToString(SingerConfigs.PrintedDateFormatString);
            replacements[1] = (contact != null) ? contact.Company.Name : "";
            replacements[2] = (contact != null) ? contact.Name : "";
            replacements[3] = (contact != null && !string.IsNullOrEmpty(contact.PrimaryPhone)) ? string.Format("Ph: {0}", contact.PrimaryPhone) : "";
            replacements[4] = (contact != null && !string.IsNullOrEmpty(contact.Fax)) ? string.Format("Fax: {0}", contact.Fax) : "";
            replacements[5] = SingerConfigs.GetConfig("SingerName") ?? "Singer Specialized Ltd.";
            replacements[6] = SingerConfigs.GetConfig("SingerAddress-StreetAddress");
            replacements[7] = SingerConfigs.GetConfig("SingerAddress-City");
            replacements[8] = SingerConfigs.GetConfig("SingerAddress-Phone");
            replacements[9] = (contact != null) ? contact.Company.Name : "";
            replacements[10] = (address != null) ? address.Line1 : "";
            replacements[11] = (address != null) ? address.Line2 : "";

            var city = (address != null) ? address.City : "";
            var prov = (address != null && address.ProvincesAndState != null) ? address.ProvincesAndState.Abbreviation : "";
            var postal = (address != null) ? address.PostalZip : "";

            replacements[12] = (address != null) ? string.Format("{0}, {1} {2}", city, prov, postal) : "";
            replacements[12] = (address != null) ? address.PrimaryPhone : "";

            return string.Format(html, replacements);
        }

        public string GetCommodities(Quote quote)
        {
            var html = GetCommodityListHTML();
            var replacements = new string[3];
            var rows = new StringBuilder();
                     
            foreach (var item in quote.QuoteStorageItems)
            {
                var commodity = item.Commodity;

                rows.Append(GetCommodityRow(commodity.NameAndUnit, commodity.Length, commodity.Width, commodity.Height, commodity.Weight, item.Price, item.BillingInterval, item.Notes));             
            }

            replacements[0] = SingerConfigs.GetConfig("SingerName") ?? "Singer Specialized Ltd.";
            replacements[1] = quote.Company.Name;
            replacements[2] = rows.ToString();

            return string.Format(html, replacements);           
        }

        public string GetCommodity(StorageItem item)
        {
            var html = GetCommodityListHTML();
            var replacements = new string[3];
            var rows = new StringBuilder();

            var commodity = item.Commodity;

            rows.Append(GetCommodityRow(commodity.NameAndUnit, commodity.Length, commodity.Width, commodity.Height, commodity.Weight, item.BillingRate, item.BillingInterval, item.Notes));            

            replacements[0] = SingerConfigs.GetConfig("SingerName") ?? "Singer Specialized Ltd.";
            replacements[1] = item.Company.Name;
            replacements[2] = rows.ToString();

            return string.Format(html, replacements);
        }

        private string GetCommodityListHTML()
        {
            var html = @"
                <div class=""commodities"">
                    <span class=""between"">Between <span class=""company1"">{0}</span> and <span class=""company2"">{1}</span></span>

                    <p>In consideration for providing storage, the undersigned agrees to pay the amount of listed below each, per month for the storage of the following described item(s):</p>            

                    <table>
                        <thead>
                            <tr>
                                <th class=""name"">Item Description</th>
                                <th class=""dimensions"">Dimensions (LxWxH)</th>
                                <th class=""weight"">Weight</th>
                                <th class=""price"">Rate</th>                    
                            </tr>
                        </thead>
                        <tbody>
                            {2}
                        </tbody>
                    </table>
                </div>
            ";

            return html;
        }

        private string GetCommodityRow(string name, double? length, double? width, double? height, double? weight, decimal? price, BillingInterval interval, string notes)
        {
            var noNoteRow = @"
                <tr class=""no_note"">
                    <td class=""name"">{0}</td>
                    <td class=""dimensions"">{1}</td>
                    <td class=""weight"">{2}</td>
                    <td class=""price"">{3}</td>
                </tr>
            ";
            var withNoteRow = @"
                <tr class=""with_note"">
                    <td class=""name"">{0}</td>
                    <td class=""dimensions"">{1}</td>
                    <td class=""weight"">{2}</td>
                    <td class=""price"">{3}</td>
                </tr>
                <tr class=""note"">
                    <td class=""note"" colspan=""4"">
                        {4}
                    </td>
                </tr>
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

            var size = (string.IsNullOrEmpty(notes)) ? 4 : 5;
            var rowReps = new string[size];

            rowReps[0] = name;
            rowReps[1] = string.Format("{0} x {1} x {2} (LxWxH)", MeasurementFormater.FromMetres(length, lengthUnit), MeasurementFormater.FromMetres(width, lengthUnit), MeasurementFormater.FromMetres(height, lengthUnit));
            rowReps[2] = string.Format("{0}", MeasurementFormater.FromKilograms(weight, weightUnit));

            if (interval == null)
                rowReps[3] = string.Format("{0:C}&nbsp;", price);
            else
                rowReps[3] = string.Format("{0:C} ({1})", price, interval.Name);

            if (!string.IsNullOrEmpty(notes))
            {
                rowReps[4] = notes;

                return string.Format(withNoteRow, rowReps);
            }
            else
            {
                return string.Format(noNoteRow, rowReps);
            }       
        }

        private static string GetLegal()
        {
            var html = @"
                <div class=""legal"">
                    <p>The undersigned, for the same consideration, understands that they are not responsible for any damage, vandalism, or other perils, which may or may not occur during the course of storage. Singer Specialized Ltd., is not responsible for maintaining any insurance coverage, and the undersigned hereby forever release, discharge, acquit, and forgive from any and all claims, actions, suits, demands, agreements, and each of the if more than one, liabilities, judgments, and proceedings both at law an in equity arising from the beginning of time to the end of the term of storage. This release shall be binging upon, and inure to the benefit of Singer Specialized Ltd., their successors, insurers, assigns, agents, and representatives.</p>
                </div>
            ";

            return html;
        }

        private static string GetSignatures()
        {
            var html = @"
                <div class=""signatures"">
                    <table>
                        <tr>
                            <td>
                                <span class=""signline"">x</span>
                                <span class=""subtext""><span>Certification - (Singer Print)</span> <span class=""date"">Date</span></span>

                                <span class=""signline"">x</span>
                                <span class=""subtext""><span>Certification - (Singer Sign)</span> <span class=""date"">Date</span></span>
                            </td>
                            <td class=""secondary"">
                                <span class=""signline"">x</span>
                                <span class=""subtext""><span>Certification - (Owner/Agent Print)</span> <span class=""date"">Date</span></span>

                                <span class=""signline"">x</span>
                                <span class=""subtext""><span>Certification - (Owner/Agent Sign)</span> <span class=""date"">Date</span></span>
                            </td>
                        </tr>
                    </table>
                </div>
            ";

            return html;
        }
    }

    public class StoredItem
    {
        public string Name { get; set; }
        public double? Length { get; set; }
        public double? Width { get; set; }
        public double? Height { get; set; }
        public double? Weight { get; set; }
        public decimal? Price { get; set; }
        public BillingInterval Interval { get; set; }
        public string Notes { get; set; }
    }
}






