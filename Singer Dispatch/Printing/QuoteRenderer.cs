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
            content.Append(GetCommodities(quote.QuoteCommodities.ToList()));
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
                        font-size: 52px;
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
                        border-spacing: 0;
                        width: 99%;
                        margin: 0 auto;
                    }

                    table.itemized th 
                    {
                        text-align: left;
                        text-transform: uppercase;
                        border-bottom: 2px #000000 solid;
                        padding: 4px;
                        padding-right: 15px;                
                    }

                    table.itemized td 
                    {
                        padding: 4px;                
                        padding-right: 25px;
                        border-bottom: 1px #000000 solid;
                    }

                    table.itemized tbody 
                    {
                        border: 1px #000000 solid;
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
                                <span class=""title"">Quote</span>
                                <span class=""quote_number"">#%QUOTE_NUMBER%</span>
                            </td>
                            <td id=""hq_location"">
                                <span class=""address"">235132 84th St. SE</span>
                                <span class=""phone"">Calgary, AB T1X 0K1</span>
                                <span class=""fax"">Phone: (403) 569-8605</span>                                
                            </td>
                        </tr>
                    </table>                        
                </div>
            ";

            var img = "file:///" + System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName), @"Images\Header.png");            

            return content.Replace("%HEADER_IMG%", img).Replace("%QUOTE_NUMBER%", quoteName);
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
        
        private static string GetCommodities(List<QuoteCommodity> commodities)
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
                </div>

                <p class=""fine_print""><em>*</em> Dimensions or weights estimated. Actual values may impact quoted price.</p>
            ";

            var rows = new StringBuilder();
            int count = 1;

            foreach (var commodity in commodities)
            {
                rows.Append("<tr>");
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

                count++;
            }

            if (count > 0)
            {
                content = content.Replace("%TABLE_BODY%", rows.ToString());
            }
            else
            {
                content = content.Replace("%TABLE_BODY%", "");
            }

            return content;
        }

        private string GetSuppluments(List<QuoteSupplement> supplements)
        {
            string content;

            content = @"
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

            if (count > 0)
            {
                content = content.Replace("%TABLE_BODY%", rows.ToString());
            }
            else
            {
                content = content.Replace("%TABLE_BODY%", "");
            }

            return content;
        }

        private static string GetConditions(Quote quote)
        {
            var builder = new StringBuilder();
    
            const string header = @"
                <div id=""conditions"">
                    <p>The quoted price includes Transportation Equipment, Permits.</p>

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
                    var replacement1 = condition.Replacement1 ?? condition.Condition.DefaultVariable1;
                    var replacement2 = condition.Replacement2 ?? condition.Condition.DefaultVariable2;
                    var replacement3 = condition.Replacement3 ?? condition.Condition.DefaultVariable3;

                    builder.Append(line.Replace("%CONDITION%", String.Format(condition.Condition.Line, replacement1, replacement2, replacement3)));
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
                    <p>We appreciate the opportunity to supply a quotation for your project.  Should you have any questions, concerns, or  comments, please feel free to contact me at your convenience.</p>

                    <p>Sincerely,</p>

                    <p class=""author"">
                        %AUTHOR%
                    </p>
                </div>
            ";

            content = content.Replace("%AUTHOR%", quote.Employee != null ? quote.Employee.Name : "Dan Klassen");

            return content;
        }
    }

    
}
