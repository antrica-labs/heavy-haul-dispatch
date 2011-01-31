using System;
using System.Collections.Generic;
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
using System.ComponentModel;
using System.Windows.Threading;
using SingerDispatch.Printing.Documents;

namespace SingerDispatch.Windows
{
    /// <summary>
    /// Interaction logic for OutOfProvinceBuilderWindow.xaml
    /// </summary>
    public partial class OutOfProvinceBuilderWindow : Window
    {
        private BackgroundWorker Backgrounder;

        private DateTime StartDate;
        private DateTime EndDate;

        public OutOfProvinceBuilderWindow()
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

            var alberta = (from p in database.ProvincesAndStates where p.Abbreviation == "AB" select p).First();
            var tractorClass = (from ec in database.EquipmentClasses where ec.Name == "Tractor" select ec).First();

            var loads = from c in database.LoadedCommodities where (c.LoadDate.Value.Date >= StartDate.Date || c.LoadDate.Value.Date <= EndDate.Date) && ((c.LoadingProvince == null || c.LoadingProvince != alberta) || (c.UnloadingProvince == null || c.UnloadingProvince != alberta)) select c.Load;
            var dispatches = from d in database.Dispatches where loads.Contains(d.Load) && d.Equipment.EquipmentType.EquipmentClass == tractorClass select d;
            
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(UpdateStatus), "Displaying report...");

            var details = new OPReportDetails() { StartDate = StartDate, EndDate = EndDate, Dispatches = dispatches.ToList() };
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action<OPReportDetails>(DisplayDocument), details);
        }

        private void DocumentCreationComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            Close();
        }

        private void UpdateStatus(string status)
        {
            lblStatusOutput.Content = status;
        }

        private void DisplayDocument(OPReportDetails details)
        {
            var title = string.Format("Out of Province Report - {0} to {1}", details.StartDate.ToString(SingerConfigs.PrintedDateFormatString), details.EndDate.ToString(SingerConfigs.PrintedDateFormatString));
            var viewer = new DocumentViewerWindow(new OutOfProvinceReportDocument(), details, title) { Owner = Application.Current.MainWindow };
            viewer.DisplayPrintout(); 
        }
    }
}
