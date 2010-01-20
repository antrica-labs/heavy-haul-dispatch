using System.Windows;
using System.Linq;

namespace SingerDispatch.Panels.Invoicing
{
    /// <summary>
    /// Interaction logic for JobInvoicingPanel.xaml
    /// </summary>
    public partial class JobInvoicingPanel : InvoiceUserControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public JobInvoicingPanel()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
            cmbJobList.Focus();
        }

        private void Panel_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshJobList();
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            RefreshJobList();
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);

        }

        private void RefreshJobList()
        {
            cmbJobList.ItemsSource = (SelectedCompany == null) ? null : from j in Database.Jobs where j.Company == SelectedCompany orderby j.Number select j;
        }

        private void cmbJobList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            //SelectedJob = (Job)cmbJobList.SelectedItem;
        }
    }
}
