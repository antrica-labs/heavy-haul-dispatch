using System;
using System.Linq;
using System.Text;

namespace SingerDispatch.Printing.Documents
{
    class InvoiceDocument : SingerPrintDocument
    {
        public InvoiceDocument()
        {
            PrintMetric = true;
            SpecializedDocument = true;
        }
        
        public override string GenerateHTML(object entity)
        {
            return GenerateHTML((Invoice)entity);
        }

        private string GenerateHTML(Invoice invoice)
        {
            var content = new StringBuilder();

            content.Append(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd"">");
            content.Append("<html>");
            content.Append("<head>");
            content.Append(@"<meta http-equiv=""Content-Type"" content=""text/html;charset=utf-8"">");
            content.Append("<title>Singer Specialized - Invoice</title>");
            content.Append(GetStyles());            
            content.Append("</head>");
            content.Append("<body>");
            content.Append(GetHeader(invoice));
            content.Append(GetBillFrom(invoice));
            content.Append(GetAttention(invoice));
            content.Append(GetBillTo(invoice));
            content.Append(GetBreakdown(invoice));
            content.Append("</body>");
            content.Append("</html>");

            return content.ToString();
        }

        private string GetHeader(Invoice invoice)
        {
            var html = @"
                <div id=""header"">
                    <table class=""header"">
                        <tr>
                            <td id=""logo"">
                                <span class=""logo""><img src=""{0}"" alt=""Singer""></span>
                                <h3>GST Registration #{1}</h3>
                            </td>
                            <td id=""document_name"">
                                <span class=""title"">Invoice</span>
                            </td>
                            <td id=""details"">
                                {2}                             
                            </td>
                        </tr>
                    </table>                        
                </div>
            ";

            var replacements = new object[3];

            replacements[0] = GetHeaderImg();
            replacements[1] = SingerConfigs.GetConfig("SingerGSTRegistrationNumber") ?? "883578023";
            replacements[2] = GetDetails(invoice);

            return string.Format(html, replacements);
        }

        private string GetDetails(Invoice invoice)
        {
            var builder = new StringBuilder();

            var row = @"
                <tr>
                    <th>%NAME%:</th>
                    <td>%VALUE%</td>                    
                </tr>    
            ";

            builder.Append(@"<table>");

            
            builder.Append(row.Replace("%NAME%", "Date").Replace("%VALUE%", String.Format("{0:MMMM d, yyyy}", invoice.InvoiceDate)));
            builder.Append(row.Replace("%NAME%", "Invoice #").Replace("%VALUE%", invoice.ToString()));


            foreach (var item in invoice.ReferenceNumbers)
            {
                builder.Append(row.Replace("%NAME%", item.Field).Replace("%VALUE%", item.Value));
            }

            builder.Append(@"</table>");

            return builder.ToString();
        }

        private string GetBillFrom(Invoice invoice)
        {
            var html = @"
                <div id=""bill_from"" class=""subsection"">
                    <div class=""address"">
                        <span>{0}</span>
                        <span>{1}</span>
                        <span>{2}</span>
                        <span>{3}</span>
                    </div>
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

            replacements[0] = SingerConfigs.GetConfig(cName) ?? "Singer Specialized Ltd.";
            replacements[1] = SingerConfigs.GetConfig(cAddress);
            replacements[2] = SingerConfigs.GetConfig(cCity);
            replacements[3] = SingerConfigs.GetConfig(cPhone);

            return string.Format(html, replacements);
        }

        private string GetAttention(Invoice invoice)
        {
            if (invoice.Contact == null) return "";

            var content = @"
                <div id=""attention"" class=""subsection"">

                    <span class=""heading"">Attention:</span>

                    <span class=""name"">%NAME%</span>
                </div>

            ";

            return content.Replace("%NAME%", invoice.Contact.Name);
        }

        private string GetBillTo(Invoice invoice)
        {
            var builder = new StringBuilder();
            var header = @"
                <div id=""bill_to"" class=""subsection"">

                    <span class=""heading"">Bill To:</span>
                    
                    <div class=""address"">
            ";
            var line = "<span>%LINE%</span>";
            var footer = @"
                    </div>
                </div>

            ";


            var company = (invoice.Job != null && invoice.Job.CareOfCompany != null) ? invoice.Job.CareOfCompany : invoice.Company;
            
            builder.Append(header);
            builder.Append(line.Replace("%LINE%", company.Name));

            var address = (invoice.BillingAddress != null) ? invoice.BillingAddress : company.Addresses.First();
            
            if (address != null)
            {   
                if (address.Line1 != null)
                    builder.Append(line.Replace("%LINE%", address.Line1));

                if (address.Line2 != null)
                    builder.Append(line.Replace("%LINE%", address.Line2));


                var cityline = address.City;

                if (address.ProvincesAndState != null)
                {
                    cityline += ", " + address.ProvincesAndState.Name + " " + address.PostalZip;
                }
                    
                builder.Append(line.Replace("%LINE%", cityline));

                if (address.ProvincesAndState != null && address.ProvincesAndState.Country != null)
                    builder.Append(line.Replace("%LINE%", address.ProvincesAndState.Country.Name));
            }
            
            builder.Append(footer);

            return builder.ToString();
        }

        private string GetBreakdown(Invoice invoice)
        {
            var builder = new StringBuilder();
            decimal? subtotal = 0.00m;
            decimal? taxTotal = 0.00m;

            var header = @"
                <div id=""breakdown"">
                    <table class=""breakdown"">
                        <tr>
                            <th class=""date"">Date</th>
                            <th class=""description"">Description of Service</th>
                            <th class=""from"">From</th>
                            <th class=""to"">To</th>
                            <th class=""hours"">Hrs</th>
                            <th class=""cost"">Cost</th>
                            <th class=""tax"">Tax</th>
                            <th class=""amount"">Amount</th>                            
                        </tr>
            ";
            var footer = @"
                    </table>
                </div>
            ";
            var subtotalText = @"
                <tr class=""summary subtotal"">
                    <td colspan=""6"" rowspan=""4"" class=""comments"">
                        <span>For inquiries, please contact the accounts receivable department at (403) 569-7635 Mon-Fri: 9:00am - 4:00pm</span>    
                        
                        <span>All accounts due upon receipt of invoice. All accounts 60 days overdue are subject to 2.0% interest per month.</span>
                        
                        <span>Please Note: Any Wire lift charges are cost + 10%</span>
                    </td>
                    <th>Subtotal</th>
                    <td class=""dollars"">%SUBTOTAL%</td>
                </tr>
            ";
            var fuelTax = @"
                <tr class=""summary fuel_tax"">
                    <th>Fuel</th>
                    <td class=""dollars"">%FUEL_TAX%</td>
                </tr>
            ";
            var regularTax = @"
                <tr class=""summary gst"">
                    <th>TAX</th>
                    <td class=""dollars"">%TAX%</td>
                </tr>
            ";
            var total = @"
                <tr class=""summary total"">
                    <th>Total</th>
                    <td class=""dollars"">%TOTAL%</td>
                </tr>
            ";

            var taxRate = invoice.TaxRate ?? SingerConfigs.GST;

            builder.Append(header);

            foreach (var item in invoice.InvoiceLineItems)
            {                
                builder.Append(GetBreakdownLine(item, taxRate));

                var hours = item.Hours ?? 1;

                if (item.Rate != null)
                {                   
                    subtotal += (item.Rate * (decimal)hours);

                    if (item.TaxExempt != true)
                        taxTotal += (item.Rate * (decimal)hours) * taxRate;
                }

                foreach (var extra in item.Extras)
                {
                    builder.Append(GetBreakdownExtra(extra, taxRate));
                    hours = extra.Hours ?? 1;

                    if (extra.Rate != null)
                    {
                        subtotal += (extra.Rate * (decimal)hours);
                        taxTotal += (extra.Rate * (decimal)hours) * taxRate;
                    }
                        
                }
            }

            var fuelTaxTotal = subtotal * SingerConfigs.FuelTax;

            builder.Append(subtotalText.Replace("%SUBTOTAL%", String.Format("{0:C}", subtotal)));
            builder.Append(fuelTax.Replace("%FUEL_TAX%", String.Format("{0:C}", fuelTaxTotal)));
            builder.Append(regularTax.Replace("%TAX%", String.Format("{0:C}", taxTotal)));
            builder.Append(total.Replace("%TOTAL%", String.Format("{0:C}", (subtotal + fuelTaxTotal + taxTotal))));

            builder.Append(footer);

            return builder.ToString();
        }

        private string GetBreakdownLine(InvoiceLineItem item, decimal taxRate)
        {
            var builder = new StringBuilder();

            var hours = item.Hours ?? 1;
            var cost = (item.Rate * (decimal)hours);
            var tax = (item.TaxExempt != true) ? cost * taxRate : 0m;

            builder.Append("<tr>");
            builder.Append(@"<td class=""dates"">%DATES%</td>".Replace("%DATES%", String.Format("{0:MMM d, yyyy}", item.ItemDate)));
            builder.Append(@"<td class=""description"">%DESCRIPTION%</td>".Replace("%DESCRIPTION%", item.Description));
            builder.Append(@"<td class=""departure"">%DEPARTURE%</td>".Replace("%DEPARTURE%", item.Departure));
            builder.Append(@"<td class=""destination"">%DESTINATION%</td>".Replace("%DESTINATION%", item.Destination));
            builder.Append(@"<td class=""hours"">%HOURS%</td>".Replace("%HOURS%", (item.Hours != null) ? item.Hours.ToString() : ""));
            builder.Append(@"<td class=""cost"">%COST%</td>".Replace("%COST%", String.Format("{0:C}", cost)));
            builder.Append(@"<td class=""line_tax"">%LINE_TAX%</td>".Replace("%LINE_TAX%", String.Format("{0:C}", tax)));
            builder.Append(@"<td class=""amount"">%AMOUNT%</td>".Replace("%AMOUNT%", String.Format("{0:C}", cost + tax)));
            builder.Append("</tr>");
            
            return builder.ToString();
        }

        private string GetBreakdownExtra(InvoiceExtra item, decimal taxRate)
        {
            var builder = new StringBuilder();

            var hours = item.Hours ?? 1;           
            var cost = (item.Rate * (decimal)hours);

            builder.Append("<tr>");
            builder.Append(@"<td class=""dates""></td>");
            builder.Append(@"<td class=""description"">%DESCRIPTION%</td>".Replace("%DESCRIPTION%", item.Description));
            builder.Append(@"<td class=""departure""></td>");
            builder.Append(@"<td class=""destination""></td>");
            builder.Append(@"<td class=""hours"">%HOURS%</td>".Replace("%HOURS%", (item.Hours != null) ? item.Hours.ToString() : ""));
            builder.Append(@"<td class=""cost"">%COST%</td>".Replace("%COST%", String.Format("{0:C}", cost)));
            builder.Append(@"<td class=""line_tax"">%LINE_TAX%</td>".Replace("%LINE_TAX%", String.Format("{0:C}", cost * taxRate)));
            builder.Append(@"<td class=""amount"">%AMOUNT%</td>".Replace("%AMOUNT%", String.Format("{0:C}", cost * (1 + taxRate))));
            builder.Append("</tr>");

            return builder.ToString();
        }

        private string GetStyles()
        {
            string content = @"
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
                        line-height: 1.3em;
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
                    }

                    /*******/

                    body
                    {          
                        padding: 1em;       
                        font-size: 12px;
                        font-family: sans-serif;
                        background-color: #FFFFFF;
                    }

                    th
                    {
                        text-align: left;
                    }

                    h1
                    {
                        font-size: 2.0em;
                        text-transform: uppercase;
                        color: #004127;
                        text-align: center;
                        margin-bottom: 0.2em;
                    }

                    h2
                    {
                        font-size: 1.5em;
                        font-weight: normal;
                        margin-bottom: 0.1em;
                    }

                    h3
                    {
                        font-size: 1em;
                        font-weight: normal;
                    }

                    div#header 
                    {                                
                        
                    }
                                
                    div#header span 
                    {
                        display: block;
                    }
                    
                    div#header table.header
                    {
                        width: 100%;
                    }
                    
                    div#header table.header td#logo,
                    div#header table.header td#document_name,
                    div#header table.header td#details
                    {                
                        width: 33%; 
                        vertical-align: top;
                    }
                    
                    div#header table.header td#logo
                    {
                        text-align: left;
                    }
                    
                    div#header td#document_name 
                    {                
                        text-align: center;              
                    }
                    
                    div#header td#document_name span.title 
                    {                
                        font-size: 2em;
                        text-transform: uppercase;
                        padding-top: 1em;
                    }
                    
                    div#header td#details 
                    {                
                        
                    }

                    div#header td#details table
                    {
                        float: right;
                        border-collapse: collapse;
                    }

                    div#header td#details table th
                    {
                        padding: 0.2em 0.9em 0.2em;
                    }

                    div#header td#details td
                    {   
                        border-bottom: 1px #CACACA solid;
                        text-align: center;
                        padding: 0 0.2em;
                    }

                    div.subsection 
                    {
                        width: 40%;	
                        margin: 2em 0;
                    }

                    div.subsection span
                    {
                        display: block;
                    }

                    div.subsection span.heading
                    {	
                        text-transform: uppercase;
                        border-bottom: 1px #CACACA solid;
                        font-weight: bold;
                        margin: -0.3em;
                        margin-bottom: 0.3em;
                        padding: 0.1em 0.3em;
                        font-weight: bold;
                    }

                    div#attention
                    {
                        margin-top: 1.5em;	
                    }

                    div#attention span.name
                    {
                        font-weight: bold;
                    }


                    div#breakdown
                    {
                        clear: both;
                    }

                    div#breakdown table.breakdown
                    {
                        width: 100%;
                        border-collapse: collapse;
                    }

                    div#breakdown table.breakdown span
                    {
                        display: block;
                    }

                    div#breakdown table.breakdown th
                    {
                        text-align: center;
                        text-transform: uppercase;	
                        background-color: #004127;
                        color: #FFFFFF;
                        font-weight: bold;
                        padding: 0 0.4em;
                    }

                    div#breakdown table.breakdown th.date,
                    div#breakdown table.breakdown th.description,
                    div#breakdown table.breakdown th.from,
                    div#breakdown table.breakdown th.to
                    {
                        text-align: left;
                    }

                    div#breakdown table.breakdown td
                    {
                        text-align: center;
                        padding: 0.6em 0.4em;
                    }

                    div#breakdown table.breakdown td.dates,
                    div#breakdown table.breakdown td.description,
                    div#breakdown table.breakdown td.departure,
                    div#breakdown table.breakdown td.destination
                    {
                        text-align: left;
                    }

                    div#breakdown table.breakdown td.amount
                    {
                        text-align: right;
                        background-color: #EDEDED;
                    }

                    div#breakdown table.breakdown tr.subtotal td, div#breakdown table.breakdown tr.subtotal th
                    {
                        border-top: 1px #CACACA solid;
                    }

                    div#breakdown table.breakdown tr.summary th
                    {
                        color: #000000;                        
                        text-align: left;
                        vertical-align: middle;                        
                        background-color: transparent;
                        padding-right: 0.4em;
                    }

                    div#breakdown table.breakdown tr.summary td.dollars
                    {
                        vertical-align: middle;
                        background-color: #EDEDED;
                    }

                    div#breakdown table.breakdown td.comments
                    {
                        text-align: left;
                        padding-right: 1em;
                    }

                    div#breakdown table.breakdown td.comments span
                    {
                        font-size: 0.9em;
                        font-weight: bold;
                        margin: 0.4em 0;
                    }

                    div#breakdown table.breakdown tr.summary td.dollars
                    {
                        text-align: right;
                    }

                    div#breakdown table.breakdown tr.total
                    {
                        border-top: 1px #CACACA solid;
                        font-weight: bold;
                    }
                </style>
                <style type=""text/css"" media=""print"">
                    body
                    {
                        font-size: 8pt;
                        padding: 0;
                    }
                </style>
            ";

            return content;
        }
    }
}


