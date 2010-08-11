using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using SingerDispatch.Controls;
using System.Windows.Input;
using SingerDispatch.Windows;
using SingerDispatch.Printing.Documents;

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

            Database = SingerConstants.CommonDataContext;

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

        private void ViewDispatches_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob == null) return;

            var dispatches = (from d in SelectedJob.Dispatches select d).ToList();
            
            var result = MessageBox.Show("Do you wish to inlcude a file copy with this printout?", "Include drivers copy?", MessageBoxButton.YesNo, MessageBoxImage.Question);

            var viewer = new DocumentViewerWindow(new DispatchDocument(result == MessageBoxResult.Yes), dispatches, string.Format("Dispatches - Job #{0}", SelectedJob.Number)) { IsMetric = !UseImperialMeasurements };
            viewer.DisplayPrintout();
        }

        
    }
}
