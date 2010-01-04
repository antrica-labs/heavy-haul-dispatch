using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SingerDispatch.Database;

namespace SingerDispatch.Panels.Jobs
{
    /// <summary>
    /// Interaction logic for JobsPanel.xaml
    /// </summary>
    public partial class JobsPanel : JobUserControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public JobsPanel()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            IsEnabled = newValue != null;
            SelectedJob = null;
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);
        }

        private void CommitJobChanges_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob == null) return;

            if (SelectedJob.ID == 0)
            {
                EntityHelper.SaveAsNewJob(SelectedJob, Database);
            }
            else
            {
                Database.SubmitChanges();
            }
        }
    }
}
