using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using SingerDispatch.Controls;
using System.Windows.Input;
using SingerDispatch.Windows;
using SingerDispatch.Printing.Documents;
using System;

namespace SingerDispatch.Panels.Jobs
{
    /// <summary>
    /// Interaction logic for JobsPanel.xaml
    /// </summary>
    public partial class JobsPanel
    {
        private CommandBinding SaveCommand { get; set; }

        public SingerDispatchDataContext Database { get; set; }
        
        public JobsPanel()
        {
            InitializeComponent();

            Database = SingerConfigs.CommonDataContext;

            SaveCommand = new CommandBinding(CustomCommands.GenericSaveCommand);
            CommandBindings.Add(SaveCommand);
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {            
            SaveCommand.Executed += CommitJobChanges_Executed;
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            IsEnabled = newValue != null;
            SelectedJob = null;
            Tabs.SelectedIndex = 0;
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);

            Tabs.SelectedIndex = 0;
        }

        private void CommitJobChanges_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob == null) return;

            try
            {
                Database.SubmitChanges();

                lblSavedStatus.Content = "Saved";
                ((ButtonBase)sender).Focus();
            }
            catch (System.Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void CommitChangesButton_LostFocus(object sender, RoutedEventArgs e)
        {
            lblSavedStatus.Content = "";
        }

        private void CommitJobChanges_Executed(object sender, ExecutedRoutedEventArgs e)
        {            
            CommitChangesButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, CommitChangesButton));
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
