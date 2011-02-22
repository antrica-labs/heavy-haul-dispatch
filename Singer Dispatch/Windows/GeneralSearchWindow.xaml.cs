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
    /// Interaction logic for GeneralSearchWindow.xaml
    /// </summary>
    public partial class GeneralSearchWindow : Window
    {        
        private object SelectedEntity;

        public GeneralSearchWindow()
        {
            InitializeComponent();

            SelectedEntity = null;
        }

        public object FindSomething()
        {
            ShowDialog();

            return SelectedEntity;
        }

        private void RunSearch_Click(object sender, RoutedEventArgs e)
        {
            var terms = txtSearchTerm.Text;
            var company = txtCompany.Text;
            var start = startDate.SelectedDate;
            var end = endDate.SelectedDate;

            FindJobs(terms, company, start, end);
        }        

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                SelectedEntity = null;
                Close();
            }
        }

        private void SearchItem_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var grid = (DataGrid)sender;

            SelectedEntity = grid.SelectedItem;

            Close();
        }

        private void FindJobs(string term, string company, DateTime? start, DateTime? end)
        {
            var database = new SingerDispatchDataContext();
            var jobs = from j in database.Jobs select j;

            if (!string.IsNullOrWhiteSpace(term))
            {
                jobs = from j in jobs where j.Name.ToUpper().Contains(term.ToUpper()) select j;
            }

            if (!string.IsNullOrWhiteSpace(company))
            {
                jobs = from j in jobs where j.Company.Name.ToUpper().Contains(company.ToUpper()) || (j.CareOfCompany != null && j.CareOfCompany.Name.ToUpper().Contains(company.ToUpper())) select j;
            }

            dgJobs.ItemsSource = jobs;
        }

        private void FindLoads(string term, string company, DateTime start, DateTime end)
        {
            var database = new SingerDispatchDataContext();
            var loads = from l in database.Loads select l;

            if (!string.IsNullOrWhiteSpace(term))
            {
                //loads = from l in database.Loads where l.
            }

            if (!string.IsNullOrWhiteSpace(company))
            {
                loads = from l in loads where l.Job.Company.Name.ToUpper().Contains(company.ToUpper()) || (l.Job.CareOfCompany != null && l.Job.CareOfCompany.Name.ToUpper().Contains(company.ToUpper())) select l;
            }

            dgLoads.ItemsSource = loads;
        }

        private void FindDispatches(string term, string company, DateTime start, DateTime end)
        {
            var database = new SingerDispatchDataContext();
            var dispatches = from d in database.Dispatches select d;

            if (!string.IsNullOrWhiteSpace(term))
            {
                
            }

            if (!string.IsNullOrWhiteSpace(company))
            {
                dispatches = from d in dispatches where d.Load.Job.Company.Name.ToUpper().Contains(company.ToUpper()) || (d.Load.Job.CareOfCompany != null && d.Load.Job.CareOfCompany.Name.ToUpper().Contains(company.ToUpper())) select d;
            }

            if (start != null)
            {
                dispatches = from d in dispatches where d.MeetingDate >= start select d;
            }

            if (end != null)
            {
                dispatches = from d in dispatches where d.MeetingDate <= end select d;
            }

            dgDispatches.ItemsSource = dispatches;
        }

        private void FindQuotes(string term, string company, DateTime start, DateTime end)
        {
            var database = new SingerDispatchDataContext();
            var quotes = from q in database.Quotes select q;

            if (!string.IsNullOrWhiteSpace(term))
            {
                quotes = from q in quotes where q.Description.ToUpper().Contains(term.ToUpper()) select q;
            }

            if (!string.IsNullOrWhiteSpace(company))
            {
                quotes = from q in quotes where q.Company.Name.ToUpper().Contains(company.ToUpper()) || (q.CareOfCompany != null && q.CareOfCompany.Name.ToUpper().Contains(company.ToUpper())) select q;
            }

            if (start != null)
            {
                quotes = from q in quotes where q.StartDate >= start || q.CreationDate >= start select q;
            }

            if (end != null)
            {
                quotes = from q in quotes where q.StartDate <= end || q.CreationDate <= end select q;
            }
        }

        private void FindInvoices(string term, string company, DateTime start, DateTime end)
        {
            var database = new SingerDispatchDataContext();
            var invoices = from i in database.Invoices select i;

            if (!string.IsNullOrWhiteSpace(term))
            {

            }

            if (!string.IsNullOrWhiteSpace(company))
            {
                invoices = from i in invoices where i.Company.Name.ToUpper().Contains(company.ToUpper()) select i;
            }

            if (start != null)
            {
                invoices = from i in invoices where i.InvoiceDate >= start select i;
            }

            if (end != null)
            {
                invoices = from i in invoices where i.InvoiceDate <= end select i;
            }

            dgInvoices.ItemsSource = invoices;
        }

        private void FindThirdPartyServices(string term, string company, DateTime start, DateTime end)
        {
            var database = new SingerDispatchDataContext();
            var services = from s in database.ThirdPartyServices select s;

            if (!string.IsNullOrWhiteSpace(term))
            {
                services = from s in services where s.Reference != null && s.Reference.ToUpper().Contains(term.ToUpper()) select s;
            }

            if (!string.IsNullOrWhiteSpace(company))
            {
                services = from s in services where s.Company != null && s.Company.Name.ToUpper().Contains(company.ToUpper()) select s;
            }

            if (start != null)
            {
                services = from s in services where s.ServiceDate >= start select s;
            }

            if (end != null)
            {
                services = from s in services where s.ServiceDate <= end select s;
            }

            dgServices.ItemsSource = services;
        }

        private void FindPermits(string term, string company, DateTime start, DateTime end)
        {
            var database = new SingerDispatchDataContext();
            var permits = from p in database.Permits select p;

            if (!string.IsNullOrWhiteSpace(term))
            {
                permits = from p in permits where p.Reference != null && p.Reference.ToUpper().Contains(term.ToUpper()) select p;
            }

            if (!string.IsNullOrWhiteSpace(company))
            {
                permits = from p in permits where p.IssuingCompany != null && p.IssuingCompany.Name.ToUpper().Contains(company.ToUpper()) select p;                          
            }

            dgPermits.ItemsSource = permits;
        }
    }
}
