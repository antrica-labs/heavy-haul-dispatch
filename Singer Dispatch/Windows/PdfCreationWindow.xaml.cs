using System;
using System.Diagnostics;
using SingerDispatch.Printing;
using System.ComponentModel;
using System.Windows.Threading;

namespace SingerDispatch.Windows
{
    /// <summary>
    /// Interaction logic for PdfCreationWindow.xaml
    /// </summary>
    public partial class PdfCreationWindow
    {
        private BackgroundWorker Backgrounder;

        private readonly string Filename;
        private readonly string HTML;

        public PdfCreationWindow(string filename, string html)
        {
            InitializeComponent();

            Backgrounder = new BackgroundWorker();

            Filename = filename;
            HTML = html;
        }

        public void Run()
        {
            Show();

            Backgrounder.DoWork += CreatePDF;
            Backgrounder.RunWorkerCompleted += CompletedPDFCreation;

            Backgrounder.RunWorkerAsync();
        }

        private void CreatePDF(object sender, DoWorkEventArgs e)
        {
            var pdf = new PDFizer();

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(UpdateStatus), "Creating PDF...");
                
            pdf.SaveHTMLToPDF(HTML, Filename);

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(UpdateStatus), "PDF creation complete, opening PDF viewer...");

            // Open the file in an external PDF viewer
            var process = new Process();
            var shell = new ProcessStartInfo(Filename);

            shell.UseShellExecute = true;
            shell.WindowStyle = ProcessWindowStyle.Normal;

            process.StartInfo = shell;
            process.Start();
        }

        private void CompletedPDFCreation(object sender, RunWorkerCompletedEventArgs e)
        {
            Close();
        }

        private void UpdateStatus(string status)
        {
            lblStatusOutput.Content = status;
        }
    }
}
