using System.Windows;
using SingerDispatch.Database;
using SingerDispatch.Controls;
using System.Windows.Input;

namespace SingerDispatch.Panels.Jobs
{
    /// <summary>
    /// Interaction logic for JobsPanel.xaml
    /// </summary>
    public partial class JobsPanel : JobUserControl
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
            SaveCommand.Executed += new ExecutedRoutedEventHandler(CommitJobChanges_Executed);            
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
                if (SelectedJob.ID == 0 || SelectedJob.Number == null)
                {
                    SelectedCompany.Jobs.Add(SelectedJob);
                    SelectedJob.CompanyID = SelectedCompany.ID;
                    EntityHelper.SaveAsNewJob(SelectedJob, Database);
                }
                else
                {
                    Database.SubmitChanges();
                }
            }
            catch (System.Exception ex)
            {
                SingerDispatch.Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void CommitJobChanges_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CommitChangesButton.Focus();
            CommitChangesButton.RaiseEvent(new System.Windows.RoutedEventArgs(System.Windows.Controls.Button.ClickEvent, CommitChangesButton));
        }
    }
}
