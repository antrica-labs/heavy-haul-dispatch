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

namespace SingerDispatch.Windows
{
    /// <summary>
    /// Interaction logic for InvoiceBuilderWindow.xaml
    /// </summary>
    public partial class InvoiceBuilderWindow : Window
    {
        private Company Company;
        private SingerDispatchDataContext Database;

        private Invoice Invoice;        

        public InvoiceBuilderWindow(Company company, SingerDispatchDataContext database)
        {
            InitializeComponent();

            Company = company;
            Database = database;

            Invoice = null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var jobs = from j in Database.Jobs where j.Company == Company && (j.Status != null && j.Status.Name != "Billed") orderby j.Number select j;
            cmbJobList.ItemsSource = jobs;
        }

        private void SelectNone_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var grid = (DataGrid)button.DataContext;

            grid.SelectedIndex = -1;
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var grid = (DataGrid)button.DataContext;

            grid.SelectAll();
        }

        private void cmbJobList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDataGrids();
        }


        private void acJobNumber_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateDataGrids();
        }

        private void UpdateDataGrids()
        {
            var job = (Job)cmbJobList.SelectedItem;

            if (job != null)
            {
                var loads = from l in Database.Loads where l.Job == job && (l.Status != null && l.Status.Name != "Billed") orderby l.Number select l;
                var services = from t in Database.ThirdPartyServices where t.Load.Job == job && t.IsBilled != true orderby t.Load.Number select t;
                var storage = from s in Database.StorageItems where s.Job == job orderby s.JobCommodity.Owner.Name select s; 

                dgLoads.ItemsSource = loads.ToList();
                dgServices.ItemsSource = services.ToList();
                dgStorage.ItemsSource = storage.ToList();
            }
            else
                dgLoads.ItemsSource = dgServices.ItemsSource = dgStorage.ItemsSource = null;
        }
        
        public Invoice CreateInvoice()
        {
            Invoice = null;

            ShowDialog();

            return Invoice;
        }

        private void CreateInvoice_Click(object sender, RoutedEventArgs e)
        {
            var job = (Job)cmbJobList.SelectedItem;

            if (job != null)
            {
                Status status;

                try
                {
                    status = (from s in Database.Statuses where s.Name == "Billed" select s).First();
                }
                catch
                {
                    status = null;
                }

                var loads = (System.Collections.IList)dgLoads.SelectedItems;
                var services = (System.Collections.IList)dgServices.SelectedItems;
                var storage = (System.Collections.IList)dgStorage.SelectedItems;

                Invoice = CreateDetailedInvoice(job, loads.Cast<Load>(), services.Cast<ThirdPartyService>(), storage.Cast<StorageItem>(), status);
            }
            else
                Invoice = new Invoice();

            Close();
        }

        private static Invoice CreateDetailedInvoice(Job job, IEnumerable<Load> loads, IEnumerable<ThirdPartyService> services, IEnumerable<StorageItem> storage, Status billedStatus)
        {
            var invoice = new Invoice();
            
            invoice.AddLoads(loads, billedStatus);            
            invoice.AddThirdPartyServices(services);
            invoice.AddStorageItems(storage);

            invoice.Job = job;
            
            return invoice;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {          
        }

        
    }
}
