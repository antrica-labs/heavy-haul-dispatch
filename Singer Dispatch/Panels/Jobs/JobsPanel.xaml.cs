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
        public SingerDispatchDataContext Database { get; set; }

        private CommandBinding SaveCommand { get; set; }
        private Status DefaultJobStatus { get; set; }
        
        public JobsPanel()
        {
            InitializeComponent();

            SaveCommand = new CommandBinding(CustomCommands.GenericSaveCommand);
            CommandBindings.Add(SaveCommand);

            if (InDesignMode()) return;
            // Work below can only be done when the real app is running. It fails during design time.

            Database = SingerConfigs.CommonDataContext;
            DefaultJobStatus = (from s in Database.Statuses where s.Name == "Pending" select s).First();
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {            
            SaveCommand.Executed += CommitJobChanges_Executed;

            if (InDesignMode()) return;

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

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            IsEnabled = newValue != null;
            SelectedJob = null;
            Tabs.SelectedIndex = 0;

            UpdateJobList();
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);

            dgJobs.SelectedItem = newValue;
        }

        private void CommitJobChanges_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob == null) return;

            var button = (ButtonBase)sender;

            try
            {
                Database.SubmitChanges();

                
                button.Focus();
                button.IsEnabled = false;
            }
            catch (System.Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void CommitChangesButton_LostFocus(object sender, RoutedEventArgs e)
        {
            var button = (ButtonBase)sender;

            button.IsEnabled = true;
        }

        private void CommitJobChanges_Executed(object sender, ExecutedRoutedEventArgs e)
        {            
            CommitChangesButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, CommitChangesButton));
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
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
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
                Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void dgJobs_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var grid = (DataGrid)sender;

            if (grid.SelectedItem == null) return;

            grid.ScrollIntoView(grid.SelectedItem);
            grid.UpdateLayout();

            SelectedJob = (Job)grid.SelectedItem;
        }

        private void ViewQuote_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob == null || SelectedJob.Quote == null) return;

            var window = (MainWindow)Application.Current.MainWindow;
            window.ViewQuote(SelectedJob.Quote);
        }

        /*
        private void ViewDispatches_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob == null) return;

            var dispatches = (from d in SelectedJob.Dispatches select d).ToList();

            bool printFileCopy;

            if (SelectedCompany.CustomerType.IsEnterprise == true)
            {
                printFileCopy = Convert.ToBoolean(SingerConfigs.GetConfig("Dispatch-EnterprisePrintFileCopy") ?? "false");                               
            }
            else
            {
                printFileCopy = Convert.ToBoolean(SingerConfigs.GetConfig("Dispatch-SingerPrintFileCopy") ?? "false");                               
            }

            var viewer = new DocumentViewerWindow(new DispatchDocument(printFileCopy), dispatches, string.Format("Dispatches - Job #{0}", SelectedJob.Number)) { IsMetric = !UseImperialMeasurements, IsSpecializedDocument = SelectedCompany.CustomerType.IsEnterprise != true };
            viewer.DisplayPrintout();
        }
        */
    }
}
