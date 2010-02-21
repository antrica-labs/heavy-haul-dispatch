using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections.ObjectModel;
using SingerDispatch.Database;

namespace SingerDispatch.Panels.Jobs
{
    /// <summary>
    /// Interaction logic for JobHistoryControl.xaml
    /// </summary>
    public partial class JobHistoryControl : JobUserControl
    {
        public SingerDispatchDataContext Database { get; set; }
        public JobStatusType DefaultJobStatus { get; set; }

        public JobHistoryControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;

            DefaultJobStatus = (JobStatusType)(from s in Database.JobStatusTypes where s.Name == "Pending" select s).First();
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            cmbCreatedBy.ItemsSource = from emp in Database.Employees select emp;
            cmbStausTypes.ItemsSource = from s in Database.JobStatusTypes select s;

            cmbQuotes.ItemsSource = (SelectedCompany == null) ? null : from q in Database.Quotes where q.Company == SelectedCompany select q;
            cmbCareOfCompanies.ItemsSource = (SelectedCompany == null) ? null : from c in Database.Companies where c != SelectedCompany && c.IsVisible == true select c;

            if (SelectedCompany != null)
            {
                var jobs = new ObservableCollection<Job>(from j in Database.Jobs where j.Company == SelectedCompany orderby j.Number descending select j);

                if (SelectedJob != null && SelectedJob.ID == 0)
                    jobs.Insert(0, SelectedJob);

                dgJobs.ItemsSource = jobs;
            }
            else
                dgJobs.ItemsSource = null;

        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            cmbQuotes.ItemsSource = (SelectedCompany == null) ? null : from q in Database.Quotes where q.Company == SelectedCompany select q;
            cmbCareOfCompanies.ItemsSource = (SelectedCompany == null) ? null : from c in Database.Companies where c != SelectedCompany && c.IsVisible == true select c;
            dgJobs.ItemsSource = (SelectedCompany == null) ? null : new ObservableCollection<Job>(from j in Database.Jobs where j.Company == SelectedCompany orderby j.Number descending select j);
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);
                     
            UpdateContactList();
        }
     

        private void cmbCareOfCompanies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {           
            UpdateContactList();
        }

        private void UpdateContactList()
        {
            List<Contact> contacts;

            if (SelectedJob == null)
            {
                contacts = new List<Contact>();
            }
            else if (SelectedJob.CareOfCompanyID != null)
            {
                contacts = (from c in Database.Contacts where c.Address.CompanyID == SelectedCompany.ID || c.Address.CompanyID == SelectedJob.CareOfCompanyID select c).ToList();
            }
            else
            {
                contacts = (from c in Database.Contacts where c.Address.CompanyID == SelectedCompany.ID select c).ToList();
            }

            dgJobContacts.ItemsSource = null;
            dgJobContacts.UpdateLayout();
            dgJobContacts.MaxHeight = dgJobContacts.ActualHeight;
            dgJobContacts.ItemsSource = contacts;
        }

        private void NewJob_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<Job>)dgJobs.ItemsSource;
            var job = new Job { JobStatusType = DefaultJobStatus, Company = SelectedCompany };

            list.Insert(0, job);
            dgJobs.SelectedItem = job;
            dgJobs.ScrollIntoView(job);

            txtDescription.Focus();
        }
               
        private void ViewQuote_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob == null || SelectedJob.Quote == null)
            {
                return;
            }

            var window = (MainWindow)Application.Current.MainWindow;
            window.ViewQuote(SelectedJob.Quote);
        }

        private void DeleteJob_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob == null)
            {
                return;
            }

            var job = SelectedJob;
                        
            try
            {
                EntityHelper.PrepareEntityDelete(job, Database);

                ((ObservableCollection<Job>)dgJobs.ItemsSource).Remove(job);
                SelectedCompany.Jobs.Remove(job);

                Database.SubmitChanges();
            }
            catch (System.Exception ex)
            {
                SingerDispatch.Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }
    }
}
