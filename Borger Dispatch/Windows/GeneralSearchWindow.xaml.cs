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

        private void RunJobsSearch_Click(object sender, RoutedEventArgs e)
        {
            var terms = txtSearchTerm.Text;
            var company = txtCompany.Text;
            var start = startDate.SelectedDate;
            var end = endDate.SelectedDate;

            FindJobs(terms, company, start, end);
            FindLoads(terms, company, start, end);
            FindDispatches(terms, company, start, end);
        }

        private void RunQuotesSearch_Click(object sender, RoutedEventArgs e)
        {
            var terms = txtSearchTerm.Text;
            var company = txtCompany.Text;
            var start = startDate.SelectedDate;
            var end = endDate.SelectedDate;

            FindQuotes(terms, company, start, end);
        }

        private void RunInvoicesSearch_Click(object sender, RoutedEventArgs e)
        {
            var terms = txtSearchTerm.Text;
            var company = txtCompany.Text;
            var start = startDate.SelectedDate;
            var end = endDate.SelectedDate;

            FindInvoices(terms, company, start, end);
        }

        private void RunServicesSearch_Click(object sender, RoutedEventArgs e)
        {
            var terms = txtSearchTerm.Text;
            var company = txtCompany.Text;
            var start = startDate.SelectedDate;
            var end = endDate.SelectedDate;

            FindThirdPartyServices(terms, company, start, end);
        }

        private void RunPermitsSearch_Click(object sender, RoutedEventArgs e)
        {
            var terms = txtSearchTerm.Text;
            var company = txtCompany.Text;
            var start = startDate.SelectedDate;
            var end = endDate.SelectedDate;

            FindPermits(terms, company, start, end);
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
            var database = SingerConfigs.CommonDataContext;
            var jobs = from j in database.Jobs where j.Company != null select j;

            if (!string.IsNullOrWhiteSpace(term))
            {
                jobs = (from j in jobs where j.Name != null && j.Name.ToUpper().Contains(term.ToUpper()) select j).Concat(from j in jobs join r in database.JobReferenceNumbers on j.ID equals r.JobID where r.Value != null && r.Value.ToUpper().Contains(term.ToUpper()) select j);
            }

            if (!string.IsNullOrWhiteSpace(company))
            {
                jobs = from j in jobs where j.Company.Name.ToUpper().Contains(company.ToUpper()) || (j.CareOfCompany != null && j.CareOfCompany.Name.ToUpper().Contains(company.ToUpper())) select j;
            }

            dgJobs.ItemsSource = jobs;
        }

        private void FindLoads(string term, string company, DateTime? start, DateTime? end)
        {
            var database = SingerConfigs.CommonDataContext;
            var loads = from l in database.Loads where l.Job != null select l;

            if (!string.IsNullOrWhiteSpace(term))
            {
                term = term.ToUpper();

                var others = from l in loads join r in database.LoadReferenceNumbers on l.ID equals r.LoadID where r.Value != null && r.Value.ToUpper().Contains(term) select l;

                loads = from l in loads where 
                            l.TrailerCombination != null && l.TrailerCombination.Combination.ToUpper().Contains(term) ||
                            l.Equipment != null && l.Equipment.UnitNumber.ToUpper().Contains(term) ||
                            l.Rate != null && l.Rate.Name.ToUpper().Contains(term)
                        select l;
                
                loads = loads.Concat(others);
            }

            if (!string.IsNullOrWhiteSpace(company))
            {
                loads = from l in loads where l.Job.Company.Name.ToUpper().Contains(company.ToUpper()) || (l.Job.CareOfCompany != null && l.Job.CareOfCompany.Name.ToUpper().Contains(company.ToUpper())) select l;
            }

            dgLoads.ItemsSource = loads.ToList();
        }

        private void FindDispatches(string term, string company, DateTime? start, DateTime? end)
        {
            var database = SingerConfigs.CommonDataContext;
            var dispatches = from d in database.Dispatches where d.Load != null && d.Load.Job != null select d;

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

            dgDispatches.ItemsSource = dispatches.ToList();
        }

        private void FindQuotes(string term, string company, DateTime? start, DateTime? end)
        {
            var database = SingerConfigs.CommonDataContext;
            var quotes = from q in database.Quotes where q.Company != null select q;

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

            dgQuotes.ItemsSource = quotes.ToList();
        }

        private void FindInvoices(string term, string company, DateTime? start, DateTime? end)
        {
            var database = SingerConfigs.CommonDataContext;
            var invoices = from i in database.Invoices where i.Company != null select i;

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

            dgInvoices.ItemsSource = invoices.ToList();
        }

        private void FindThirdPartyServices(string term, string company, DateTime? start, DateTime? end)
        {
            var database = SingerConfigs.CommonDataContext;
            var services = from s in database.ThirdPartyServices where s.Load != null && s.Load.Job != null select s;

            if (!string.IsNullOrWhiteSpace(term))
            {
                services = from s in services where (s.Reference != null && s.Reference.ToUpper().Contains(term.ToUpper())) || (s.Location != null && s.Location.ToUpper().Contains(term.ToUpper())) select s;
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

            dgServices.ItemsSource = services.ToList();
        }

        private void FindPermits(string term, string company, DateTime? start, DateTime? end)
        {
            var database = SingerConfigs.CommonDataContext;
            var permits = from p in database.Permits where p.Load != null && p.Load.Job != null select p;

            if (!string.IsNullOrWhiteSpace(term))
            {
                permits = from p in permits where p.Reference != null && p.Reference.ToUpper().Contains(term.ToUpper()) select p;
            }

            if (!string.IsNullOrWhiteSpace(company))
            {
                permits = from p in permits where p.IssuingCompany != null && p.IssuingCompany.Name.ToUpper().Contains(company.ToUpper()) select p;                          
            }

            dgPermits.ItemsSource = permits.ToList();
        }
    }
}
