using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace SingerDispatch.Printing
{
    class QuoteRenderer
    {

        public string GenerateQuotePrintout(Quote quote)
        {
            var content = new StringBuilder();

            content.Append(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd""><html>");
            content.Append("<head>");
            content.Append(GetTitle("Quote - " + quote.Company.Name + " - " + quote.NumberAndRev));
            content.Append(GetStyles());
            content.Append("</head>");
            content.Append("<body>");
            content.Append(GetHeader(quote.NumberAndRev));
            content.Append(GetRecipient());
            content.Append(GetDescription("Some guy", "Transportation Quote", quote.CreationDate, quote.ExpirationDate));
            content.Append(GetCommodities(quote.QuoteCommodities));
            content.Append(GetSuppluments(quote.QuoteSupplements));
            content.Append(GetConditions());
            content.Append(GetSignoff());
            content.Append("</body>");
            content.Append("</html>");

            return content.ToString();
        }


        private string GetTitle(string title)
        {
            return "<title>" + title + "</title>";
        }

        private string GetStyles()
        {
            string content;

            content = @"
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
                    
                    div#description span.attention 
                    {
                        display: block;
                        font-weight: bold;
                        
                    }
                    
                    div#description span.attention span.name
                    {
                        font-weight: normal;
                        margin-left: 10px;
                    }
                    
                    div#description span.regarding
                    {
                        display: block;
                        font-weight: bold;
                        margin-bottom: 20px;
                    }
                    
                    div#description span.regarding span.subject
                    {
                        font-weight: normal;
                        margin-left: 50px;
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

        private string GetHeader(string quoteName)
        {
            string content;

            content = @"
                <div id=""header"">
                    <table>
                        <tr>
                            <td id=""logo"">
                                <span class=""logo""><img src=""logo.png"" alt=""Singer Specialized""></span>                        
                            </td>
                            <td id=""quote_name"">
                                <span class=""title"">Quote</span>
                                <span class=""quote_number"">#%QUOTE_NUMBER%</span>
                            </td>
                            <td id=""hq_location"">
                                <span class=""address"">Site 12 Box 26 RR5. Calgary, AB T2P 2G6</span>
                                <span class=""phone"">Ph: (403) 569 - 8605</span>
                                <span class=""fax"">Fax: (403) 569 - 7643</span>
                            </td>
                        </tr>
                    </table>                        
                </div>
            ";

            return content.Replace("%QUOTE_NUMBER%", quoteName);
        }

        private string GetRecipient()
        {
            string content;

            content = @"
                <div id=""recipient"">
                    <span id=""quote_date"">%TODAY%</span>
        
                    <table>
                        <tr>                    
                            <td id=""address"">
                                <span>Xyz Company</span>
                                <span>C/O Somothere Systems</span>
                                <span>Calgary, Alberta</span>
                                <span>Canada</span>
                                <span>X2X 2X3</span>
                            </td>
                            <td id=""contact"">
                                <span>Telephone: (403) 279-8388</span>
                                <span>Fax: (403) 279-8390</span>
                                <span>Email: dosomethign@xyzcompany.com</span>
                            </td>
                        </tr>
                    </table>
                </div>
            ";

            content = content.Replace("%TODAY%", DateTime.Now.ToString(SingerConstants.PrintedDateFormatString));

            return content;
        }

        private string GetDescription(string recipient, string subject, DateTime? openDate, DateTime? closingDate)
        {
            string content;

            content = @"
                <div id=""description"">
                    <span class=""attention"">Attention: <span class=""name"">%RECIPIENT_CONTACT%</span></span>
                    
                    <span class=""regarding"">Re: <span class=""subject"">%SUBJECT%</span></span>
                    
                    <p>As per your quotation of %OPEN_DATE% we are pleased to submit the following proposal, valid until %CLOSING_DATE%:</p>
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

            content = content.Replace("%RECIPIENT_CONTACT%", recipient);
            content = content.Replace("%SUBJECT%", subject);
            content = content.Replace("%OPEN_DATE%", openDate.Value.ToString(SingerConstants.PrintedDateFormatString));
            content = content.Replace("%CLOSING_DATE%", closingDate.Value.ToString(SingerConstants.PrintedDateFormatString));

            return content;
        }
        
        private string GetCommodities(EntitySet<QuoteCommodity> commodities)
        {
            string content;

            content = @"
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
                                <th>Cost</th>
                            </tr>
                        </thead>
                        <tbody>
                            %TABLE_BODY%
                        </tbody>
                    </table>
                </div>

                <p class=""fine_print""><em>*</em> Dimensions or weights estimated. Actual values may impacted quoted price.</p>
            ";

            StringBuilder rows = new StringBuilder();
            int count = 0;

            foreach (QuoteCommodity commodity in commodities)
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
                rows.Append("<td>");
                rows.Append("$");
                rows.Append(commodity.Value);
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

        private string GetSuppluments(EntitySet<QuoteSupplement> supplements)
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

            StringBuilder rows = new StringBuilder();
            int count = 0;

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

        private string GetConditions()
        {
            string content;

            content = @"
                <div id=""conditions"">
                    <p>The quoted price includes Transportation Equipment, Permits.</p>

                    <p>This quotation is subject to the following conditions:</p>

                    <ol class=""conditions"">
                        <li>Subject to Alberta infrastructure regulations of today's date.</li>
                        <li>Weights &amp; dimensions being as stated.</li>
                        <li>Availability of transport equipment at time of firm order.</li>
                        <li>Loading provided by others.</li>
                        <li>Unloading provided by others.</li>
                        <li>Wire lifting, utility services, rail crossing charges, police escorts, sign crews, bridge surveys or Engineering, if required, will be invoiced at cost plus 10%.</li>
                        <li>Allow 2 hours for loading each piece.</li>
                        <li>Allow 4 hours for unloading each piece.</li>
                        <li>Subject to fall weight restriction.</li>
                        <li>A different seasonal weight restriction may result in a recalculation of the quoted price.</li>
                        <li>Suitable access to and on site as well as ground conditions on site and in all work areas to be provided by others. If additional towing or pushing of our equipment is required because of off highway or site conditions, any cost incurred will be extra. Any damages incurred to property or equipment (including our equipment) as a result of towing or pushing will be charged as an extra.</li>
                        <li>Delays due to any reason beyond our direct control including inclement weather may result in extra charges for labor and equipment.</li>
                        <li>Our standard land transport cargo insurance coverage to a maximum $2,000,000.00 Canadian is included covering cargo that is in our direct care, custody and control. If no declared value is stated on the bill of lading, coverage is limited to $2.00/lb. Additional insurance if required, will be extra.</li>
                        <li>Sites in remote areas camps supplied by others.</li>
                    </ol>
                </div>
            ";

            return content;
        }

        private string GetSignoff()
        {
            string content;

            content = @"
                <div id=""signoff"">
                    <p>We appreciate the opportunity to supply a quotation for your project.  Should you have any questions, concerns, or  comments, please feel free to contact me at your convenience.</p>

                    <p>Sincerely,</p>

                    <p class=""author"">
                        Dan Klassen<br>
                        Operations Manager
                    </p>
                </div>
            ";

            return content;
        }
    }

    
}
