using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Threading;

namespace SingerDispatch.Printing
{
    class PDFizer
    {
        private const string DEFAULT_PDF_COMMAND = @"wkhtmltopdf.exe";
        private const string DEFAULT_PDF_ARGS = @"--print-media-type --page-size Letter ""%HTML_FILE%"" ""%PDF_FILE%""";

        private string PdfCommand { get; set; }
        private string PdfArgs { get; set; }
        
        public PDFizer()
        {
            PdfCommand = SingerConfigs.GetConfig("PDF-ExecutablePath") ?? DEFAULT_PDF_COMMAND;
            PdfArgs = SingerConfigs.GetConfig("PDF-Arguments") ?? DEFAULT_PDF_ARGS;
        }
        
        public void SaveHTMLToPDF(string html, string filename)
        {
            var tmp = ConvertHTMLToTempPDF(html);

            FileUtils.MoveFile(tmp, filename);
        }

        private string ConvertHTMLToTempPDF(string html)
        {   
            var htmlFile = WriteHTMLToTempFile(html);
            var pdfFile = FileUtils.CreateTempFile("pdf");

            var command = PdfCommand;
            var arguments = PdfArgs.Replace("%HTML_FILE%", htmlFile).Replace("%PDF_FILE%", pdfFile);

            ExecuteCommandSync(command, arguments);

            FileUtils.DeleteFile(htmlFile);

            return pdfFile;
        }

        private static string WriteHTMLToTempFile(string html)
        {
            var file = FileUtils.CreateTempFile("html");
            var writer = new StreamWriter(file);

            writer.Write(html);
            writer.Close();

            return file;
        }

        private void ExecuteCommandSync(string command, string arguments)
        {
            var process = new Process();

            // Configure the process to run
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = arguments;

            // Don't show the console window
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            
            // Capture console output.
            process.StartInfo.RedirectStandardOutput = true;
            

            process.Start();

            //Wait for process to finish
            process.WaitForExit();
        }
        
    }
}
