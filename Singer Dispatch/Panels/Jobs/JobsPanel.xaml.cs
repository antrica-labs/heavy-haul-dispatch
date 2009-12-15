using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SingerDispatch.Panels.Jobs
{
    /// <summary>
    /// Interaction logic for JobsPanel.xaml
    /// </summary>
    public partial class JobsPanel : JobUserControl
    {
        public JobsPanel()
        {
            InitializeComponent();
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

        private void btnCommitChanges_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob == null)
            {
                return;
            }
            
            if (SelectedJob.ID == 0)
            {
                try
                {
                    SelectedJob.Number = (from j in SingerConstants.CommonDataContext.Jobs select j.Number).Max() + 1;
                }
                catch
                {
                    SelectedJob.Number = 1;
                }

                SingerConstants.CommonDataContext.Jobs.InsertOnSubmit(SelectedJob);
            }

            SingerConstants.CommonDataContext.SubmitChanges();
        }
    }
}
