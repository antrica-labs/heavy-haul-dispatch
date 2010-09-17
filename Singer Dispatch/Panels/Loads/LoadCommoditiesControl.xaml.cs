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

namespace SingerDispatch.Panels.Loads
{
    /// <summary>
    /// Interaction logic for LoadCommoditiesControl.xaml
    /// </summary>
    public partial class LoadCommoditiesControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public LoadCommoditiesControl()
        {
            InitializeComponent();
        }

        private void ThePanel_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateComboBoxes();
        }

        protected override void SelectedLoadChanged(Load newValue, Load oldValue)
        {
            base.SelectedLoadChanged(newValue, oldValue);

        }

        private void UpdateComboBoxes()
        {
            if (SelectedLoad == null) return;

            var companies = from c in Database.Companies select c;

            cmbShipperCompanies.ItemsSource = companies;
            cmbConsigneeCompanies.ItemsSource = companies;
            cmbLoadingSiteContactCompanies.ItemsSource = companies;
            cmbLoadingContactCompanies.ItemsSource = companies;
            cmbUnloadingSiteContactCompanies.ItemsSource = companies;
            cmbUnloadingContactCompanies.ItemsSource = companies;

            var methods = from m in Database.LoadUnloadMethods select m;

            cmbLoadMethods.ItemsSource = methods;
            cmbUnloadMethods.ItemsSource = methods;

            if (cmbLoadingSiteContactCompanies.SelectedItem == null)
                cmbLoadingSiteContactCompanies.SelectedItem = SelectedCompany;

            if (cmbUnloadingSiteContactCompanies.SelectedItem == null)
                cmbUnloadingSiteContactCompanies.SelectedItem = SelectedCompany;

            var provinces = from p in Database.ProvincesAndStates select p;

            cmbLoadingProvinces.ItemsSource = provinces;
            cmbUnloadingProvinces.ItemsSource = provinces;
        }

        private void CommodityName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void NewCommodity_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DuplicateCommodity_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveCommodity_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PrintBoL_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cmbLoadingSiteContactCompanies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var company = (Company)cmbLoadingSiteContactCompanies.SelectedItem;

            cmbLoadingSiteContacts.ItemsSource = (company == null) ? null : (from c in Database.Contacts where c.Address.CompanyID == company.ID orderby c.FirstName, c.LastName select c).ToList();
        }

        private void cmbUnloadingSiteContactCompanies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var company = (Company)cmbUnloadingSiteContactCompanies.SelectedItem;

            cmbUnloadingSiteContacts.ItemsSource = (company == null) ? null : (from c in Database.Contacts where c.Address.CompanyID == company.ID orderby c.FirstName, c.LastName select c).ToList();
        }

        private void cmbLoadingContactCompanies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var company = (Company)cmbLoadingContactCompanies.SelectedItem;

            cmbLoadingContacts.ItemsSource = (company == null) ? null : (from c in Database.Contacts where c.Address.CompanyID == company.ID orderby c.FirstName, c.LastName select c).ToList();
        }

        private void cmbUnloadingContactCompanies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var company = (Company)cmbUnloadingContactCompanies.SelectedItem;

            cmbUnloadingContacts.ItemsSource = (company == null) ? null : (from c in Database.Contacts where c.Address.CompanyID == company.ID orderby c.FirstName, c.LastName select c).ToList();
        }

        private void ShipperCompany_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = (ComboBox)sender;
            var company = (Company)cmb.SelectedItem;

            cmbShipperAddresses.ItemsSource = (company != null) ? company.Addresses : null;
        }

        private void ConsigneeCompany_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = (ComboBox)sender;
            var company = (Company)cmb.SelectedItem;

            cmbConsigneeAddresses.ItemsSource = (company != null) ? company.Addresses : null;
        }
        
    }
}
