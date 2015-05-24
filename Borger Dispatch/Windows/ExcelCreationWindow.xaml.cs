using System;
using System.Diagnostics;
using SingerDispatch.Printing;
using System.ComponentModel;
using System.Windows.Threading;
using ClosedXML.Excel;

namespace SingerDispatch.Windows
{
    /// <summary>
    /// Interaction logic for PdfCreationWindow.xaml
    /// </summary>
    public partial class ExcelCreationWindow
    {
        private BackgroundWorker Backgrounder;

        private readonly string Filename;
        private readonly XLWorkbook Excel;

        public ExcelCreationWindow(string filename, XLWorkbook excel)
        {
            InitializeComponent();

            Backgrounder = new BackgroundWorker();

            Filename = filename;
            Excel = excel;
        }

        public void Run()
        {
            Show();

            Backgrounder.DoWork += CreateExcel;
            Backgrounder.RunWorkerCompleted += CompletedExcelCreation;

            Backgrounder.RunWorkerAsync();
        }

        private void CreateExcel(object sender, DoWorkEventArgs e)
        {           

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(UpdateStatus), "Generating Excel output...");

            Excel.SaveAs(Filename);

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(UpdateStatus), "Excel creation complete, opening file...");

            // Open the file in an external PDF viewer
            var process = new Process();
            var shell = new ProcessStartInfo(Filename);

            shell.UseShellExecute = true;
            shell.WindowStyle = ProcessWindowStyle.Normal;

            process.StartInfo = shell;
            process.Start();
        }

        private void CompletedExcelCreation(object sender, RunWorkerCompletedEventArgs e)
        {
            Close();
        }

        private void UpdateStatus(string status)
        {
            lblStatusOutput.Content = status;
        }
    }
}
