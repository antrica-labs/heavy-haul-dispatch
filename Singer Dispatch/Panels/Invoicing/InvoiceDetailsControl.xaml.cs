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
using System.Collections.ObjectModel;
using SingerDispatch.Controls;
using SingerDispatch.Windows;

namespace SingerDispatch.Panels.Invoicing
{
    /// <summary>
    /// Interaction logic for InvoiceDetailsControl.xaml
    /// </summary>
    public partial class InvoiceDetailsControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public InvoiceDetailsControl()
        {
            InitializeComponent();

            if (InDesignMode()) return;

            Database = SingerConfigs.CommonDataContext;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode() || IsVisible == false) return;

            UpdateLists();
        }

        protected override void SelectedInvoiceChanged(Invoice newValue, Invoice oldValue)
        {
            base.SelectedInvoiceChanged(newValue, oldValue);

            UpdateLists();
            UpdatePriceAndHours();
        }

        private void UpdateLists()
        {
            if (SelectedInvoice != null)
            {
                dgReferenceNumbers.ItemsSource = new ObservableCollection<InvoiceReferenceNumber>(from r in Database.InvoiceReferenceNumbers where r.Invoice == SelectedInvoice select r);
                dgLineItems.ItemsSource = new ObservableCollection<InvoiceLineItem>(from l in Database.InvoiceLineItems where l.Invoice == SelectedInvoice select l);

                var company = SelectedInvoice.Company;
                var careOf = (SelectedInvoice.Job != null) ? SelectedInvoice.Job.CareOfCompany : null;
                
                if (careOf == null)
                {
                    cmbAddresses.ItemsSource = new ObservableCollection<Address>(from a in Database.Addresses where a.Company == company select a);
                    cmbContacts.ItemsSource = new ObservableCollection<Contact>(from c in Database.Contacts where c.Company == company select c);
                }
                else
                {
                    cmbAddresses.ItemsSource = new ObservableCollection<Address>(from a in Database.Addresses where a.Company == company || a.Company == careOf select a);
                    cmbContacts.ItemsSource = new ObservableCollection<Contact>(from c in Database.Contacts where c.Company == company || c.Company == careOf select c);
                }
            }
            else
            {
                dgReferenceNumbers.ItemsSource = null;
                dgLineItems.ItemsSource = null;

                cmbAddresses.ItemsSource = null;
                cmbContacts.ItemsSource = null;
            }
        }

        private void AddReferenceNumber_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedInvoice == null) return;

            var list = (ObservableCollection<InvoiceReferenceNumber>)dgReferenceNumbers.ItemsSource;
            var reference = new InvoiceReferenceNumber();

            SelectedInvoice.ReferenceNumbers.Add(reference);
            list.Add(reference);

            DataGridHelper.EditFirstColumn(dgReferenceNumbers, reference);
        }

        private void RemoveReferenceNumber_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<InvoiceReferenceNumber>)dgReferenceNumbers.ItemsSource;
            var selected = (InvoiceReferenceNumber)dgReferenceNumbers.SelectedItem;

            if (SelectedInvoice == null || selected == null) return;

            SelectedInvoice.ReferenceNumbers.Remove(selected);
            list.Remove(selected);
        }
        
        private void NewLineItem_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedInvoice == null) return;

            var list = (ObservableCollection<InvoiceLineItem>)dgLineItems.ItemsSource;
            var item = new InvoiceLineItem { Invoice = SelectedInvoice };

            SelectedInvoice.InvoiceLineItems.Add(item);
            list.Add(item);
            dgLineItems.SelectedItem = item;

            txtDescription.Focus();
        }

        private void RemoveLineItem_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<InvoiceLineItem>)dgLineItems.ItemsSource;
            var item = (InvoiceLineItem)dgLineItems.SelectedItem;

            if (list == null) return;

            var confirmation = MessageBox.Show(SingerConfigs.DefaultRemoveItemMessage, "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            try
            {
                SelectedInvoice.InvoiceLineItems.Remove(item);
                Database.SubmitChanges();
                list.Remove(item);
            }
            catch (Exception ex)
            {
                NoticeWindow.ShowError("Error while attempting to delete line item", ex.Message);
            }
        }

        private void NewLineExtra_Click(object sender, RoutedEventArgs e)
        {
            var item = (InvoiceLineItem)dgLineItems.SelectedItem;
            var extra = new InvoiceExtra { LineItem = item };

            if (item == null) return;

            ((ObservableCollection<InvoiceExtra>)dgLineExtras.ItemsSource).Add(extra);
            item.Extras.Add(extra);

            DataGridHelper.EditFirstColumn(dgLineExtras, extra);
        }

        private void RemoveLineExtra_Click(object sender, RoutedEventArgs e)
        {
            var item = (InvoiceLineItem)dgLineItems.SelectedItem;
            var extra = (InvoiceExtra)dgLineExtras.SelectedItem;

            if (item == null || extra == null) return;

            ((ObservableCollection<InvoiceExtra>)dgLineExtras.ItemsSource).Remove(extra);
            item.Extras.Remove(extra);
        }

        private void dgLineItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var line = (InvoiceLineItem)dgLineItems.SelectedItem;

            if (dgLineItems.SelectedItem != null)
                dgLineItems.ScrollIntoView(dgLineItems.SelectedItem);

            dgLineExtras.ItemsSource = (line == null) ? null : new ObservableCollection<InvoiceExtra>(from ex in Database.InvoiceExtras where ex.LineItem == line select ex);
        }

        private void RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                try
                {
                    Database.SubmitChanges();
                }
                catch (System.Exception ex)
                {
                    Windows.NoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
                }
            }
        }

        private void UpdatePriceAndHours()
        {
            if (SelectedInvoice == null) return;

            SelectedInvoice.UpdateTotalCost();
            SelectedInvoice.UpdateTotalHours();
        }
    }
}
