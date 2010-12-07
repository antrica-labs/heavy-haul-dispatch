using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingerDispatch.Printing.Documents
{
    public abstract class SingerPrintDocument : IPrintDocument
    {
        public bool PrintMetric { get; set; }
        public bool SpecializedDocument { get; set; }

        public abstract string GenerateHTML(object entity);

        public string GetHeaderImg()
        {
            string img = null;

            if (SpecializedDocument)
                img = SingerConfigs.GetConfig("Documents-SingerHeaderImg") ?? @"Images\SingerHeader.png";
            else
                img = SingerConfigs.GetConfig("Documents-MEHeaderImg") ?? @"Images\MEHeader.png";

            try
            {
                var uri = new System.Uri(img);
                img = uri.ToString();
            }
            catch
            {
                var process = System.Diagnostics.Process.GetCurrentProcess();

                if (process.MainModule != null)
                {
                    var uri = new System.Uri(process.MainModule.FileName);
                    uri = new System.Uri(uri, img);
                    img = uri.ToString();
                }
            }

            return img;
        }
    }
}
