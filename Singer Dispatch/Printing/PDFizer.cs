using System.IO;

namespace SingerDispatch.Printing
{
    class PDFizer
    {
        private static string PDF_COMMAND = @"PDF\wkhtmltopdf.exe";
        private static string PDF_ARGS = @"-s Letter ""%HTML_FILE%"" ""%PDF_FILE%""";

        public void SaveHTMLToPDF(string html, string filename)
        {
            var tmp = ConvertHTMLToTempPDF(html);

            FileUtils.MoveFile(tmp, filename);
        }

        private string ConvertHTMLToTempPDF(string html)
        {   
            var htmlFile = WriteHTMLToTempFile(html);
            var pdfFile = FileUtils.CreateTempFile("pdf");

            var command = PDF_COMMAND;
            var arguments = PDF_ARGS.Replace("%HTML_FILE%", htmlFile).Replace("%PDF_FILE%", pdfFile);

            ExecuteCommandSync(command, arguments);

            FileUtils.DeleteFile(htmlFile);

            return pdfFile;
        }

        private string WriteHTMLToTempFile(string html)
        {
            var file = FileUtils.CreateTempFile("html");
            var writer = new StreamWriter(file);

            writer.Write(html);
            writer.Close();

            return file;
        }

        private void ExecuteCommandSync(string command, string arguments)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();

            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

            process.Start();

            string output = process.StandardOutput.ReadToEnd();

            //Wait for process to finish
            process.WaitForExit();
        }
    }
}
