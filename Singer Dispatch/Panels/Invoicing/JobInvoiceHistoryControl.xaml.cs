using System;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using SingerDispatch.Database;
using SingerDispatch.Controls;

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
            DgReferenceNumbers.ItemsSource = new ObservableCollection<ReferenceNumber>();
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshAddressesAndContacts();
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);

            RefreshAddressesAndContacts();

            DgInvoices.ItemsSource = (newValue == null) ? null : new ObservableCollection<Invoice>(from i in Database.Invoices where i.Job == newValue orderby i.Number descending, i.Revision descending select i);
        }

        protected override void SelectedInvoiceChanged(Invoice newValue, Invoice oldValue)
        {
            base.SelectedInvoiceChanged(newValue, oldValue);

            var invoice = newValue;
            var list = (ObservableCollection<ReferenceNumber>)DgReferenceNumbers.ItemsSource;

            list.Clear();

            if (invoice == null) return;

            foreach (var item in invoice.ReferenceNumbers)
            {
                list.Add(item);
            }
        }

        private void RefreshAddressesAndContacts()
        {
            if (SelectedJob != null)
            {
                var addressQuery = from a in Database.Addresses where a.Company == SelectedJob.Company || a.Company == SelectedJob.CareOfCompany select a;

                CmbAddresses.ItemsSource = addressQuery.ToList();
                CmbContacts.ItemsSource = (from c in Database.Contacts where addressQuery.Contains(c.Address) select c).ToList();
            }
            else
            {
                CmbAddresses.ItemsSource = null;
                CmbContacts.ItemsSource = null;
            }
        }

        private void NewInvoice_Click(object sender, RoutedEventArgs e)        
        {
            if (SelectedJob == null) return;

            var list = (ObservableCollection<Invoice>) DgInvoices.ItemsSource;
            var invoice = new Invoice { Job = SelectedJob, InvoiceDate = DateTime.Now };

            list.Insert(0, invoice);
            DgInvoices.SelectedItem = invoice;
            DgInvoices.ScrollIntoView(invoice);
            SelectedJob.Invoices.Add(SelectedInvoice);

            try
            {
                EntityHelper.SaveAsNewInvoice(SelectedInvoice, Database);

                CmbAddresses.Focus();
            }
            catch (Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.ToString());
            }
        }

        private void CreateRevision_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedInvoice == null) return;

            var list = (ObservableCollection<Invoice>)DgInvoices.ItemsSource;
            var invoice = (Invoice)DgInvoices.SelectedItem;

            invoice = invoice.Duplicate();

            SelectedJob.Invoices.Add(invoice);
            list.Insert(0, invoice);
            DgInvoices.SelectedItem = invoice;
            DgInvoices.ScrollIntoView(invoice);
            SelectedJob.Invoices.Add(SelectedInvoice);

            try
            {
                EntityHelper.SaveAsInvoiceRevision(SelectedInvoice, Database);   
            }
            catch (Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
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

                ((ObservableCollection<Invoice>)DgInvoices.ItemsSource).Remove(SelectedInvoice);
                SelectedJob.Invoices.Remove(SelectedInvoice);

                Database.SubmitChanges();
            }
            catch (Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void AddReferenceNumber_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedInvoice == null) return;

            var reference = new ReferenceNumber();

            SelectedInvoice.ReferenceNumbers.Add(reference);
            ((ObservableCollection<ReferenceNumber>)DgReferenceNumbers.ItemsSource).Add(reference);
            DgReferenceNumbers.SelectedItem = reference;
            DgReferenceNumbers.ScrollIntoView(reference);

            DataGridHelper.GetCell(DgReferenceNumbers, DgReferenceNumbers.SelectedIndex, 0).Focus();
        }

        private void RemoveReferenceNumber_Click(object sender, RoutedEventArgs e)
        {
            var selected = (ReferenceNumber) DgReferenceNumbers.SelectedItem;

            if (SelectedInvoice == null || selected == null) return;

            SelectedInvoice.ReferenceNumbers.Remove(selected);
            ((ObservableCollection<ReferenceNumber>) DgReferenceNumbers.ItemsSource).Remove(selected);
        }
    }
}
