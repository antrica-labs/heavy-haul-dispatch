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

namespace SingerDispatch.Panels.Pricing
{
    /// <summary>
    /// Interaction logic for JobPricingPanel.xaml
    /// </summary>
    public partial class JobPricingPanel : UserControl
    {
        public static DependencyProperty SelectedCompanyProperty = DependencyProperty.Register("SelectedCompany", typeof(Company), typeof(JobPricingPanel), new PropertyMetadata(null, JobPricingPanel.SelectedCompanyPropertyChanged));

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

        public JobPricingPanel()
        {
            InitializeComponent();
        }

        public static void SelectedCompanyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            JobPricingPanel control = (JobPricingPanel)d;
            Company company = (Company)e.NewValue;

            if (company == null)
            {
                control.IsEnabled = false;
            }
            else
            {
                control.IsEnabled = true;
            }
        }

        private void btnCommitJobChanges_Click(object sender, RoutedEventArgs e)
        {
            SingerConstants.CommonDataContext.SubmitChanges();
        }
    }
}
