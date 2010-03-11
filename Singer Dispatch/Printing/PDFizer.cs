using System.IO;

namespace SingerDispatch.Printing
{
    class PDFizer
    {
        private const string DEFAULT_PDF_COMMAND = @"wkhtmltopdf.exe";
        private const string DEFAULT_PDF_ARGS = @"-s Letter ""%HTML_FILE%"" ""%PDF_FILE%""";

        private string PdfCommand { get; set; }
        private string PdfArgs { get; set; }

        public PDFizer()
        {
            PdfCommand = SingerConstants.GetConfig("PDF-ExecutablePath") ?? DEFAULT_PDF_COMMAND;
            PdfArgs = SingerConstants.GetConfig("PDF-Arguments") ?? DEFAULT_PDF_ARGS;
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

        private static void ExecuteCommandSync(string command, string arguments)
        {
            var process = new System.Diagnostics.Process();

            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

            process.Start();

            process.StandardOutput.ReadToEnd();

            //Wait for process to finish
            process.WaitForExit();
        }
    }
}
