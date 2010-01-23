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
using SingerDispatch.Database;

namespace SingerDispatch.Panels.Invoicing
{
    /// <summary>
    /// Interaction logic for JobInvoiceHistoryControl.xaml
    /// </summary>
    public partial class JobInvoiceHistoryControl : InvoiceUserControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public JobInvoiceHistoryControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            //cmbContacts.ItemsSource = (SelectedJob == null) ? null : from c in Database.Con
            if (SelectedJob != null)
            {
                var addressQuery = from a in Database.Addresses where a.Company == SelectedJob.Company || a.Company == SelectedJob.CareOfCompany select a;
                cmbContacts.ItemsSource = from c in Database.Contacts where addressQuery.Contains(c.Address) select c;
            }
            else
            {
                cmbContacts.ItemsSource = null;
            }
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);

            dgInvoices.ItemsSource = (newValue == null) ? null : new ObservableCollection<Invoice>(from i in Database.Invoices where i.Job == newValue orderby i.Number descending, i.Revision descending select i);
        }

        private void NewInvoice_Click(object sender, RoutedEventArgs e)        
        {
            if (SelectedJob == null) return;

            var list = (ObservableCollection<Invoice>)dgInvoices.ItemsSource;
            var invoice = new Invoice();

            list.Insert(0, invoice);
            dgInvoices.SelectedItem = invoice;
            dgInvoices.ScrollIntoView(invoice);

            txtComment.Focus();
        }

        private void CreateRevision_Click(object sender, RoutedEventArgs e)
        {  }

        private void PrintInvoice_Click(object sender, RoutedEventArgs e)
        {  }

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
                SingerDispatch.Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }
        
    }
}
