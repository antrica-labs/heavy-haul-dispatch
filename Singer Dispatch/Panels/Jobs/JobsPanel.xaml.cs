using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using SingerDispatch.Controls;
using System.Windows.Input;
using SingerDispatch.Windows;
using SingerDispatch.Printing.Documents;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using SingerDispatch.Database;

namespace SingerDispatch.Panels.Jobs
{
    /// <summary>
    /// Interaction logic for JobsPanel.xaml
    /// </summary>
    public partial class JobsPanel
    {   
        private Status DefaultJobStatus { get; set; }
        
        public JobsPanel()
        {
            InitializeComponent();

            if (InDesignMode()) return;
            // Work below can only be done when the real app is running. It fails during design time.

            Database = SingerConfigs.CommonDataContext;
            DefaultJobStatus = (from s in Database.Statuses where s.Name == "Pending" select s).First();
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode() || IsVisible == false) return;

            UpdateJobList();
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

        protected override void CompanyListChanged(ObservableCollection<Company> newValue, ObservableCollection<Company> oldValue)
        {
            base.CompanyListChanged(newValue, oldValue);
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);
                        
            SelectedJob = null;
            Tabs.SelectedIndex = 0;
            
            UpdateJobList();
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);
        }

        private void NewJob_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Database.SubmitChanges();
            }
            catch (Exception ex)
            {
                Windows.NoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
                Database.RevertChanges();
            }

            var window = new NewJobNumberWindow() { Owner = Application.Current.MainWindow };
            var jobNumber = window.CreateJobNumber();

            if (jobNumber == null)
                return;

            var list = (ObservableCollection<Job>)dgJobs.ItemsSource;
            var job = new Job { Number = jobNumber, Status = DefaultJobStatus, Company = SelectedCompany };
                        
            list.Insert(0, job);
            SelectedCompany.Jobs.Add(job);

            SelectedJob = job;
            
            try
            {
                Database.SubmitChanges();
            }
            catch (Exception ex)
            {
                Windows.NoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
                Database.RevertChanges();
            }
        }

        private void DuplicateJob_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob == null) return;

            try
            {
                Database.SubmitChanges();
            }
            catch (Exception ex)
            {
                Windows.NoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
                Database.RevertChanges();
            }

            var window = new NewJobNumberWindow() { Owner = Application.Current.MainWindow };

            var list = (ObservableCollection<Job>)dgJobs.ItemsSource;
            var job = SelectedJob.Duplicate();

            job.Number = window.CreateJobNumber();

            if (job.Number != null)
            {
                list.Insert(0, job);
                SelectedCompany.Jobs.Add(job);

                SelectedJob = job;
            }
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
                Windows.NoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
                Database.RevertChanges();
            }
        }

        private void dgJobs_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var grid = (DataGrid)sender;

            if (grid.SelectedItem == null) return;

            grid.ScrollIntoView(grid.SelectedItem);
            grid.UpdateLayout();
        }

        private void ViewQuote_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob == null || SelectedJob.Quote == null) return;

            var window = (MainWindow)Application.Current.MainWindow;
            window.ViewQuote(SelectedJob.Quote);
        }

        private void ViewLoads_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob == null) return;

            var window = (MainWindow)Application.Current.MainWindow;
            window.ViewLoads(SelectedJob);
        }     
    }
}
