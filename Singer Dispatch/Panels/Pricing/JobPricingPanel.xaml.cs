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
using SingerDispatch.Panels.Jobs;

namespace SingerDispatch.Panels.Pricing
{
    /// <summary>
    /// Interaction logic for JobPricingPanel.xaml
    /// </summary>
    public partial class JobPricingPanel : JobUserControl
    {
        public JobPricingPanel()
        {
            InitializeComponent();
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            IsEnabled = newValue != null;
        }

        private void btnCommitJobChanges_Click(object sender, RoutedEventArgs e)
        {
            SingerConstants.CommonDataContext.SubmitChanges();
        }
    }
}
