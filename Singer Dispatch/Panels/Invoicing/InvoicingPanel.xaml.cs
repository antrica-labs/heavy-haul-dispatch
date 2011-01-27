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
using SingerDispatch.Windows;
using SingerDispatch.Printing.Documents;

namespace SingerDispatch.Panels.Invoicing
{
    /// <summary>
    /// Interaction logic for InvoicingPanel.xaml
    /// </summary>
    public partial class InvoicingPanel
    {
        public SingerDispatchDataContext Database { get; set; }

        public InvoicingPanel()
        {
            InitializeComponent();

            if (InDesignMode()) return;
            // Work below can only be done when the real app is running. It fails during design time.

            Database = SingerConfigs.CommonDataContext;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode()) return;

            UpdateLists();
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            UpdateLists();
        }

        private void UpdateLists()
        {
            dgInvoices.ItemsSource = (SelectedCompany == null) ? null : new ObservableCollection<Invoice>(from i in Database.Invoices where i.Company == SelectedCompany orderby i.Number descending select i);
        }

        private void ViewInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedInvoice == null) return;

            var specialized = (SelectedInvoice.Company.CustomerType != null) ? SelectedCompany.CustomerType.IsEnterprise != true : true;

            var viewer = new DocumentViewerWindow(new InvoiceDocument(), SelectedInvoice, string.Format("Invoice #{0}", SelectedInvoice.Number)) { IsMetric = !UseImperialMeasurements, IsSpecializedDocument = specialized };
            viewer.DisplayPrintout();
        }

        private void NewInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCompany == null) return;

            var list = (ObservableCollection<Invoice>)dgInvoices.ItemsSource;
            var builder = new InvoiceBuilderWindow(SelectedCompany, Database) { Owner = Application.Current.MainWindow };
            var invoice = builder.CreateInvoice();

            if (invoice == null) return;
                                    
            try
            {
                EntityHelper.SaveAsNewInvoice(invoice, Database);

                list.Insert(0, invoice);
                SelectedCompany.Invoices.Add(invoice);

                SelectedInvoice = invoice;
            }
            catch (Exception ex)
            {
                NoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }

        private void DeleteInvoice_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<Invoice>)dgInvoices.ItemsSource;

            if (SelectedInvoice == null) return;

            var confirmation = MessageBox.Show(SingerConfigs.DefaultRemoveItemMessage, "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            try
            {
                Database.SubmitChanges(); // Save any unsaved changes before doing delete.

                EntityHelper.PrepareEntityDelete(SelectedInvoice, Database);

                SelectedCompany.Invoices.Remove(SelectedInvoice);
                Database.SubmitChanges();

                list.Remove(SelectedInvoice);
            }
            catch (Exception ex)
            {
                NoticeWindow.ShowError("Error while attempting to delete invoice", ex.Message);
            }
        }

        private void dgInvoices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgInvoices.SelectedItem != null)
                dgInvoices.ScrollIntoView(dgInvoices.SelectedItem);
        }

        private void CreateRevision_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<Invoice>)dgInvoices.ItemsSource;

            if (SelectedInvoice == null) return;

            var revision = SelectedInvoice.Duplicate();

            try
            {
                EntityHelper.SaveAsInvoiceRevision(revision, Database);

                list.Insert(0, revision);
                SelectedCompany.Invoices.Add(revision);

                SelectedInvoice = revision;
            }
            catch (Exception ex)
            {
                NoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }
    }
}
