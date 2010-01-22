using System.Windows;
using System.Linq;
using SingerDispatch.Database;

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
            
        }

        private void CommitInvoiceChanges_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedInvoice == null) return;

            try
            {
                if (SelectedInvoice.ID == 0)
                {
                    SelectedJob.Invoices.Add(SelectedInvoice);

                    if (SelectedInvoice.Number != null)
                        EntityHelper.SaveAsInvoiceRevision(SelectedInvoice, Database);
                    else
                        EntityHelper.SaveAsNewInvoice(SelectedInvoice, Database);
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
    }
}
