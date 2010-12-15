using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using SingerDispatch.Database;
using SingerDispatch.Windows;
using System.Windows.Data;
using SingerDispatch.Printing.Documents;

namespace SingerDispatch.Panels.Quotes
{
    /// <summary>
    /// Interaction logic for QuoteDetailsControl.xaml
    /// </summary>
    public partial class QuoteDetailsControl
    {
        private Style _previousStyle;

        public SingerDispatchDataContext Database { get; set; }

        public QuoteDetailsControl()
        {
            InitializeComponent();

            if (InDesignMode()) return;

            Database = SingerConfigs.CommonDataContext;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode()) return;

            UpdateAuthorList();
            UpdateCareOfCompanies();
            UpdateAddressesAndContacts();
            CalculateItemizedCost();
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            if (!IsVisible) return;
            
            UpdateCareOfCompanies();
        }

        protected override void SelectedQuoteChanged(Quote newValue, Quote oldValue)
        {
            base.SelectedQuoteChanged(newValue, oldValue);

            UpdateAuthorList();
            UpdateCareOfCompanies();
            UpdateAddressesAndContacts();
            CalculateItemizedCost();
        }

        private void UpdateAuthorList()
        {
            if (SelectedQuote != null)
            {
                var selected = cmbQuotedBy.SelectedItem;
                cmbQuotedBy.ItemsSource = from emp in Database.Employees orderby emp.FirstName, emp.LastName select emp;
                cmbQuotedBy.SelectedItem = selected;
            }
            else
                cmbQuotedBy.ItemsSource = null;
        }

        private void UpdateCareOfCompanies()
        {
            
        }

        private void UpdateAddressesAndContacts()
        {
            if (SelectedQuote != null)
            {
                var address = cmbAddresses.SelectedItem;
                var contact = cmbContacts.SelectedItem;

                var addressQuery = from a in Database.Addresses where a.Company == SelectedQuote.Company || a.Company == SelectedQuote.CareOfCompany orderby a.AddressType.Name select a;

                cmbAddresses.ItemsSource = addressQuery.ToList();
                cmbContacts.ItemsSource = (from c in Database.Contacts where c.Company == SelectedQuote.Company || c.Company == SelectedQuote.CareOfCompany orderby c.FirstName, c.LastName select c).ToList();

                cmbAddresses.SelectedItem = address;
                cmbContacts.SelectedItem = contact;
            }
            else
            {
                cmbAddresses.ItemsSource = null;
                cmbContacts.ItemsSource = null;
            }
        }

        private void ViewQuote_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedQuote == null) return;

            var title = String.Format("Quote #{0}-{1}", SelectedQuote.Number, SelectedQuote.Revision);

            var viewer = new Windows.DocumentViewerWindow(new QuoteDocument(), SelectedQuote, title) { IsMetric = !UseImperialMeasurements, IsSpecializedDocument = SelectedCompany.CustomerType.IsEnterprise != true };
            viewer.DisplayPrintout();
        }

        private void CareOfCompanies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAddressesAndContacts();
        }        

        private void ItemizedBilling_Checked(object sender, RoutedEventArgs e)
        {
            if (SelectedQuote == null) return;

            txtPrice.IsReadOnly = true;
            _previousStyle = txtPrice.Style;
            txtPrice.Style = (Style)TryFindResource("ReadOnly");

            SelectedQuote.PrintoutCostHeading = "Price";

            CalculateItemizedCost();
        }

        private void ItemizedBilling_Unchecked(object sender, RoutedEventArgs e)
        {
            if (SelectedQuote == null) return;

            txtPrice.IsReadOnly = false;
            txtPrice.Style = _previousStyle;

            SelectedQuote.PrintoutCostHeading = "";
        }

        private void CalculateItemizedCost()
        {
            if (SelectedQuote == null || SelectedQuote.IsItemizedBilling != true) return;

            SelectedQuote.Price = 0.00m;

            foreach (var item in (from c in SelectedQuote.QuoteCommodities where c.Cost != null select c.Cost))
            {
                SelectedQuote.Price += item;
            }

            foreach (var item in (from c in SelectedQuote.QuoteSupplements where c.CostPerItem != null select c))
            {
                if (item.Quantity != null)
                    SelectedQuote.Price += item.Quantity * item.CostPerItem;
                else
                    SelectedQuote.Price += item.CostPerItem;
            }
        }
    }
}
