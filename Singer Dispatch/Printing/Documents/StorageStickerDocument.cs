using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SingerDispatch.Utils;

namespace SingerDispatch.Printing.Documents
{
    class StorageStickerDocument : SingerPrintDocument
    {
        public StorageStickerDocument()
        {
            PrintMetric = true;
            SpecializedDocument = true;
        }

        public override string GenerateHTML(object entity)
        {
            return GenerateHTML((StorageItem)entity);
        }

        private string GenerateHTML(StorageItem item)
        {
            var content = new StringBuilder();

            content.Append(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd""><html>");
            content.Append("<head>");
            content.Append(@"<meta http-equiv=""Content-Type"" content=""text/html;charset=utf-8"">");
            content.Append(GetTitle("Storage Sticker - " + item.Number));
            content.Append(GetStyles());
            content.Append("</head>");
            content.Append("<body>");
            content.Append(GetHeader(item));
            content.Append(GetCommodity(item));
            content.Append(GetContact(item));
            content.Append(GetNotes(item));
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
                        font-size: 14pt;
                        font-family: Verdana, Arial, Helvetica, sans-serif;
                        line-height: 1.5em;                
                        padding: 10px;
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
                        vertical-align: top;
                        width: 50%;
                    }                    
                    
                    div#header td#document_name 
                    {                
                        text-align: center;                
                    }
                    
                    div#header td#document_name span.title 
                    {                
                        font-size: 1.5em;               
                    }
            
                    div#header td#document_name span.date
                    {
                        font-size: 1.1em;
                        margin-top: 10px;   
                    }
                    
                    div#header td#hq_location 
                    {                
                        text-align: right;
                    }
                    
                    span.commodity_name
                    {
                        font-size: 1.2em;
                        display: block;
                        font-weight: bold;
                        margin-top: 30px;
                    }      
            
                    span.commodity_name span.unit
                    {
                        font-weight: normal;
                    }

                    div.commodity_details
                    {
                        padding: 10px;
                        margin-bottom: 30px;
                    }

                    div.commodity_details span
                    {
                        display: block;
                        margin-bottom: 2px;
                    }

                    div.contact
                    {
                        margin-bottom: 40px;
                    }

                    div.contact span
                    {
                        display: block;
                        margin-bottom: 2px;
                    }

                    div.contact span.company
                    {
                        font-weight: bold;
                    }

                    span.section_title
                    {
                        display: block;
                        font-weight: bold;
                    }

                </style>
                <style type=""text/css"" media=""print"">
                    body
                    {
                        font-size: 24pt;
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

            return content;
        }

        private string GetHeader(StorageItem item)
        {
            const string html = @"
                <div id=""header"">
                    <table>
                        <tr>
                            <td id=""logo"">
                                <span class=""logo""><img src=""{0}"" alt=""Singer""></span>                        
                            </td>
                            <td id=""document_name"">
                                <span class=""title"">Storage #{1}</span>
                                <span class=""date"">{2}</span>
                            </td>
                        </tr>
                    </table>                        
                </div>
            ";

            var replacements = new string[3];

            replacements[0] = GetHeaderImg();
            replacements[1] = item.Number.ToString();
            replacements[2] = (item.DateEntered != null) ? item.DateEntered.Value.ToString(SingerConfigs.PrintedDateFormatString) : "";

            return string.Format(html, replacements);
        }

        private string GetCommodity(StorageItem item)
        {
            const string html = @"
                <span class=""commodity_name"">{0} <span class=""unit"">{1}</span></span>
                
                <div class=""commodity_details"">
                    <span class=""dimensions"">{2}</span>
                    <span class=""weight"">{3}</span>
                </div>
            ";

            if (item.JobCommodity == null) return "";

            string distance, weight;

            if (PrintMetric != true)
            {
                distance = MeasurementFormater.UFeet;
                weight = MeasurementFormater.UPounds;
            }
            else
            {
                distance = MeasurementFormater.UMetres;
                weight = MeasurementFormater.UKilograms;
            }

            var replacements = new string[4];

            replacements[0] = item.JobCommodity.Name;
            replacements[1] = string.Format("[{0}]", item.JobCommodity.Unit);
            replacements[2] = string.Format("{0} x {1} x {2} (LxWxH)", MeasurementFormater.FromMetres(item.JobCommodity.Length, distance), MeasurementFormater.FromMetres(item.JobCommodity.Width, distance), MeasurementFormater.FromMetres(item.JobCommodity.Height, distance));
            replacements[3] = string.Format("{0}", MeasurementFormater.FromKilograms(item.JobCommodity.Weight, weight));

            return string.Format(html, replacements);
        }

        private string GetContact(StorageItem item)
        {
            const string html = @"
                <div class=""contact"">
                    <span class=""company"">{0}</span>
                    <span class=""name"">{1}</span>
                    <span class=""email"">{2}</span>
                    <span class=""phone"">{3}</span>
                    <span class=""phone"">{4}</span>
                    <span class=""fax"">{5}</span>
                </div>
            ";

            if (item.Contact == null) return "";

            var replacements = new string[6];

            replacements[0] = item.Contact.Company.Name;
            replacements[1] = item.Contact.Name;
            replacements[2] = (string.IsNullOrWhiteSpace(item.Contact.Email)) ? "" : item.Contact.Email;
            replacements[3] = (string.IsNullOrWhiteSpace(item.Contact.PrimaryPhone)) ? "" : item.Contact.PrimaryPhone;
            replacements[4] = (string.IsNullOrWhiteSpace(item.Contact.SecondaryPhone)) ? "" : item.Contact.SecondaryPhone;
            replacements[5] = (string.IsNullOrWhiteSpace(item.Contact.Fax)) ? "" : item.Contact.Fax;

            return string.Format(html, replacements);
        }

        private string GetNotes(StorageItem item)
        {
            const string html = @"
                <span class=""section_title"">Notes:</span>

                <div class=""notes"">
                    <p>{0}</p>
                </div>
            ";

            return string.Format(html, item.Notes);
        }

    }
}
