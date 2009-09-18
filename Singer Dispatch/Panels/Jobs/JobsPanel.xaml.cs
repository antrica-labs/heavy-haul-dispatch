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

namespace SingerDispatch.Panels.Jobs
{
    /// <summary>
    /// Interaction logic for JobsPanel.xaml
    /// </summary>
    public partial class JobsPanel : JobUserControl
    {
        public static DependencyProperty SelectedCompanyProperty = DependencyProperty.Register("SelectedCompany", typeof(Company), typeof(JobsPanel), new PropertyMetadata(null, JobsPanel.SelectedCompanyPropertyChanged));
        
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

        public JobsPanel()
        {
            InitializeComponent();
        }

        public static void SelectedCompanyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (JobsPanel)d;
            var company = (Company)e.NewValue;

            control.IsEnabled = company != null;
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);

            bool enable = (newValue != null);

            foreach (TabItem tab in tabs.Items)
            {
                if (tab != masterTab)
                {
                    tab.IsEnabled = enable;
                }
            }
        }

        private void btnCommitChanges_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob == null)
            {
                return;
            }

            MessageBoxResult confirm = MessageBox.Show("Are you sure you wish to commit the changes to this job and all of its properties?", "Save confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm == MessageBoxResult.No)
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
