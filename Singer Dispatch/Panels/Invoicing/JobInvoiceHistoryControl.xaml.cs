using System;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using SingerDispatch.Database;
using SingerDispatch.Controls;
using SingerDispatch.Printing.Documents;

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

            Database = SingerConfigs.CommonDataContext;
            DgReferenceNumbers.ItemsSource = new ObservableCollection<InvoiceReferenceNumber>();
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshAddressesAndContacts();
            UpdatePriceAndHours();
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);
            
            DgInvoices.ItemsSource = (newValue == null) ? null : new ObservableCollection<Invoice>(from i in Database.Invoices where i.Job == newValue orderby i.Number descending, i.Revision descending select i);

            RefreshAddressesAndContacts();
        }

        protected override void SelectedInvoiceChanged(Invoice newValue, Invoice oldValue)
        {
            base.SelectedInvoiceChanged(newValue, oldValue);

            var invoice = newValue;
            var list = (ObservableCollection<InvoiceReferenceNumber>)DgReferenceNumbers.ItemsSource;

            list.Clear();

            if (invoice == null) return;

            foreach (var item in invoice.ReferenceNumbers)
            {
                list.Add(item);
            }


            UpdatePriceAndHours();
        }

        private void UpdatePriceAndHours()
        {
            if (SelectedInvoice == null) return;

            SelectedInvoice.TotalCost = 0.00m;
            SelectedInvoice.TotalHours = 0.0;

            foreach (var item in SelectedInvoice.InvoiceLineItems)
            {
                if (item.Hours != null)
                    SelectedInvoice.TotalHours += item.Hours;

                if (item.Cost != null)
                    SelectedInvoice.TotalCost += item.Cost;

                foreach (var extra in item.Extras)
                {
                    if (extra.Hours != null)
                        SelectedInvoice.TotalHours += extra.Hours;

                    if (extra.Cost != null)
                        SelectedInvoice.TotalCost += extra.Cost;
                }
            }
        }

        private void RefreshAddressesAndContacts()
        {
            if (SelectedJob != null)
            {
                CmbAddresses.ItemsSource = (from a in Database.Addresses where a.Company == SelectedJob.Company || a.Company == SelectedJob.CareOfCompany select a).ToList();
                CmbContacts.ItemsSource = (from c in Database.Contacts where c.Company == SelectedJob.Company || c.Company == SelectedJob.CareOfCompany orderby c.FirstName, c.LastName select c).ToList();
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

            foreach (var item in SelectedJob.ReferenceNumbers)
            {
                var reference = new InvoiceReferenceNumber { Field = item.Field, Value = item.Value };

                invoice.ReferenceNumbers.Add(reference);
            }

            SelectedJob.Invoices.Add(SelectedInvoice);
            list.Add(invoice);            
            DgInvoices.SelectedItem = invoice;
            DgInvoices.ScrollIntoView(invoice);            

            try
            {
                EntityHelper.SaveAsNewInvoice(SelectedInvoice, Database);

                CmbAddresses.Focus();
            }
            catch (Exception ex)
            {
                Windows.NoticeWindow.ShowError("Error while attempting to write changes to database", ex.ToString());
            }
        }

        private void CreateRevision_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedInvoice == null) return;

            var list = (ObservableCollection<Invoice>)DgInvoices.ItemsSource;
            var invoice = (Invoice)DgInvoices.SelectedItem;

            invoice = invoice.Duplicate();

            SelectedJob.Invoices.Add(invoice);
            list.Add(invoice);
            DgInvoices.SelectedItem = invoice;
            SelectedJob.Invoices.Add(SelectedInvoice);

            try
            {
                EntityHelper.SaveAsInvoiceRevision(SelectedInvoice, Database);   
            }
            catch (Exception ex)
            {
                Windows.NoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }

        private void ViewInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedInvoice == null) return;

            var viewer = new Windows.DocumentViewerWindow(new InvoiceDocument(), SelectedInvoice) { IsMetric = !UseImperialMeasurements, IsSpecializedDocument = SelectedCompany.CustomerType.IsEnterprise != true };
            viewer.DisplayPrintout();
        }

        private void DeleteInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedInvoice == null) return;

            var confirmation = MessageBox.Show("Are you sure you want to complete remove this invoice?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            try
            {
                EntityHelper.PrepareEntityDelete(SelectedInvoice, Database);
                                
                SelectedJob.Invoices.Remove(SelectedInvoice);
                Database.SubmitChanges();

                ((ObservableCollection<Invoice>)DgInvoices.ItemsSource).Remove(SelectedInvoice);
            }
            catch (Exception ex)
            {
                Windows.NoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }

        private void AddReferenceNumber_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedInvoice == null) return;

            var reference = new InvoiceReferenceNumber();

            SelectedInvoice.ReferenceNumbers.Add(reference);
            ((ObservableCollection<InvoiceReferenceNumber>)DgReferenceNumbers.ItemsSource).Add(reference);

            DataGridHelper.EditFirstColumn(DgReferenceNumbers, reference);
        }

        private void RemoveReferenceNumber_Click(object sender, RoutedEventArgs e)
        {
            var selected = (InvoiceReferenceNumber)DgReferenceNumbers.SelectedItem;

            if (SelectedInvoice == null || selected == null) return;

            SelectedInvoice.ReferenceNumbers.Remove(selected);
            ((ObservableCollection<InvoiceReferenceNumber>)DgReferenceNumbers.ItemsSource).Remove(selected);
        }

        private void DgInvoices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var grid = (DataGrid)sender;

            if (grid.SelectedItem == null) return;

            grid.ScrollIntoView(grid.SelectedItem);
            grid.UpdateLayout();
        }
    }
}
