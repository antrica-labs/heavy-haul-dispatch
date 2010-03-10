using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace SingerDispatch.Printing
{
    class QuoteRenderer : IRenderer
    {
        public string GenerateHTML(object quote)
        {
            return GenerateHTML((Quote)quote);
        }

        public string GenerateHTML(Quote quote)
        {
            var content = new StringBuilder();

            content.Append(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd""><html>");
            content.Append("<head>");
            content.Append(@"<meta http-equiv=""Content-Type"" content=""text/html;charset=utf-8"">");
            content.Append(GetTitle("Quote - " + quote.Company.Name + " - " + quote.NumberAndRev));
            content.Append(GetStyles());
            content.Append("</head>");
            content.Append("<body>");
            content.Append(GetHeader(quote.NumberAndRev));
            content.Append(GetRecipient(quote.BillingAddress, quote.Contact));
            content.Append(GetDescription(quote.Contact, "Transportation Quote", quote.CreationDate, quote.ExpirationDate));

            if (quote.QuoteCommodities.Count > 0)
                content.Append(GetCommodities(quote.QuoteCommodities.ToList()));

            if (quote.QuoteSupplements.Count > 0)
                content.Append(GetSuppluments(quote.QuoteSupplements.ToList()));

            content.Append(GetConditions(quote));
            content.Append(GetSignoff(quote));
            content.Append("</body>");
            content.Append("</html>");

            return content.ToString();
        }


        private static string GetTitle(string title)
        {
            return "<title>" + title + "</title>";
        }

        private static string GetStyles()
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

                    /***** QUOTE SPECIFIC STYLES *****/

                    body
                    {
                        font-size: 10pt;
                        font-family: Verdana, Arial, Helvetica, sans-serif;
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
                        margin: 20px 0;
                    }
                    
                    div#header td#hq_location 
                    {                
                        text-align: right;
                    }
                    
                    
                    div#recipient 
                    {             
                        margin-top: 50px;
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
                        margin-bottom: 20px;
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
                        padding-top: 30px;
                        margin-bottom: 25px;
                    }
                    
                    div#description table
                    {
                    	margin-bottom: 10px;
                    }
                    
                    div#description td.fieldname 
                    {                        
                        font-weight: bold;
                        padding-right: 10px;                        
                    }
                    
                    div#commodities 
                    {
                        clear: both;
                        margin-bottom: 30px;
                    }

                    div#supplements 
                    {
                        margin-bottom: 20px;
                    }


                    /****** GENERAL STYLES *****/
                    p
                    {
                        margin-bottom: 15px;
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
                        border-bottom: 1px #000000 solid;
                        padding: 4px;
                        padding-right: 15px;                
                    }

                    table.itemized td 
                    {
                    	border-top: 1px;
                        padding: 4px;                
                        padding-right: 25px;
                    }
                    
                    table.itemized tr.details
                    {
                        border-top: 1px #000000 solid;                       
                    }

                    table.itemized tr.notes
                    {
                    	font-style: italic;
                    }

                    table.itemized tr.notes td
                    {
                        padding-bottom: 10px;
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
                    
                    ol.conditions
                    {
                        list-style-type: decimal;
                        list-style-position: outside;
                        padding-left: 25px;
                        margin: 15px 25px;
                    }
                    
                    ol.conditions li 
                    {
                        padding: 2px 0;
                    }
                </style>
            ";

            return content;
        }

        private static string GetHeader(string quoteName)
        {
            const string content = @"
                <div id=""header"">
                    <table>
                        <tr>
                            <td id=""logo"">
                                <span class=""logo""><img src=""%HEADER_IMG%"" alt=""Singer Specialized""></span>                        
                            </td>
                            <td id=""quote_name"">
                                <span class=""title"">Quote #%QUOTE_NUMBER%</span>
                            </td>
                            <td id=""hq_location"">
                                <span class=""address"">%STREET_ADDRESS%</span>
                                <span class=""phone"">%CITY%</span>
                                <span class=""fax"">Phone: %PHONE%</span>                                
                            </td>
                        </tr>
                    </table>                        
                </div>
            ";

            var address = SingerConstants.GetConfig("SingerAddress-StreetAddress");
            var city = SingerConstants.GetConfig("SingerAddress-City");
            var phone = SingerConstants.GetConfig("SingerAddress-Phone");

            var process = System.Diagnostics.Process.GetCurrentProcess();
            var img = "";

            if (process.MainModule != null)
            {
                img = "file:///" + System.IO.Path.Combine(System.IO.Path.GetDirectoryName(process.MainModule.FileName), @"Images\Header.png");
            }

            return content.Replace("%HEADER_IMG%", img).Replace("%QUOTE_NUMBER%", quoteName).Replace("%STREET_ADDRESS%", address).Replace("%CITY%", city).Replace("%PHONE%", phone);
        }

        private static string GetRecipient(Address address, Contact contact)
        {
            var content = new StringBuilder();

            const string header = @"
                <div id=""recipient"">
                    <span id=""quote_date"">%TODAY%</span>
        
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

            content.Append(header.Replace("%TODAY%", DateTime.Now.ToString(SingerConstants.PrintedDateFormatString)));

            if (address != null)
            {
                if (address.Company != null)
                    content.Append("<span>" + address.Company.Name + "</span>");

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
                if (contact.PrimaryPhone != null)
                    content.Append("<span>Telephone: " + contact.PrimaryPhone + "</span>");

                if (contact.SecondaryPhone != null)
                    content.Append("<span>Telephone: " + contact.SecondaryPhone + "</span>");

                if (contact.Fax != null)
                    content.Append("<span>Fax: " + contact.Fax + "</span>");

                if (contact.Email != null)
                    content.Append("<span>Email: " + contact.Email + "</span>");
            }

            content.Append(footer);


            return content.ToString();
        }

        private static string GetDescription(Contact recipient, string subject, DateTime? openDate, DateTime? closingDate)
        {
            var content = @"
                <div id=""description"">
                    <table>
                        %ATTENTION%
                        <tr><td class=""fieldname"">Re:</td><td>%SUBJECT%</td></tr>
                    </table>

                    <p>As per your quotation of %OPEN_DATE% we are pleased to submit the following proposal, valid until %CLOSING_DATE%:</p>
                </div>
            ";

            var attention = (recipient != null) ? @"<tr><td class=""fieldname"">Attention:</td><td>" + recipient.Name + "</td></tr>" : "";

            if (openDate == null)
            {
                openDate = DateTime.Now;
                closingDate = openDate.Value.AddDays(30);
            }

            if (closingDate == null)
            {
                closingDate = openDate.Value.AddDays(30);
            }

            content = content.Replace("%ATTENTION%", attention);
            content = content.Replace("%SUBJECT%", subject);
            content = content.Replace("%OPEN_DATE%", openDate.Value.ToString(SingerConstants.PrintedDateFormatString));
            content = content.Replace("%CLOSING_DATE%", closingDate.Value.ToString(SingerConstants.PrintedDateFormatString));

            return content;
        }
        
        private static string GetCommodities(IEnumerable<QuoteCommodity> commodities)
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
                                <th>Dimensions (LxWxH)</th>
                                <th>Weight (kg)</th>                                
                            </tr>
                        </thead>
                        <tbody>
                            %TABLE_BODY%
                        </tbody>
                    </table>

                    <p class=""fine_print""><em>*</em> Dimensions or weights estimated. Actual values may impact quoted price.</p>
                </div>
            ";

            var rows = new StringBuilder();
            int count = 1;

            foreach (var commodity in commodities)
            {
                rows.Append(@"<tr class=""details"">");
                rows.Append("<td>");
                rows.Append(count);
                rows.Append("</td>");
                rows.Append("<td>");
                rows.Append(commodity.Name);
                rows.Append("</td>");
                rows.Append("<td>");
                rows.Append(commodity.DepartureSiteName);               
                rows.Append("</td>");
                rows.Append("<td>");
                rows.Append(commodity.ArrivalSiteName);
                rows.Append("</td>");
                rows.Append("<td>");
                rows.Append(commodity.Length);
                rows.Append("x");
                rows.Append(commodity.Width);
                rows.Append("x");
                rows.Append(commodity.Height);
                rows.Append("</td>");
                rows.Append("<td>");
                rows.Append(commodity.Weight);
                rows.Append("</td>");
                rows.Append("</tr>");

                if (!string.IsNullOrEmpty(commodity.Notes))
                {
                    rows.Append(@"<tr class=""notes"">");
                    rows.Append("<td></td>");
                    rows.Append(@"<td colspan=""4"">");
                    rows.Append(commodity.Notes.Replace("\n", "<br>"));
                    rows.Append("</td>");
                    rows.Append("<td></td>");
                    rows.Append("</tr>");
                }

                count++;
            }

            content = content.Replace("%TABLE_BODY%", count > 0 ? rows.ToString() : "");

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
                                <th>Price Per</th>
                                <th>Total</th>
                            </tr>
                        </thead>
                        <tbody>
                            %TABLE_BODY%
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
                rows.Append(supplement.CostPerItem);
                rows.Append("</td>");
                rows.Append("<td>");

                if (supplement.BillingType != null && supplement.BillingType.Name != "Cost Included")
                {
                    rows.Append("$");
                    rows.Append(supplement.CostPerItem * supplement.Quantity);
                }

                rows.Append("</td>");
                rows.Append("</tr>");

                count++;
            }

            content = content.Replace("%TABLE_BODY%", count > 0 ? rows.ToString() : "");

            return content;
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
            const string line = "<li>%CONDITION%</li>";
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
                    builder.Append(line.Replace("%CONDITION%", condition.Line));
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
                    <p>%SIGNOFF%</p>

                    <p>Sincerely,</p>

                    <p class=""author"">
                        %AUTHOR%
                    </p>
                </div>
            ";

            content = content.Replace("%SIGNOFF%", SingerConstants.GetConfig("Quote-DefaultSignoff") ?? "").Replace("%AUTHOR%", quote.Employee != null ? quote.Employee.Name : "Dan Klassen");

            return content;
        }
    }

    
}
