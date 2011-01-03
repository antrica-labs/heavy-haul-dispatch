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

            UpdateCareOfCompanies();
            UpdateAddressesAndContacts();
            CalculateItemizedCost();
        }

        private void UpdateAuthorList()
        {
            //var selected = cmbQuotedBy.SelectedItem;
            cmbQuotedBy.ItemsSource = from emp in Database.Employees where emp.Archived != true orderby emp.FirstName, emp.LastName select emp;
            //cmbQuotedBy.SelectedItem = selected;
        }

        private void UpdateCareOfCompanies()
        {
            
        }

        private void UpdateAddressesAndContacts()
        {
            if (SelectedQuote != null)
            {
                var address = SelectedQuote.BillingAddress;
                var contact = SelectedQuote.Contact;

                cmbAddresses.ItemsSource = new ObservableCollection<Address>(from a in Database.Addresses where a.Company == SelectedQuote.Company || a.Company == SelectedQuote.CareOfCompany orderby a.AddressType.Name select a);
                cmbContacts.ItemsSource = new ObservableCollection<Contact>(from c in Database.Contacts where c.Company == SelectedQuote.Company || c.Company == SelectedQuote.CareOfCompany orderby c.FirstName, c.LastName select c);

                SelectedQuote.BillingAddress = address;
                SelectedQuote.Contact = contact;
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

            foreach (var item in (from c in SelectedQuote.QuoteSupplements where c.Cost != null select c))
            {
                SelectedQuote.Price += item.Cost;
            }
        }

        private void AddCompany_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedQuote == null) return;

            var window = new CreateCompanyWindow(Database) { Owner = Application.Current.MainWindow };
            var company = window.CreateCompany();

            if (company == null) return;

            try
            {
                Database.SubmitChanges();
                CompanyList.Add(company);

                SelectedQuote.CareOfCompany = company;

                UpdateAddressesAndContacts();
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Error while adding company to database", ex.Message);
            }
        }

        private void AddContact_Click(object sender, RoutedEventArgs e)
        {
                if (SelectedQuote == null) return;

                var window = new CreateContactWindow(Database, SelectedCompany, SelectedQuote.CareOfCompany) { Owner = Application.Current.MainWindow };
                var contact = window.CreateContact();

                if (contact == null || contact.Company == null) return;

                var list = (ObservableCollection<Contact>)cmbContacts.ItemsSource;

                contact.Company.Contacts.Add(contact);
                list.Add(contact);

                SelectedQuote.Contact = contact;
        }

        private void AddAddress_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedQuote == null) return;

            var window = new CreateAddressWindow(Database, SelectedCompany, SelectedQuote.CareOfCompany) { Owner = Application.Current.MainWindow };
            var address = window.CreateAddress();

            if (address == null || address.Company == null) return;

            var list = (ObservableCollection<Address>)cmbAddresses.ItemsSource;

            address.Company.Addresses.Add(address);
            list.Add(address);

            SelectedQuote.BillingAddress = address;                
        }
    }
}
