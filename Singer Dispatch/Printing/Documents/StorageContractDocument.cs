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
            {
                return GenerateHTML((Quote)entity);
            }
            else
                throw new Exception("Unknown enity type");
        }

        public string GenerateHTML(Quote entity)
        {
            var content = new StringBuilder();

            content.Append(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd""><html>");
            content.Append("<head>");
            content.Append(@"<meta http-equiv=""Content-Type"" content=""text/html;charset=utf-8"">");
            content.Append(GetTitle("Storage Contract"));
            content.Append(GetStyles());
            content.Append("</head>");
            content.Append("<body>");

            // Fill document body...
            content.Append(GenerateBodyHTML(entity));

            content.Append("</body>");
            content.Append("</html>");

            return content.ToString();
        }

        public string GenerateBodyHTML(Quote entity)
        {
            var content = new StringBuilder();

            var documentNumber = string.Format("QS{0}-{1}", entity.Number, entity.Revision);

            content.Append(@"<div class=""storage_contract"">");
            content.Append(GetHeader(documentNumber));
            content.Append(GetReferenceTable(entity));
            content.Append(GetCommodities(entity));
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
                    
                    <span class=""title"">General Storage Contract</span>
                </div>
            ";

            var replacements = new object[7];            

            replacements[0] = GetHeaderImg();
            replacements[1] = SingerConfigs.GetConfig("SingerName") ?? "Singer Specialized";
            replacements[2] = SingerConfigs.GetConfig("SingerAddress-StreetAddress");
            replacements[3] = SingerConfigs.GetConfig("SingerAddress-City");
            replacements[4] = SingerConfigs.GetConfig("SingerAddress-Phone");
            replacements[5] = documentID;            

            return string.Format(html, replacements);
        }

        public string GetReferenceTable(Quote entity)
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
            replacements[1] = entity.Company.Name;
            replacements[2] = (entity.Contact != null) ? entity.Contact.Name : "";
            replacements[3] = (entity.Contact != null && !string.IsNullOrEmpty(entity.Contact.PrimaryPhone)) ? string.Format("Ph: {0}", entity.Contact.PrimaryPhone) : "";
            replacements[4] = (entity.Contact != null && !string.IsNullOrEmpty(entity.Contact.Fax)) ? string.Format("Fax: {0}", entity.Contact.Fax) : "";
            replacements[5] = SingerConfigs.GetConfig("SingerName") ?? "Singer Specialized Ltd.";
            replacements[6] = SingerConfigs.GetConfig("SingerAddress-StreetAddress");
            replacements[7] = SingerConfigs.GetConfig("SingerAddress-City");
            replacements[8] = SingerConfigs.GetConfig("SingerAddress-Phone");                        
            replacements[9] = entity.Company.Name;
            replacements[10] = (entity.BillingAddress != null) ? entity.BillingAddress.Line1 : "";
            replacements[11] = (entity.BillingAddress != null) ? entity.BillingAddress.Line2 : "";

            var city = (entity.BillingAddress != null) ? entity.BillingAddress.City : "";
            var prov = (entity.BillingAddress != null && entity.BillingAddress.ProvincesAndState != null) ? entity.BillingAddress.ProvincesAndState.Abbreviation : "";
            var postal = (entity.BillingAddress != null) ? entity.BillingAddress.PostalZip : "";

            replacements[12] = (entity.BillingAddress != null) ? string.Format("{0}, {1} {2}", city, prov, postal) : "";
            replacements[12] = (entity.BillingAddress != null) ? entity.BillingAddress.PrimaryPhone : "";

            return string.Format(html, replacements);
        }

        public string GetCommodities(Quote entity)
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
                                <th class=""price"">Price per Month</th>                    
                            </tr>
                        </thead>
                        <tbody>
                            {2}
                        </tbody>
                    </table>
                </div>
            ";
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
                        <p>Some note goes here. This is just a test note. Nothing really to see here.</p>
                    </td>
                </tr>
            ";
            var replacements = new string[3];
            var rows = new StringBuilder();

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

            foreach (var item in entity.QuoteStorageItems)
            {
                var size = (string.IsNullOrEmpty(item.Notes)) ? 4 : 5;
                var rowReps = new string[size];

                rowReps[0] = item.Commodity.NameAndUnit;
                rowReps[1] = string.Format("{0} x {1} x {2} (LxWxH)", MeasurementFormater.FromMetres(item.Commodity.Length, lengthUnit), MeasurementFormater.FromMetres(item.Commodity.Width, lengthUnit), MeasurementFormater.FromMetres(item.Commodity.Height, lengthUnit));
                rowReps[2] = string.Format("{0}", MeasurementFormater.FromKilograms(item.Commodity.Weight, weightUnit));
                rowReps[3] = string.Format("{0:C}", item.Price);

                if (!string.IsNullOrEmpty(item.Notes))
                {
                    replacements[4] = item.Notes;

                    rows.Append(string.Format(withNoteRow, rowReps));
                }
                else
                {
                    rows.Append(string.Format(noNoteRow, rowReps));
                }                
            }

            replacements[0] = SingerConfigs.GetConfig("SingerName") ?? "Singer Specialized Ltd.";
            replacements[1] = entity.Company.Name;
            replacements[2] = rows.ToString();

            return string.Format(html, replacements);           
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
}






