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
            if (entity is StorageItem)
                return GenerateHTML((StorageItem)entity);
            else if (entity is Job)
                return GenerateHTML((Job)entity);
            else
                throw new Exception("Unknown enity type");
        }

        public string GenerateHTML(Job job)
        {
            var content = new StringBuilder();

            content.Append(GenerateHeaderHTML());
            content.Append(GenerateBodyHTML(job));
            content.Append(GenerateFooterHTML());

            return content.ToString();
        }

        public string GenerateHTML(StorageItem item)
        {
            var content = new StringBuilder();

            content.Append(GenerateHeaderHTML());
            content.Append(GenerateBodyHTML(item));
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

        public string GenerateBodyHTML(Job job)
        {
            var content = new StringBuilder();
            var documentNumber = string.Format("SC-{0}", job.Number);

            Address address;
            Contact contact;
            var company = job.CareOfCompany ?? job.Company;

            try
            {   
                address = (from a in company.Addresses where a.AddressType.Name == "Head Office" select a).First();
            }
            catch (Exception e)
            {
                throw new Exception("Company needs a head office address before a storage contract can be printed from here.");            
            }

            try
            {
                contact = job.StoredItems.First().Contact;
            }
            catch
            {
                contact = null;
            }

            content.Append(@"<div class=""storage_contract"">");
            content.Append(GetHeader(documentNumber));
            content.Append(GetReferenceTable(job.Company, job.CareOfCompany, contact, address));
            content.Append(GetCommodities(job));
            content.Append(GetLegal());
            content.Append(GetSignatures());
            content.Append("</div>");

            return content.ToString();
        }

        public string GenerateBodyHTML(StorageItem item)
        {
            if (item.JobCommodity == null)
                throw new Exception("There needs to a a commodity associated with this storage item before a contract may be created");

            var content = new StringBuilder();
            var documentNumber = string.Format("SC-{0}-{1}", item.Job.Number, item.Number);

            var company = item.Job.CareOfCompany ?? item.Job.Company;
            var contact = item.Contact;

            Address address;

            try
            {
                address = (from a in company.Addresses where a.AddressType.Name == "Head Office" select a).First();
            }
            catch (Exception e)
            {
                throw new Exception("Company needs a head office address before a storage contract can be printed from here.");
            }

            content.Append(@"<div class=""storage_contract"">");
            content.Append(GetHeader(documentNumber));
            content.Append(GetReferenceTable(item.Job.Company, item.Job.CareOfCompany, contact, address));
            content.Append(GetCommodity(item));
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
                        font-size: 12px;
                        font-family: sans-serif;
                        padding: 10px;
                    }

                     %BOL_SCREEN%
                </style>
                <style type=""text/css"" media=""print"">
                    body
                    {
                        font-size: 10px;
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
                    border-top: 1px #000000 solid;
                    border-bottom: 1px #000000 solid;
                }
            
                div.storage_contract div.reference
                {
                    border-left: 1px #000000 solid;
                    border-right: 1px #000000 solid;                    
                }             
                
                div.storage_contract div.reference div.customer span.date,
                div.storage_contract div.reference div.customer span.customer,
                div.storage_contract div.reference div.customer span.contact
                {
                    display: block;
                    border-bottom: 1px #000000 solid;
                    padding: 3px 7px;
                    font-weight: bold;
                }
                
                div.storage_contract div.reference div.customer span.date span,
                div.storage_contract div.reference div.customer span.customer span,
                div.storage_contract div.reference div.customer span.contact span
                {
                    font-weight: normal;
                }
            
                div.storage_contract div.reference div.customer span.contact span.phone
                {
                    padding-left: 10px;
                }
            
                div.storage_contract div.reference table
                {
                    width: 100%;
                    border-collapse: collapse;
                }
                                
                div.storage_contract div.reference td
                {
                    width: 50%;
                    padding: 3px 7px 5px 7px;
                }
                
                div.storage_contract div.reference td span.heading
                {
                    margin-bottom: 3px;
                }
                   
                div.storage_contract div.reference td span
                {
                    display: block;
                }
                    
                div.storage_contract div.reference td.secondary
                {   
                    border-left: 1px #000000 solid;
                }
                
                div.storage_contract div.commodities
                {
                    border: 1px #000000 solid;
                    padding: 5px 7px;
                    min-height: 125px;
                }
                
                div.storage_contract div.commodities span.between
                {
                    display: block;
                    margin-bottom: 3px;    
                }
                
                div.storage_contract div.commodities table
                {
                    margin-top: 7px;
                    width: 100%;
                    border-collapse: collapse;
                }
                
                div.storage_contract div.commodities th
                {
                }
                
                div.storage_contract div.commodities th.name,
                div.storage_contract div.commodities th.owner
                {                    
                    text-align: left;
                }
                
                div.storage_contract div.commodities td.name,
                div.storage_contract div.commodities td.owner
                {                    
                    text-align: left;
                    padding-top: 5px;
                }
                
                div.storage_contract div.commodities td.dimensions,
                div.storage_contract div.commodities td.weight,
                div.storage_contract div.commodities td.price
                {
                    padding-top: 5px;
                    text-align: center;
                }
                
                div.storage_contract div.commodities tr.with_note td
                {
                    
                }
                
                div.storage_contract div.commodities tr.note td
                {
                    padding-top: 5px;
                    font-style: italic;
                }
                
                div.storage_contract div.commodities tr.note td,
                div.storage_contract div.commodities tr.no_note td 
                {
                    border-bottom: 1px #000000 dashed;
                    padding-bottom: 5px;
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
                    clear: both;
                    display: block;
                    border-bottom: 1px #000000 dotted;
                }
                
                div.storage_contract div.signatures span.subtext
                {
                    display: block;
                    font-size: 0.8em;
                    padding: 0 10px;
                }
                
                div.storage_contract div.signatures span.subtext span
                {
                    float: left;
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

        public string GetReferenceTable(Company company, Company careOfCompany, Contact contact, Address address)
        {
            var html = @"
                <div class=""reference"">
                    <div class=""customer"">
                        <span class=""date"">Date: <span>{0}</span></span>

                        <span class=""customer"">Customer: <span>{1}</span></span>

                        <span class=""contact"">Contact: <span class=""name"">{2}</span><span class=""phone"">{3}</span><span class=""phone"">{4}</span></span>                        
                    </div>

                    <table>
                        <tr>
                            <td class=""primary"">
                                <div class=""owner"">
                                    <span class=""heading"">Owner/Agent (Name Address)</span>

                                    <span>{5}</span>
                                    <span>{6}</span>
                                    <span>{7}</span>
                                    <span>{8}</span>
                                    <span>{9}</span>
                                </div>
                            </td>
                            <td class=""secondary"">                                
                                <div class=""store"">
                                    <span class=""heading"">Singer Storage (Name Address)</span>
                                    
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

            var customer = string.Format("{0}", company);
            if (careOfCompany != null)
                customer = string.Format("{0} c/o {1}", customer, careOfCompany);

            replacements[0] = DateTime.Now.ToString(SingerConfigs.PrintedDateFormatString);
            replacements[1] = customer;

            replacements[2] = (contact != null) ? contact.Name : "";
            replacements[3] = (contact != null && !string.IsNullOrEmpty(contact.PrimaryPhone)) ? string.Format("[ Ph: {0} ]", contact.PrimaryPhone) : "";
            replacements[4] = (contact != null && !string.IsNullOrEmpty(contact.Fax)) ? string.Format("[ Fax: {0} ]", contact.Fax) : "";

            var city = (address != null) ? address.City : "";
            var prov = (address != null && address.ProvincesAndState != null) ? address.ProvincesAndState.Abbreviation : "";
            var postal = (address != null) ? address.PostalZip : "";

            replacements[5] = customer;
            replacements[6] = (address != null) ? address.Line1 : "";
            replacements[7] = (address != null) ? address.Line2 : "";
            replacements[8] = (address != null) ? string.Format("{0}, {1} {2}", city, prov, postal) : "";
            replacements[9] = (address != null) ? address.PrimaryPhone : "";

            replacements[10] = SingerConfigs.GetConfig("SingerName") ?? "Singer Specialized Ltd.";
            replacements[11] = SingerConfigs.GetConfig("SingerAddress-StreetAddress");
            replacements[12] = SingerConfigs.GetConfig("SingerAddress-City");
            replacements[13] = SingerConfigs.GetConfig("SingerAddress-Phone");

            return string.Format(html, replacements);
        }

        public string GetCommodities(Job job)
        {
            var html = GetCommodityListHTML();
            var replacements = new string[3];
            var rows = new StringBuilder();

            var company = job.CareOfCompany ?? job.Company;

            foreach (var item in job.StoredItems)
            {
                var commodity = item.JobCommodity;

                if (commodity != null)
                    rows.Append(GetCommodityRow(commodity.NameAndUnit, commodity.Owner.Name, commodity.Length, commodity.Width, commodity.Height, commodity.Weight, item.BillingRate, item.BillingInterval, item.Notes));
            }

            replacements[0] = SingerConfigs.GetConfig("SingerName") ?? "Singer Specialized Ltd.";
            replacements[1] = company.Name;
            replacements[2] = rows.ToString();

            return string.Format(html, replacements);
        }

        public string GetCommodity(StorageItem item)
        {
            var html = GetCommodityListHTML();
            var replacements = new string[3];
            var rows = new StringBuilder();

            var commodity = item.JobCommodity;
            var company = item.Job.CareOfCompany ?? item.Job.Company;

            if (commodity != null)
                rows.Append(GetCommodityRow(commodity.NameAndUnit, commodity.Owner.Name, commodity.Length, commodity.Width, commodity.Height, commodity.Weight, item.BillingRate, item.BillingInterval, item.Notes));            

            replacements[0] = SingerConfigs.GetConfig("SingerName") ?? "Singer Specialized Ltd.";
            replacements[1] = company.Name;
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
                                <th class=""owner"">Owner</th>
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

        private string GetCommodityRow(string name, string owner, double? length, double? width, double? height, double? weight, decimal? price, BillingInterval interval, string notes)
        {
            var noNoteRow = @"
                <tr class=""no_note"">
                    <td class=""name"">{0}</td>
                    <td class=""owner"">{1}</td>
                    <td class=""dimensions"">{2}</td>
                    <td class=""weight"">{3}</td>
                    <td class=""price"">{4}</td>
                </tr>
            ";
            var withNoteRow = @"
                <tr class=""with_note"">
                    <td class=""name"">{0}</td>
                    <td class=""owner"">{1}</td>
                    <td class=""dimensions"">{2}</td>
                    <td class=""weight"">{3}</td>
                    <td class=""price"">{4}</td>
                </tr>
                <tr class=""note"">
                    <td class=""note"" colspan=""5"">
                        {5}
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

            var size = (string.IsNullOrEmpty(notes)) ? 5 : 6;
            var rowReps = new string[size];

            rowReps[0] = name;
            rowReps[1] = owner;
            rowReps[2] = string.Format("{0} x {1} x {2} (LxWxH)", MeasurementFormater.FromMetres(length, lengthUnit), MeasurementFormater.FromMetres(width, lengthUnit), MeasurementFormater.FromMetres(height, lengthUnit));
            rowReps[3] = string.Format("{0}", MeasurementFormater.FromKilograms(weight, weightUnit));

            if (interval == null)
                rowReps[4] = string.Format("{0:C}&nbsp;", price);
            else
                rowReps[4] = string.Format("{0:C} ({1})", price, interval.Name);

            if (!string.IsNullOrEmpty(notes))
            {
                rowReps[5] = notes;

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
                                <br><br>

                                <span class=""signline"">x</span>
                                <span class=""subtext""><span>Certification - (Singer Print)</span> <span class=""date"">Date</span></span>

                                <br><br><br>

                                <span class=""signline"">x</span>
                                <span class=""subtext""><span>Certification - (Singer Sign)</span> <span class=""date"">Date</span></span>
                            </td>
                            <td class=""secondary"">
                                <br><br>

                                <span class=""signline"">x</span>
                                <span class=""subtext""><span>Certification - (Owner/Agent Print)</span> <span class=""date"">Date</span></span>

                                <br><br><br>

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






