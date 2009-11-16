using System.Windows;

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
