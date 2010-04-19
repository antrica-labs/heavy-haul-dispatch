using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using SingerDispatch.Database;
using SingerDispatch.Windows;
using SingerDispatch.Controls;
using System.Windows.Controls.Primitives;

namespace SingerDispatch.Panels.Jobs
{
    /// <summary>
    /// Interaction logic for JobHistoryControl.xaml
    /// </summary>
    public partial class JobHistoryControl
    {
        public SingerDispatchDataContext Database { get; set; }
        public JobStatusType DefaultJobStatus { get; set; }

        public JobHistoryControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;

            DefaultJobStatus = (from s in Database.JobStatusTypes where s.Name == "Pending" select s).First();            
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            CmbCreatedBy.ItemsSource = from emp in Database.Employees orderby emp.FirstName, emp.LastName select emp;
            CmbStausTypes.ItemsSource = from s in Database.JobStatusTypes select s;

            CmbQuotes.ItemsSource = (SelectedCompany == null) ? null : from q in Database.Quotes where q.Company == SelectedCompany select q;
            CmbCareOfCompanies.ItemsSource = (SelectedCompany == null) ? null : from c in Database.Companies where c != SelectedCompany && c.IsVisible == true select c;

            if (SelectedCompany != null)
            {
                var jobs = new ObservableCollection<Job>(from j in Database.Jobs where j.Company == SelectedCompany orderby j.Number descending select j);

                if (SelectedJob != null && SelectedJob.ID == 0)
                    jobs.Insert(0, SelectedJob);

                DgJobs.ItemsSource = jobs;
            }
            else
                DgJobs.ItemsSource = null;

        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            CmbQuotes.ItemsSource = (SelectedCompany == null) ? null : from q in Database.Quotes where q.Company == SelectedCompany select q;
            CmbCareOfCompanies.ItemsSource = (SelectedCompany == null) ? null : from c in Database.Companies where c != SelectedCompany && c.IsVisible == true select c;
            DgJobs.ItemsSource = (SelectedCompany == null) ? null : new ObservableCollection<Job>(from j in Database.Jobs where j.Company == SelectedCompany orderby j.Number descending select j);
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);

            DgReferenceNumbers.ItemsSource = null;
            DgReferenceNumbers.UpdateLayout();
            DgReferenceNumbers.MaxHeight = DgReferenceNumbers.ActualHeight;

            if (newValue != null)
                DgReferenceNumbers.ItemsSource = new ObservableCollection<JobReferenceNumber>(newValue.ReferenceNumbers);

            UpdateContactList();
        }

        private void DgJobs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var grid = (DataGrid)sender;

            if (grid.SelectedItem == null) return;

            grid.ScrollIntoView(grid.SelectedItem);
            grid.UpdateLayout();
        }

        private void CmbCareOfCompanies_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

            DgJobContacts.ItemsSource = null;
            DgJobContacts.UpdateLayout();
            DgJobContacts.MaxHeight = DgJobContacts.ActualHeight;
            DgJobContacts.ItemsSource = contacts;
        }

        private void NewJob_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<Job>)DgJobs.ItemsSource;
            var job = new Job { JobStatusType = DefaultJobStatus, Company = SelectedCompany };

            list.Insert(0, job);
            DgJobs.SelectedItem = job;
            SelectedCompany.Jobs.Add(SelectedJob);

            try
            {
                EntityHelper.SaveAsNewJob(SelectedJob, Database);

                txtName.Focus();
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
            
        }

        private void AddReferenceNumber_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob == null) return;

            var reference = new JobReferenceNumber();

            SelectedJob.ReferenceNumbers.Add(reference);
            ((ObservableCollection<JobReferenceNumber>)DgReferenceNumbers.ItemsSource).Add(reference);

            DataGridHelper.EditFirstColumn(DgReferenceNumbers, reference);
        }

        private void RemoveReferenceNumber_Click(object sender, RoutedEventArgs e)
        {
            var selected = (JobReferenceNumber)DgReferenceNumbers.SelectedItem;

            if (SelectedJob == null || selected == null) return;

            SelectedJob.ReferenceNumbers.Remove(selected);
            ((ObservableCollection<JobReferenceNumber>)DgReferenceNumbers.ItemsSource).Remove(selected);
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
            if (SelectedJob == null) return;

            var confirmation = MessageBox.Show("Are you sure you want to delete this job?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;
                        
            try
            {
                EntityHelper.PrepareEntityDelete(SelectedJob, Database);
                                
                SelectedCompany.Jobs.Remove(SelectedJob);
                Database.SubmitChanges();

                ((ObservableCollection<Job>)DgJobs.ItemsSource).Remove(SelectedJob);
            }
            catch (System.Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        
    }
}
