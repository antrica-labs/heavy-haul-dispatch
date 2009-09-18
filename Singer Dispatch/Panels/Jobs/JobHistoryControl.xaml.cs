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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace SingerDispatch.Panels.Jobs
{
    /// <summary>
    /// Interaction logic for JobHistoryControl.xaml
    /// </summary>
    public partial class JobHistoryControl : JobUserControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public static DependencyProperty SelectedCompanyProperty = DependencyProperty.Register("SelectedCompany", typeof(Company), typeof(JobHistoryControl), new PropertyMetadata(null, JobHistoryControl.SelectedCompanyPropertyChanged));

        public Company SelectedCompany
        {
            get
            {
                return (Company)GetValue(SelectedCompanyProperty);
            }
            set
            {
                SetValue(SelectedCompanyProperty, value);
            }
        }

        public JobHistoryControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;

            cmbCreatedBy.ItemsSource = (from u in Database.Users select u).ToList();
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            cmbQuotes.ItemsSource = (from q in Database.Quotes where q.Company == SelectedCompany select q).ToList();
        }

        public static void SelectedCompanyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            JobHistoryControl control = (JobHistoryControl)d;

            control.SelectedCompanyChanged((Company)e.NewValue, (Company)e.OldValue);          
        }

        private void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            Company company = newValue;
            
            if (company != null)
            {
                dgJobs.ItemsSource = new ObservableCollection<Job>((from j in Database.Jobs where j.CompanyID == newValue.ID orderby j.EndDate descending select j).ToList());
                cmbCareOfCompanies.ItemsSource = (from c in Database.Companies where c.ID != newValue.ID select c).ToList();
            }
            else
            {
                ((ObservableCollection<Job>)dgJobs.ItemsSource).Clear();
                ((List<Company>)cmbCareOfCompanies.ItemsSource).Clear();
            }

            UpdateContactList();
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);

            UpdateContactList();
            BubbleUpJob(newValue);
        }

        private void BubbleUpJob(Job job)
        {
            // Updated the selected job of any parent controls that may have a dependency property
            FrameworkElement parent = (FrameworkElement)this.Parent;

            while (parent != null && !(parent is JobsPanel))
            {
                parent = (FrameworkElement)parent.Parent;
            }

            if (parent != null)
            {
                JobsPanel panel = (JobsPanel)parent;
                panel.SelectedJob = job;
            }
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

            dgJobContacts.ItemsSource = contacts;
        }

        private void btnNewJob_Click(object sender, RoutedEventArgs e)
        {
            Job job = new Job() { CompanyID = SelectedCompany.ID, Number = 0 };

            ((ObservableCollection<Job>)dgJobs.ItemsSource).Insert(0, job);
            dgJobs.SelectedItem = job;
            txtDescription.Focus();
        }

        private void cmbQuotes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
