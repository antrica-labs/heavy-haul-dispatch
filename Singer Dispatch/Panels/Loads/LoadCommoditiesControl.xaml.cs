using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using SingerDispatch.Printing.Documents;
using SingerDispatch.Windows;

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

            if (InDesignMode()) return;

            Database = SingerConfigs.CommonDataContext;
        }

        private void ThePanel_Loaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode()) return;

            UpdateComboBoxes();
        }

        protected override void SelectedLoadChanged(Load newValue, Load oldValue)
        {
            base.SelectedLoadChanged(newValue, oldValue);

            dgCommodities.ItemsSource = (newValue == null) ? null : new ObservableCollection<LoadedCommodity>(newValue.LoadedCommodities);
        }

        private void UpdateComboBoxes()
        {
            if (SelectedLoad == null) return;

            cmbCommodityName.ItemsSource = SelectedLoad.Job.JobCommodities.ToList();

            var companies = (from c in Database.Companies select c).ToList();

            cmbShipperCompanies.ItemsSource = companies;
            cmbConsigneeCompanies.ItemsSource = companies;
            cmbLoadingSiteContactCompanies.ItemsSource = companies;
            cmbLoadingContactCompanies.ItemsSource = companies;
            cmbUnloadingSiteContactCompanies.ItemsSource = companies;
            cmbUnloadingContactCompanies.ItemsSource = companies;

            var methods = from m in Database.LoadUnloadMethods select m;

            cmbLoadMethods.ItemsSource = methods;
            cmbUnloadMethods.ItemsSource = methods;
            
            var provinces = from p in Database.ProvincesAndStates select p;

            cmbLoadingProvinces.ItemsSource = provinces;
            cmbUnloadingProvinces.ItemsSource = provinces;
        }
                
        private void NewCommodity_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedLoad == null) return;

            var list = (ObservableCollection<LoadedCommodity>)dgCommodities.ItemsSource;
            var loaded = new LoadedCommodity { Load = SelectedLoad, LoadSiteCompany = SelectedCompany, UnloadSiteCompany = SelectedCompany };

            var singerList = from c in (List<Company>)cmbShipperCompanies.ItemsSource where c.Name.Contains(SingerConfigs.SingerSearchString) select c;

            if (singerList.Count() > 0)
                loaded.ShipperCompany = singerList.First();

            try
            {
                loaded.ShipperAddress = loaded.ShipperCompany.Addresses.First();
            }
            catch { }

            loaded.ConsigneeCompany = SelectedCompany;

            try
            {
                loaded.ConsigneeAddress = loaded.ConsigneeCompany.Addresses.First();
            }
            catch { }

            SelectedLoad.LoadedCommodities.Add(loaded);
            list.Add(loaded);
            dgCommodities.SelectedItem = loaded;
            SelectedLoad.Notify("LoadedCommodities");
        }

        private void DuplicateCommodity_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<LoadedCommodity>)dgCommodities.ItemsSource;
            var loaded = (LoadedCommodity)dgCommodities.SelectedItem;

            if (loaded == null) return;

            loaded = loaded.Duplicate();

            SelectedLoad.LoadedCommodities.Add(loaded);
            list.Add(loaded);
            dgCommodities.SelectedItem = loaded;
            SelectedLoad.Notify("LoadedCommodities");
        }

        private void RemoveCommodity_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<LoadedCommodity>)dgCommodities.ItemsSource;
            var loaded = (LoadedCommodity)dgCommodities.SelectedItem;

            if (loaded == null) return;

            var confirmation = MessageBox.Show(SingerConfigs.DefaultRemoveItemMessage, "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            list.Remove(loaded);
            SelectedLoad.LoadedCommodities.Remove(loaded);
            SelectedLoad.Notify("LoadedCommodities");
        }

        private void PrintBoL_Click(object sender, RoutedEventArgs e)
        {
            var loaded = (LoadedCommodity)dgCommodities.SelectedItem;

            if (loaded == null || loaded.JobCommodity == null) return;

            var viewer = new DocumentViewerWindow(new BillOfLadingDocument(), loaded, string.Format("Bill of Lading - {0}", loaded.JobCommodity.NameAndUnit)) { IsMetric = !UseImperialMeasurements, IsSpecializedDocument = SelectedCompany.CustomerType.IsEnterprise != true };
            viewer.DisplayPrintout();
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

        private void dgCommodities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var grid = (DataGrid)sender;

            if (grid.SelectedItem == null) return;

            grid.ScrollIntoView(grid.SelectedItem);
            grid.UpdateLayout();

            UpdateComboBoxes();
            
            if (SelectedLoad != null)
                SelectedLoad.Notify("LoadedCommodities");
        }
        
    }
}
