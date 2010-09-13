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

            // refresh database lists in case they have been modified elsewhere.
            cmbQuotedBy.ItemsSource = from emp in Database.Employees orderby emp.FirstName, emp.LastName select emp;
            cmbCareOfCompanies.ItemsSource = (SelectedCompany == null) ? null : from c in Database.Companies where c != SelectedCompany && c.IsVisible == true select c;
            
            RefreshAddressesAndContacts();
            CalculateItemizedCost();
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            cmbCareOfCompanies.ItemsSource = (SelectedCompany == null) ? null : from c in Database.Companies where c != SelectedCompany select c;
        }

        protected override void SelectedQuoteChanged(Quote newValue, Quote oldValue)
        {
            base.SelectedQuoteChanged(newValue, oldValue);

            RefreshAddressesAndContacts();
            CalculateItemizedCost();
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
            RefreshAddressesAndContacts();
        }

        private void RefreshAddressesAndContacts()
        {
            if (SelectedQuote != null)
            {
                var addressQuery = from a in Database.Addresses where a.Company == SelectedQuote.Company || a.Company == SelectedQuote.CareOfCompany orderby a.AddressType.Name select a;

                cmbAddresses.ItemsSource = addressQuery.ToList();
                cmbContacts.ItemsSource = (from c in Database.Contacts where addressQuery.Contains(c.Address) orderby c.FirstName, c.LastName select c).ToList();
            }
            else
            {
                cmbAddresses.ItemsSource = null;
                cmbContacts.ItemsSource = null;
            }
        }

        private void ItemizedBilling_Checked(object sender, RoutedEventArgs e)
        {
            if (SelectedQuote == null) return;

            txtPrice.IsReadOnly = true;
            _previousStyle = txtPrice.Style;
            txtPrice.Style = (Style)TryFindResource("ReadOnly");

            CalculateItemizedCost();
        }

        private void ItemizedBilling_Unchecked(object sender, RoutedEventArgs e)
        {
            if (SelectedQuote == null) return;

            txtPrice.IsReadOnly = false;
            txtPrice.Style = _previousStyle;
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
