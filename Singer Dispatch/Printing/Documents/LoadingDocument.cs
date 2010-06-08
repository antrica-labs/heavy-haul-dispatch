using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingerDispatch.Printing.Documents
{
    class LoadingDocument : IPrintDocument
    {        
        public string GenerateHTML(object entity, bool metric)
        {
            var html = @"
            <!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd"">
            <html>
                <head>
                    <meta http-equiv=""Content-Type"" content=""text/html;charset=utf-8"">
            
                    <title>Loading...</title>

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
                            font-size: 10pt;
                            font-family: Verdana, Arial, Helvetica, sans-serif;
                            padding: 100px;
                            text-align: center;
                        }
                    </style>
                </head>
                </body>
                    <img src=""%LOADING_GIF"" alt=""Loading..."" />
                </body>
            </html>
            ";

            var process = System.Diagnostics.Process.GetCurrentProcess();
            var img = "";

            if (process.MainModule != null)
            {
                img = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(process.MainModule.FileName), @"Images\Loading.gif");
            }

            return html.Replace("%LOADING_GIF", img);
        }
    }
}
