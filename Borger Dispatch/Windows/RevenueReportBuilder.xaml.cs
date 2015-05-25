using SingerDispatch.Printing.Documents;
using SingerDispatch.Printing.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SingerDispatch.Windows
{
    /// <summary>
    /// Interaction logic for RevenueReportBuilder.xaml
    /// </summary>
    public partial class RevenueReportBuilder : Window
    {
        private BackgroundWorker Backgrounder;

        private DateTime StartDate;
        private DateTime EndDate;

        public RevenueReportBuilder()
        {
            InitializeComponent();

            Backgrounder = new BackgroundWorker();
        }

        public void BuildReport(DateTime start, DateTime end)
        {
            StartDate = start;
            EndDate = end;

            Backgrounder.DoWork += BuildDocument;
            Backgrounder.RunWorkerCompleted += DocumentCreationComplete;

            Backgrounder.RunWorkerAsync();

            ShowDialog();
        }

        private void BuildDocument(object sender, DoWorkEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(UpdateStatus), "Collecting records...");

            var database = new SingerDispatchDataContext();

            // Get all the expected revenue in selected date range...
            var invoices = from i in database.Invoices where i.InvoiceDate >= StartDate && i.InvoiceDate <= EndDate 
                           orderby i.Company.Name, i.InvoiceDate, i.Number, i.Revision
                           select i;

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(UpdateStatus), "Displaying report...");

            var details = new RevenueReportDetails() { StartDate = StartDate, EndDate = EndDate, Invoices = invoices.ToList() };
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action<RevenueReportDetails>(DisplayDocument), details);
        }

        private void DocumentCreationComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            Close();
        }

        private void UpdateStatus(string status)
        {
            lblStatusOutput.Content = status;
        }

        private void DisplayDocument(RevenueReportDetails details)
        {
            var title = string.Format("Revenue Report - {0} to {1}", details.StartDate.ToString(SingerConfigs.PrintedDateFormatString), details.EndDate.ToString(SingerConfigs.PrintedDateFormatString));
            var viewer = new DocumentViewerWindow(new RevenueReportDocument(), new RevenueReportExcel(), details, title) { Owner = Application.Current.MainWindow };
            viewer.DisplayPrintout();
        }
    }
}
