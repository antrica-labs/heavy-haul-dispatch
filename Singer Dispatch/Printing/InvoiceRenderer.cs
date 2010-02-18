﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingerDispatch.Printing
{
    class InvoiceRenderer : Renderer
    {
        public string GenerateHTML(object invoice)
        {
            return GenerateHTML((Invoice)invoice);
        }

        public string GenerateHTML(Invoice invoice)
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
            content.Append("<h1>Invoice</h1>");
            content.Append(GetDetails(invoice));
            content.Append("<h2>Singer Specialized</h2>");
            content.Append("<h3>GST Registration #883578023</h3>");
            content.Append(GetBillFrom(invoice));
            content.Append(GetAttention(invoice));
            content.Append(GetBillTo(invoice));
            content.Append(GetBreakdown(invoice));
            content.Append("</body>");
            content.Append("</html>");

            return content.ToString();
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

            builder.Append(@"
                <div id=""details"">
                    <table class=""details"">
            ");

            builder.Append(row.Replace("%NAME%", "Date").Replace("%VALUE%", String.Format("{0:MMMM d, yyyy}", invoice.InvoiceDate)));
            builder.Append(row.Replace("%NAME%", "Invoice #").Replace("%VALUE%", invoice.Number.ToString()));

            builder.Append(@"
                    </table>
                </div>
            ");


            return builder.ToString();
        }

        private string GetBillFrom(Invoice invoice)
        {
            var content = @"
                <div id=""bill_from"" class=""subsection"">
                    <div class=""address"">
                        <span>235132 84th St. SE</span>
                        <span>Calgary, AB T1X 0K1</span>
                        <span>Phone: (403) 569-8605</span>
                    </div>
                </div>

            ";
            return content;
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


            var company = (invoice.Job.CareOfCompany != null) ? invoice.Job.CareOfCompany : invoice.Job.Company;
            
            builder.Append(header);
            builder.Append(line.Replace("%LINE%", company.Name));

            if (company.Addresses.Count > 0)
            {
                var address = company.Addresses.First();
                
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
            decimal? running = 0.00m;
            var header = @"
                <div id=""breakdown"">
                    <table class=""breakdown"">
                        <tr>
                            <th>Date</th>
                            <th>Description of Service</th>
                            <th>From</th>
                            <th>To</th>
                            <th>Hrs</th>
                            <th>Cost</th>
                            <th>GST</th>
                            <th>Amount</th>
                        </tr>
            ";
            var footer = @"
                    </table>
                </div>
            ";
            var subtotal = @"
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
            var gst = @"
                <tr class=""summary gst"">
                    <th>GST</th>
                    <td class=""dollars"">%GST%</td>
                </tr>
            ";
            var total = @"
                <tr class=""summary total"">
                    <th>Total</th>
                    <td class=""dollars"">%TOTAL%</td>
                </tr>
            ";

            var tax = invoice.GSTExempt == true ? 0.00m : SingerConstants.GST;

            builder.Append(header);

            foreach (var item in invoice.InvoiceLineItems)
            {                
                builder.Append(GetBreakdownLine(item, tax));

                if (item.Cost != null)
                    running += item.Cost;
            }

            foreach (var item in invoice.InvoiceExtras)
            {
                builder.Append(GetBreakdownExtra(item, tax));

                if (item.Cost != null)
                    running += item.Cost;
            }

            builder.Append(subtotal.Replace("%SUBTOTAL%", String.Format("{0:C}", running)));
            builder.Append(fuelTax.Replace("%FUEL_TAX%", String.Format("{0:C}", running * SingerConstants.FuelTax)));
            builder.Append(gst.Replace("%GST%", String.Format("{0:C}", (running * (1 + SingerConstants.FuelTax)) * tax)));
            builder.Append(total.Replace("%TOTAL%", String.Format("{0:C}", (running * (1 + SingerConstants.FuelTax)) * (1 + tax))));

            builder.Append(footer);

            return builder.ToString();
        }

        private string GetBreakdownLine(InvoiceLineItem item, decimal gst)
        {
            var builder = new StringBuilder();
            
            builder.Append("<tr>");
            builder.Append(@"<td class=""dates"">%DATES%</td>".Replace("%DATES%", String.Format("{0:MMM d, yyyy}", item.StartDate)));
            builder.Append(@"<td class=""description"">%DESCRIPTION%</td>".Replace("%DESCRIPTION%", item.Description));
            builder.Append(@"<td class=""departure"">%DEPARTURE%</td>".Replace("%DEPARTURE%", item.Departure));
            builder.Append(@"<td class=""destination"">%DESTINATION%</td>".Replace("%DESTINATION%", item.Destination));
            builder.Append(@"<td class=""hours"">%HOURS%</td>".Replace("%HOURS%", item.Hours.ToString()));
            builder.Append(@"<td class=""cost"">%COST%</td>".Replace("%COST%", String.Format("{0:C}", item.Cost)));
            builder.Append(@"<td class=""line_tax"">%LINE_TAX%</td>".Replace("%LINE_TAX%", String.Format("{0:C}", item.Cost * gst)));
            builder.Append(@"<td class=""amount"">%AMOUNT%</td>".Replace("%AMOUNT%", String.Format("{0:C}", item.Cost * (1 + gst))));
            builder.Append("</tr>");
            
            return builder.ToString();
        }

        private string GetBreakdownExtra(InvoiceExtra item, decimal gst)
        {
            var builder = new StringBuilder();

            builder.Append("<tr>");
            builder.Append(@"<td class=""dates"">%DATES%</td>".Replace("%DATES%", String.Format("{0:MMM d, yyyy}", item.StartDate)));
            builder.Append(@"<td class=""description"">%DESCRIPTION%</td>".Replace("%DESCRIPTION%", item.Description));
            builder.Append(@"<td class=""departure"">%DEPARTURE%</td>".Replace("%DEPARTURE%", item.Departure));
            builder.Append(@"<td class=""destination"">%DESTINATION%</td>".Replace("%DESTINATION%", item.Destination));
            builder.Append(@"<td class=""hours"">%HOURS%</td>".Replace("%HOURS%", item.Hours.ToString()));
            builder.Append(@"<td class=""cost"">%COST%</td>".Replace("%COST%", String.Format("{0:C}", item.Cost)));
            builder.Append(@"<td class=""line_tax"">%LINE_TAX%</td>".Replace("%LINE_TAX%", String.Format("{0:C}", item.Cost * gst)));
            builder.Append(@"<td class=""amount"">%AMOUNT%</td>".Replace("%AMOUNT%", String.Format("{0:C}", item.Cost * (1 + gst))));
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
	                    margin: 20px;
                        font-size: 10pt;
                        font-family: Trebuchet MS, Arial, Helvetica, Tahoma, sans-serif;
                    }

                    th
                    {
                        text-align: left;
                    }

                    h1
                    {
	                    font-size: 2.0em;
	                    text-transform: uppercase;
	                    color: #8393C9;
	                    text-align: right;
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

                    div#details
                    {
	                    float: right;
                    }


                    div#details table.details 
                    {
	                    border-collapse: collapse;
                    }

                    div#details table.details th
                    {
	                    padding: 0.2em 0.9em 0.2em;
                    }

                    div#details td
                    {
	                    border-bottom: 1px #838383 dotted;
	                    text-align: center;
                        padding: 0 0.2em;
                    }

                    div.subsection 
                    {
	                    width: 40%;	
	                    margin: 1.2em 0;
                    }

                    div.subsection span
                    {
	                    display: block;
                    }

                    div.subsection span.heading
                    {	
	                    text-transform: uppercase;
                        border-bottom: 1px #838383 dotted;
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
	                    background-color: #3A4D86;
	                    color: #FFFFFF;
	                    font-weight: bold;
                    }

                    div#breakdown table.breakdown td
                    {
	                    text-align: center;
	                    padding: 0.6em 0.8em;
                    }

                    div#breakdown table.breakdown td.dates
                    {
	                    text-align: left;
                    }

                    div#breakdown table.breakdown td.description
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
	                    border-top: 1px #838383 dotted;
                    }

                    div#breakdown table.breakdown tr.summary th
                    {
	                    color: #000000;
	                    background-color: transparent;
	                    text-align: left;
                    }

                    div#breakdown table.breakdown tr.summary td
                    {
	                    padding: 0.2em 0.8em;
                        background-color: #EDEDED;
                    }

                    div#breakdown table.breakdown td.comments
                    {
	                    text-align: left;
                        background-color: transparent !important;
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
	                    border-top: 1px #838383 dotted;
	                    font-weight: bold;
                    }
                </style>
            ";

            return content;
        }
    }
}

