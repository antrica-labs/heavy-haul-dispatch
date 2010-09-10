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
using SingerDispatch.Database;
using SingerDispatch.Windows;
using SingerDispatch.Controls;

namespace SingerDispatch.Panels.Jobs
{
    /// <summary>
    /// Interaction logic for JobListControl.xaml
    /// </summary>
    public partial class JobListControl
    {
        public SingerDispatchDataContext Database { get; set; }

        private Status DefaultJobStatus { get; set; }

        public JobListControl()
        {
            InitializeComponent();
        
            if (InDesignMode()) return;
            // Work below can only be done when the real app is running. It fails during design time.

            Database = SingerConfigs.CommonDataContext;
            DefaultJobStatus = (from s in Database.Statuses where s.Name == "Pending" select s).First();
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode()) return;

            var job = SelectedJob;

            UpdateJobList();
        }

        private void UpdateComboBoxes()
        {
            if (SelectedJob != null)
            {
                cmbCareOfCompanies.ItemsSource = from c in Database.Companies where c != SelectedCompany && c.IsVisible == true select c;
                cmbContacts.ItemsSource = (SelectedCompany == null) ? from c in Database.Contacts where c.Address.CompanyID == SelectedCompany.ID orderby c.FirstName, c.LastName select c : from c in Database.Contacts where c.Address.CompanyID == SelectedCompany.ID || c.Address.CompanyID == SelectedJob.CareOfCompanyID orderby c.FirstName, c.LastName select c;
                cmbStatuses.ItemsSource = from s in Database.Statuses orderby s.Name select s;
            }
            else
            {
                cmbCareOfCompanies.ItemsSource = null;
                cmbContacts.ItemsSource = null;
                cmbStatuses.ItemsSource = null;
            }

        }

        private void UpdateJobList()
        {
            if (SelectedCompany != null)
            {
                dgJobs.ItemsSource = new ObservableCollection<Job>(from j in SelectedCompany.Jobs orderby j.Number descending select j);
            }
            else
            {
                dgJobs.ItemsSource = null;
            }
        }

        private void UpdateReferenceNumbers()
        {
            if (SelectedJob != null)
            {
                dgReferenceNumbers.ItemsSource = new ObservableCollection<JobReferenceNumber>(SelectedJob.ReferenceNumbers);
            }
            else
            {
                dgReferenceNumbers.ItemsSource = null;
            }
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            UpdateJobList();
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);

            dgJobs.SelectedItem = newValue;

            UpdateComboBoxes();
            UpdateReferenceNumbers();
        }

        private void dgJobs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var grid = (DataGrid)sender;

            if (grid.SelectedItem == null) return;

            grid.ScrollIntoView(grid.SelectedItem);
        }

        private void cmbCareOfCompanies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbContacts.ItemsSource = (SelectedCompany == null) ? from c in Database.Contacts where c.Address.CompanyID == SelectedCompany.ID orderby c.FirstName, c.LastName select c : from c in Database.Contacts where c.Address.CompanyID == SelectedCompany.ID || c.Address.CompanyID == SelectedJob.CareOfCompanyID orderby c.FirstName, c.LastName select c;
        }

        private void NewJob_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<Job>)dgJobs.ItemsSource;
            var job = new Job { Status = DefaultJobStatus, Company = SelectedCompany };

            list.Insert(0, job);            
            SelectedCompany.Jobs.Add(job);
            
            try
            {
                EntityHelper.SaveAsNewJob(job, Database);

                SelectedJob = job;
                txtName.Focus();
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void DuplicateJob_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob == null) return;

            var list = (ObservableCollection<Job>)dgJobs.ItemsSource;
            var job = SelectedJob.Duplicate();

            list.Insert(0, job);
            SelectedCompany.Jobs.Add(job);

            try
            {
                EntityHelper.SaveAsNewJob(job, Database);

                SelectedJob = job;
                txtName.Focus();
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void ViewQuote_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob == null || SelectedJob.Quote == null) return;

            var window = (MainWindow)Application.Current.MainWindow;
            window.ViewQuote(SelectedJob.Quote);
        }

        private void DeleteJob_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob == null) return;

            var confirmation = MessageBox.Show("Are you sure you want to delete this job?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            try
            {
                EntityHelper.PrepareEntityDelete(SelectedJob, Database);

                SelectedCompany.Jobs.Remove(SelectedJob);
                Database.SubmitChanges();
                
                // If the job was removed without exception, remove it from the list
                ((ObservableCollection<Job>)dgJobs.ItemsSource).Remove(SelectedJob);                
            }
            catch (System.Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void AddReferenceNumber_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob == null) return;

            var reference = new JobReferenceNumber();

            SelectedJob.ReferenceNumbers.Add(reference);
            ((ObservableCollection<JobReferenceNumber>)dgReferenceNumbers.ItemsSource).Add(reference);

            DataGridHelper.EditFirstColumn(dgReferenceNumbers, reference);
        }

        private void RemoveReferenceNumber_Click(object sender, RoutedEventArgs e)
        {
            var selected = (JobReferenceNumber)dgReferenceNumbers.SelectedItem;

            if (SelectedJob == null || selected == null) return;

            SelectedJob.ReferenceNumbers.Remove(selected);
            ((ObservableCollection<JobReferenceNumber>)dgReferenceNumbers.ItemsSource).Remove(selected);
        }

        private void Control_LayoutUpdated(object sender, EventArgs e)
        {
            dgReferenceNumbers.UpdateLayout();
            dgReferenceNumbers.MaxHeight = dgReferenceNumbers.ActualHeight;
        }

        
    }
}
