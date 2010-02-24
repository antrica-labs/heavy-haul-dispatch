using System;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using SingerDispatch.Database;

namespace SingerDispatch.Panels.Invoicing
{
    /// <summary>
    /// Interaction logic for JobInvoiceHistoryControl.xaml
    /// </summary>
    public partial class JobInvoiceHistoryControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public JobInvoiceHistoryControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshAddressesAndContacts();
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);

            RefreshAddressesAndContacts();

            dgInvoices.ItemsSource = (newValue == null) ? null : new ObservableCollection<Invoice>(from i in Database.Invoices where i.Job == newValue orderby i.Number descending, i.Revision descending select i);
        }

        private void RefreshAddressesAndContacts()
        {
            //cmbContacts.ItemsSource = (SelectedJob == null) ? null : from c in Database.Con
            if (SelectedJob != null)
            {
                var addressQuery = from a in Database.Addresses where a.Company == SelectedJob.Company || a.Company == SelectedJob.CareOfCompany select a;

                cmbAddresses.ItemsSource = addressQuery.ToList();
                cmbContacts.ItemsSource = (from c in Database.Contacts where addressQuery.Contains(c.Address) select c).ToList();
            }
            else
            {
                cmbAddresses.ItemsSource = null;
                cmbContacts.ItemsSource = null;
            }
        }

        private void NewInvoice_Click(object sender, RoutedEventArgs e)        
        {
            if (SelectedJob == null) return;

            var list = (ObservableCollection<Invoice>)dgInvoices.ItemsSource;
            var invoice = new Invoice { Job = SelectedJob, InvoiceDate = DateTime.Now };

            list.Insert(0, invoice);
            dgInvoices.SelectedItem = invoice;
            dgInvoices.ScrollIntoView(invoice);

            txtComment.Focus();
        }

        private void CreateRevision_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedInvoice == null) return;

            var list = (ObservableCollection<Invoice>)dgInvoices.ItemsSource;
            var invoice = (Invoice)dgInvoices.SelectedItem;

            invoice = invoice.Duplicate();

            SelectedJob.Invoices.Add(invoice);
            list.Insert(0, invoice);
            dgInvoices.SelectedItem = invoice;
            dgInvoices.ScrollIntoView(invoice);
        }

        private void ViewInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedInvoice == null) return;

            var viewer = new Windows.DocumentViewer();
            viewer.DisplayPrintout(SelectedInvoice);
        }

        private void DeleteInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedInvoice == null) return;

            try
            {
                EntityHelper.PrepareEntityDelete(SelectedInvoice, Database);

                ((ObservableCollection<Invoice>)dgInvoices.ItemsSource).Remove(SelectedInvoice);
                SelectedJob.Invoices.Remove(SelectedInvoice);

                Database.SubmitChanges();
            }
            catch (Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }
        
    }
}
