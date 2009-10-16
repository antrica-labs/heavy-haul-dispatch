using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingerDispatch.Printing
{
    class DispatchRenderer
    {
        public string GeneratePrintout()
        {
            var content = new StringBuilder();

            content.Append(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd""><html>");
            content.Append("<head>");
            content.Append(GetTitle("Singer Specialized - Dispatch"));
            content.Append(GetStyles());
            content.Append("</head>");
            content.Append("<body>");            
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
            string content = @"<style type=""text/css""></styles>";

            return content;
        }
        
    }
}
