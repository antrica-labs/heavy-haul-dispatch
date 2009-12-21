using System.Windows;

namespace SingerDispatch.Panels.Invoicing
{
    /// <summary>
    /// Interaction logic for JobInvoicingPanel.xaml
    /// </summary>
    public partial class JobInvoicingPanel : JobUserControl
    {
        public JobInvoicingPanel()
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
