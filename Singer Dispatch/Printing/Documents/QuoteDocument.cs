using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using SingerDispatch.Utils;

namespace SingerDispatch.Printing.Documents
{
    class QuoteDocument : SingerPrintDocument
    {
        private const string PageBreak = @"<div class=""page_break""></div>";

        public QuoteDocument()
        {
            PrintMetric = true;
            SpecializedDocument = true;
        }

        public override string GenerateHTML(object entity)
        {
            return GenerateHTML((Quote)entity);
        }

        private string GenerateHTML(Quote quote)
        {
            var content = new StringBuilder();

            content.Append(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd""><html>");
            content.Append("<head>");
            content.Append(@"<meta http-equiv=""Content-Type"" content=""text/html;charset=utf-8"">");
            content.Append(GetTitle("Quote - " + quote.Company.Name + " - " + quote.NumberAndRev));
            content.Append(GetStyles());
            content.Append("</head>");
            content.Append("<body>");
            content.Append(GetQuoteBody(quote));
            content.Append("</body>");
            content.Append("</html>");

            return content.ToString();
        }


        private StringBuilder GetQuoteBody(Quote quote)
        {
            var content = new StringBuilder();

            content.Append(@"<div class=""quote_body"">");
            content.Append(GetHeader("#" + quote.NumberAndRev));
            content.Append(GetRecipient(quote));
            content.Append(GetDescription(quote.Contact, quote.Description, quote.CreationDate, quote.ExpirationDate));
            content.Append(GetCommodities(quote));

            if (quote.QuoteSupplements.Count > 0)
                content.Append(GetSuppluments(quote.QuoteSupplements.ToList()));

            content.Append(GetNotes(quote));
            content.Append(GetPrice(quote));
            content.Append(GetInclusions(quote));
            content.Append(GetConditions(quote));
            content.Append(GetSignoff(quote));
            content.Append("</div>");

            return content;
        }

        private static string GetTitle(string title)
        {
            return "<title>" + title + "</title>";
        }

        private static string GetStyles()
        {
            const string css = @"
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

                    /***** SECTION STYLES *****/
                    div#header 
                    {                                
                        color: #767676;
                    }
                                
                    div#header span 
                    {
                        display: block;
                    }
                    
                    div#header table
                    {
                        width: 100%;
                    }
                    
                    div#header table td
                    {
                        width: 33%;
                    }
                    
                    
                    div#header td#quote_name 
                    {                
                        text-align: center;                
                    }
                    
                    div#header td#quote_name span.title 
                    {                
                        font-size: 1.6em;
                        margin: 1em 0;
                    }
                    
                    div#header td#hq_location 
                    {                
                        text-align: right;
                    }
                    
                    
                    div#recipient 
                    {             
                        margin-top: 2.5em;
                    }
                    
                    div#recipient table
                    {
                        width: 100%;
                    }             
                     
                    div#recipient span
                    {
                        display: block;
                    }
                     
                    div#recipient span#quote_date
                    {
                        margin-bottom: 1em;
                    }
                    
                    div#recipient td#address 
                    {                
                        width: 50%;
                    }
                    
                    div#recipient td#contact
                    {
                        text-align: right;                
                        width: 50%;                
                    }
                    
                    div#description 
                    {
                        clear: both;
                        padding-top: 1.5em;
                        margin-bottom: 1em;
                    }
                    
                    div#description table
                    {
                    	margin-bottom: 0.5em;
                    }
                    
                    div#description td.fieldname 
                    {                        
                        font-weight: bold;
                        padding-right: 0.5em;                        
                    }
                    
                    div#commodities 
                    {
                        clear: both;
                        margin-bottom: 1.5em;
                    }

                    div#supplements 
                    {
                        margin-bottom: 1.5em;
                    }
                   
                    div#notes
                    {
                        margin-bottom: 1em;
                    }


                    /****** GENERAL STYLES *****/
                    span.heading
                    {
                        font-weight: bold;
                        display: block;
                    }

                    p
                    {
                        margin-bottom: 1em;
                    }

                    td
                    {
                        vertical-align: top;
                    }            

                    div.break 
                    {
                        clear: both;    
                    }
                    
                    table.itemized 
                    {
                        border-collapse: collapse;                        
                        width: 100%;
                        margin: 0 auto;
                        border-bottom: 1px #000000 solid;
                    }

                    table.itemized th 
                    {
                        text-align: left;
                        text-transform: uppercase;                        
                        padding: 0.2em;
                        padding-right: 0.5em;
                    }

                    table.itemized th span.sub 
                    {
                        text-transform: none;
                    }

                    table.itemized td 
                    {
                    	border-top: 1px #000000 solid;
                        padding: 0.4em;                
                        padding-right: 1.1em;
                    }
                    
                    table.itemized tr.details
                    {
                                               
                    }

                    table.itemized tr.notes
                    {
                    	font-style: italic;
                    }

                    table.itemized tr.notes td
                    {
                        padding-bottom: 0.5em;
                        border-top: none;
                    }

                    table.itemized tbody 
                    {
                        
                    }

                    table.itemized tr.alt 
                    {
                        background-color: #dedede;
                    }

                    p.fine_print
                    {
                        font-size: 0.8em;
                    }

                    div#price
                    {
                        margin: 1em 0;
                    }
                    
                    div#price span.heading
                    {
                        font-size: 1.05em;
                        display: inline;
                        text-transform: uppercase;
                        text-decoration: underline;
                        padding-right: 1em;
                    }                    

                    ol.conditions
                    {
                        list-style-type: decimal;
                        list-style-position: outside;
                        padding-left: 1em;
                        margin: 1em 1.25em;
                    }
                    
                    ol.conditions li 
                    {
                        padding: 0.2em 0;
                    }
                
                    div.page_break
                    {
                        display: block;
                        margin: 3em;
                        height: 1px;
                        border-top: 1px #454545 solid;
                    }
                </style>
                <style type=""text/css"" media=""print"">
                    body
                    {
                    	font-size: 9pt;
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

            return css;
        }

        private string GetHeader(string quoteName)
        {
            const string html = @"
                <div id=""header"">
                    <table>
                        <tr>
                            <td id=""logo"">
                                <span class=""logo""><img src=""{0}"" alt=""Singer""></span>                        
                            </td>
                            <td id=""quote_name"">
                                <span class=""title"">Quote {1}</span>
                            </td>
                            <td id=""hq_location"">
                                <span class=""name"">{2}</span>                                
                                <span class=""address"">{3}</span>
                                <span class=""phone"">{4}</span>
                                <span class=""fax"">Phone: {5}</span>                                
                            </td>
                        </tr>
                    </table>                        
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
            replacements[1] = quoteName;
            replacements[2] = SingerConfigs.GetConfig(cName) ?? "Singer Specialized Ltd.";
            replacements[3] = SingerConfigs.GetConfig(cAddress);
            replacements[4] = SingerConfigs.GetConfig(cCity);
            replacements[5] = SingerConfigs.GetConfig(cPhone);
            
            return string.Format(html, replacements);
        }

        private static string GetRecipient(Quote quote)
        {
            var content = new StringBuilder();

            const string header = @"
                <div id=""recipient"">
                    <span id=""quote_date"">{0}</span>
        
                    <table>
                        <tr>                    
                            <td id=""address"">
            ";
            const string middle = @"
                            </td>
                            <td id=""contact"">
            ";
            const string footer = @"
                            </td>
                        </tr>
                    </table>
                </div>
            ";

            var address = quote.BillingAddress;
            var contact = quote.Contact;

            content.Append(string.Format(header, DateTime.Now.ToString(SingerConfigs.PrintedDateFormatString)));            
            content.Append("<span>" + quote.Company.Name + "</span>");

            if (quote.CareOfCompany != null)
                content.Append("<span>c/o " + quote.CareOfCompany.Name + "</span>");

            if (address != null)
            {
                if (address.Line1 != null)
                    content.Append("<span>" + address.Line1 + "</span>");

                if (address.Line2 != null)
                    content.Append("<span>" + address.Line2 + "</span>");

                var cityandprov = "" + address.City;

                if (address.ProvincesAndState != null)
                {
                    cityandprov += (cityandprov.Length == 0) ? address.ProvincesAndState.Name : ", " + address.ProvincesAndState.Name;

                    content.Append("<span>" + cityandprov + "</span>");
                    content.Append("<span>" + address.ProvincesAndState.Country.Name + "</span>");
                }

                if (address.PostalZip != null)
                    content.Append("<span>" + address.PostalZip + "</span>");
            }

            content.Append(middle);

            if (contact != null)
            {
                var telephone = "<span>Telephone: {0}</span>";
                var fax = "<span>Fax: {0}</span>";
                var secondary = "<span>Telephone: {0}</span>";
                var email = "<span>Email: {0}</span>";

                if (!string.IsNullOrEmpty(contact.PrimaryPhone))
                    telephone = string.Format(telephone, contact.PrimaryPhone);
                else
                    telephone = (!string.IsNullOrEmpty(address.PrimaryPhone)) ? string.Format(telephone, address.PrimaryPhone) : "";                    
                
                if (!string.IsNullOrEmpty(contact.SecondaryPhone))
                    secondary = string.Format(secondary, contact.SecondaryPhone);
                else
                    secondary = (address != null && !string.IsNullOrEmpty(address.SecondaryPhone)) ? string.Format(secondary, address.SecondaryPhone) : "";

                if (!string.IsNullOrEmpty(contact.Fax))
                    fax = string.Format(fax, contact.Fax);
                else
                    fax = (address != null && !string.IsNullOrEmpty(address.Fax)) ? string.Format(fax, address.Fax) : "";
                
                email = (!string.IsNullOrEmpty(contact.Email)) ? string.Format(email, contact.Email) : "";           
                

                content.Append(telephone);
                content.Append(secondary);
                content.Append(fax);
                content.Append(email);
            }

            content.Append(footer);


            return content.ToString();
        }

        private static string GetDescription(Contact recipient, string subject, DateTime? openDate, DateTime? closingDate)
        {
            var html = @"
                <div id=""description"">
                    <table>
                        {0}                        
                    </table>

                    <p>As per your quotation request we are pleased to submit the following proposal, valid until {2}:</p>
                </div>
            ";

            if (openDate == null)
            {
                openDate = DateTime.Now;
                closingDate = openDate.Value.AddDays(30);
            }

            if (closingDate == null)
            {
                closingDate = openDate.Value.AddDays(30);
            }

            var replacements = new object[3];

            replacements[0] = (recipient != null) ? string.Format(@"<tr><td class=""fieldname"">Attention:</td><td>{0}</td></tr>", recipient.Name) : "";
            replacements[1] = subject;
            replacements[2] = closingDate.Value.ToString(SingerConfigs.PrintedDateFormatString);

            return string.Format(html, replacements);
        }
        
        private string GetCommodities(Quote quote)
        {
            var content = @"
                <div id=""commodities"">
                    <table class=""itemized"">
                        <thead>
                            <tr>
                                <th>Item</th>
                                <th>Description</th>
                                <th>From</th>
                                <th>To</th>
                                <th>Dimensions <span class=""sub"">(LxWxH)</span></th>
                                <th>Weight</th>                                
                                {0}
                            </tr>
                        </thead>
                        <tbody>
                            {1}
                        </tbody>
                    </table>

                    <p class=""fine_print""><em>*</em> Dimensions or weights estimated. Actual values may impact quoted price.</p>
                </div>
            ";
            string costcol = (quote.IsItemizedBilling == true) ? string.Format("<th>{0}</th>", quote.PrintoutCostHeading) : null;


            var rows = new StringBuilder();            

            foreach (var commodity in (from qc in quote.QuoteCommodities orderby qc.OrderIndex select qc))
            {
                string length, width, height, weight;

                if (PrintMetric != true)
                {
                    length = width = height = MeasurementFormater.UFeet;
                    weight = MeasurementFormater.UPounds;
                }
                else
                {
                    length = width = height = MeasurementFormater.UMetres;
                    weight = MeasurementFormater.UKilograms;
                }

                string depart, arrive, description;

                if (string.IsNullOrEmpty(commodity.ArrivalAddress) || string.IsNullOrEmpty(commodity.ArrivalSiteName))
                    arrive = string.Format("{0} {1}", commodity.ArrivalSiteName, commodity.ArrivalAddress);
                else
                    arrive = string.Format("{0} - {1}", commodity.ArrivalSiteName, commodity.ArrivalAddress);

                if (string.IsNullOrEmpty(commodity.DepartureAddress) || string.IsNullOrEmpty(commodity.DepartureSiteName))
                    depart = string.Format("{0} {1}", commodity.DepartureSiteName, commodity.DepartureAddress);
                else
                    depart = string.Format("{0} - {1}", commodity.DepartureAddress, commodity.DepartureSiteName);
                
                if (string.IsNullOrEmpty(commodity.Unit))
                    description = commodity.Name;
                else
                    description = string.Format("{0} - {1}", commodity.Name, commodity.Unit);

                rows.Append(@"<tr class=""details"">");
                rows.Append("<td>");
                rows.Append(commodity.OrderIndex);
                rows.Append("</td>");
                rows.Append("<td>");
                rows.Append(description);
                rows.Append("</td>");
                rows.Append("<td>");
                rows.Append(depart);               
                rows.Append("</td>");
                rows.Append("<td>");
                rows.Append(arrive);
                rows.Append("</td>");
                rows.Append("<td>");
                rows.Append(MeasurementFormater.FromMetres(commodity.Length, length));
                rows.Append(" x ");
                rows.Append(MeasurementFormater.FromMetres(commodity.Width, width));
                rows.Append(" x ");
                rows.Append(MeasurementFormater.FromMetres(commodity.Height, height));
                rows.Append("</td>");
                rows.Append("<td>");
                rows.Append(MeasurementFormater.FromKilograms(commodity.Weight, weight));
                rows.Append("</td>");

                if (costcol != null)
                {
                    rows.Append("<td>");
                    rows.Append(string.Format("{0:C}", commodity.Cost));
                    rows.Append("</td>");
                }

                rows.Append("</tr>");

                if (!string.IsNullOrEmpty(commodity.Notes))
                {
                    rows.Append(@"<tr class=""notes"">");
                    rows.Append("<td></td>");

                    if (costcol != null)
                        rows.Append(@"<td colspan=""5"">");
                    else
                        rows.Append(@"<td colspan=""4"">");

                    rows.Append(commodity.Notes.Replace("\n", "<br>"));
                    rows.Append("</td>");
                    rows.Append("<td></td>");
                    rows.Append("</tr>");
                }
            }

            content = string.Format(content, costcol, quote.QuoteCommodities.Count > 0 ? rows.ToString() : "");

            return content;
        }

        private static string GetSuppluments(IEnumerable<QuoteSupplement> supplements)
        {
            var content = @"
                <div id=""supplements"">
                    <table class=""itemized"">
                        <thead>
                            <tr>
                                <th>Item</th>
                                <th>Description</th>
                                <th>Billed By</th>
                                <th>Quantity</th>                                
                                <th>Cost</th>
                            </tr>
                        </thead>
                        <tbody>
                            {0}
                        </tbody>
                    </table>
                </div>
            ";

            var rows = new StringBuilder();
            int count = 1;

            foreach (QuoteSupplement supplement in supplements)
            {
                rows.Append("<tr>");
                rows.Append("<td>");
                rows.Append(count);
                rows.Append("</td>");
                rows.Append("<td>");
                rows.Append(supplement.Name);
                rows.Append("</td>");
                rows.Append("<td>");

                if (supplement.BillingType != null)
                    rows.Append(supplement.BillingType.Name);

                rows.Append("</td>");                
                rows.Append("<td>");
                rows.Append(supplement.Quantity);                
                rows.Append("</td>");
                rows.Append("<td>");

                if (supplement.BillingType != null && supplement.BillingType.Name != "Cost Included")
                {
                    rows.Append(string.Format("{0:C}", supplement.Cost));
                }

                rows.Append("</td>");
                rows.Append("</tr>");

                count++;
            }

            content = string.Format(content, count > 0 ? rows.ToString() : "");

            return content;
        }

        private static string GetNotes(Quote quote)
        {
            if (quote.QuoteNotes.Count < 1)
                return "";

            const string html = @"
                <div id=""notes"">
                    <span>Notes:</span>

                    <ol class=""conditions"">
                        {0}
                    </ol>
                </div>
            ";
            const string line = "<li>{0}</li>";

            var rows = new StringBuilder();
            foreach (var item in quote.QuoteNotes)
            {
                rows.Append(string.Format(line, item.Note.Replace("\n", "<br>").Replace("  ", "&nbsp;")));
            }            

            return string.Format(html, rows.ToString());
        }

        private static string GetPrice(Quote quote)
        {
            var html = @"
                <div id=""price"">
                    <p><span class=""heading"">Quoted Price:</span><span class=""price"">{0}</span> <span class=""price_note"">{1}</span></p>
                </div>
            ";           

            var price = (quote.IsItemizedBilling == true || quote.Price == 0.0m) ? "As listed" : string.Format("{0:C}", quote.Price);
            var note = quote.PriceNote;

            return string.Format(html, price, note);
        }

        private static string GetInclusions(Quote quote)
        {
            var html = @"
                <div id=""inclusions"">
                    <p>The quoted price includes {0}.</p>
                </div>
            ";

            if (quote.QuoteInclusions.Count == 0) return "";

            var inclusionlist = new StringBuilder();

            for (var i = 0; i < quote.QuoteInclusions.Count; i++)            
            {
                inclusionlist.Append(quote.QuoteInclusions[i].Inclusion.Line);

                if (i + 1 != quote.QuoteInclusions.Count)
                {
                    inclusionlist.Append(", ");
                }
            }

            return string.Format(html, inclusionlist.ToString());
        }

        private static string GetConditions(Quote quote)
        {
            if (quote.QuoteConditions.Count < 1) 
                return "";

            var builder = new StringBuilder();
    
            const string header = @"
                <div id=""conditions"">
                    <p>This quotation is subject to the following conditions:</p>

                    <ol class=""conditions"">
            ";
            const string line = "<li>{0}</li>";
            const string footer = @"
                     </ol>
                </div>
            ";


            builder.Append(header);

            var conditions = from qc in quote.QuoteConditions select qc;            

            foreach (var condition in conditions)
            {
                try
                {
                    builder.Append(string.Format(line, condition.Line));
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
            }

            builder.Append(footer);


            return builder.ToString();
        }

        private static string GetSignoff(Quote quote)
        {
            var content = @"
                <div id=""signoff"">
                    <p>{0}</p>

                    <p>Sincerely,</p>
                    <br><br><br>
                    <p class=""author"">
                        {1}
                    </p>
                </div>
            ";

            

            string employee;
            
            if (quote.Employee != null)            
            {
                employee = quote.Employee.Name;

                if (!string.IsNullOrWhiteSpace(quote.Employee.JobTitle))
                {
                    employee += "<br>";
                    employee += quote.Employee.JobTitle;
                }

                if (!string.IsNullOrWhiteSpace(quote.Employee.Email))
                {
                    employee += "<br>";
                    employee += quote.Employee.Email;
                }
            }
            else
                employee = "Dan Klassen";
            

            

            content = string.Format(content, SingerConfigs.GetConfig("Quote-DefaultSignoff") ?? "", employee);

            return content;
        }
    }
}


