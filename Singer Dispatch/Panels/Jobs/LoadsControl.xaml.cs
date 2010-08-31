using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using SingerDispatch.Database;
using SingerDispatch.Windows;
using System.Collections.Generic;
using SingerDispatch.Printing.Documents;

namespace SingerDispatch.Panels.Jobs
{
    /// <summary>
    /// Interaction logic for LoadsControl.xaml
    /// </summary>
    public partial class LoadsControl
    {
        public SingerDispatchDataContext Database { get; set; }
        public Status DefaultLoadStatus { get; set; }

        public LoadsControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;

            DefaultLoadStatus = (from s in Database.Statuses where s.Name == "Pending" select s).First();
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            var companies = from c in Database.Companies select c;

            cmbShipperCompanies.ItemsSource = companies;
            cmbConsigneeCompanies.ItemsSource = companies;
            cmbLoadingSiteContactCompanies.ItemsSource = companies;
            cmbLoadingContactCompanies.ItemsSource = companies;
            cmbUnloadingSiteContactCompanies.ItemsSource = companies;
            cmbUnloadingContactCompanies.ItemsSource = companies;
            
            cmbSeasons.ItemsSource = from s in Database.Seasons select s;
            cmbRates.ItemsSource = GetCompanyRates(SelectedCompany);
            cmbUnits.ItemsSource = (SelectedJob == null) ? null : from u in Database.Equipment where u.EquipmentClass.Name == "Tractor" || u.EquipmentClass.Name == "Trailor" orderby u.UnitNumber select u;
            cmbStatuses.ItemsSource = from s in Database.Statuses select s;

            if (cmbRates.SelectedItem != null)
            {
                cmbTrailerCombinations.ItemsSource = (from tc in Database.TrailerCombinations where tc.Rate == cmbRates.SelectedItem select tc).ToList();
            }

            var methods = (SelectedJob == null) ? null : (from m in Database.LoadUnloadMethods select m).ToList();

            cmbLoadMethods.ItemsSource = methods;
            cmbUnloadMethods.ItemsSource = methods;
            
            UpdateExtras();
            UpdateCommodityList();
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);

            dgLoads.ItemsSource = (newValue == null) ? null : new ObservableCollection<Load>(from l in newValue.Loads orderby l.Number select l);
        }

        private void NewLoad_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<Load>)dgLoads.ItemsSource;
            var load = new Load { Job = SelectedJob, StartDate = SelectedJob.StartDate, EndDate = SelectedJob.EndDate, Status = DefaultLoadStatus };

            SelectedJob.Loads.Add(load);
            list.Add(load);
            
            dgLoads.ScrollIntoView(load);
            dgLoads.SelectedItem = load;

            try
            {
                EntityHelper.SaveAsNewLoad(load, Database);                
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void DuplicateLoad_Click(object sender, RoutedEventArgs e)
        {
            var load = (Load)dgLoads.SelectedItem;
            var list = (ObservableCollection<Load>)dgLoads.ItemsSource;

            if (load == null)
                return;

            load = load.Duplicate();

            SelectedJob.Loads.Add(load);
            list.Add(load);

            dgLoads.ScrollIntoView(load);
            dgLoads.SelectedItem = load;            

            try
            {
                EntityHelper.SaveAsNewLoad(load, Database);                
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void RemoveLoad_Click(object sender, RoutedEventArgs e)
        {
            var load = (Load)dgLoads.SelectedItem;
            var list = (ObservableCollection<Load>)dgLoads.ItemsSource;

            if (load == null) return;

            var confirmation = MessageBox.Show(SingerConstants.DefaultRemoveItemMessage, "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            foreach (var item in load.Dispatches.ToList())
                item.Load = null;

            foreach (var item in load.LoadedCommodities.ToList())
                item.Load = null;

            foreach (var item in load.Permits.ToList())
                item.Load = null;

            foreach (var item in load.ThirdPartyServices.ToList())
                item.Load = null;

            list.Remove(load);
            SelectedJob.Loads.Remove(load);
            dgLoads.SelectedItem = null;
        }

        private void AxleWeightChanged(object sender, TextChangedEventArgs e)
        {
            var load = (Load)dgLoads.SelectedItem;

            if (load == null)
            {
                return;
            }
            
            double? gross = 0;

            gross += load.SWeightSteer;
            gross += load.SWeightDrive;
            gross += load.SWeightGroup1;
            gross += load.SWeightGroup2;
            gross += load.SWeightGroup3;
            gross += load.SWeightGroup4;
            gross += load.SWeightGroup5;
            gross += load.SWeightGroup6;
            gross += load.SWeightGroup7;
            gross += load.SWeightGroup8;
            gross += load.SWeightGroup9;
            gross += load.SWeightGroup10;

            load.GrossWeight = gross;
        }

        private void dgLoads_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateExtras();
            UpdateCommodityList();
        }

        private void UpdateExtras()
        {
            if (dgLoads.SelectedItem == null) return;

            var load = (Load)dgLoads.SelectedItem;
            var types = (from et in Database.ExtraEquipmentTypes orderby et.Name select et).ToList();
            var list = new ObservableCollection<ExtraEquipment>(load.ExtraEquipment);
                        
            lbExtraEquipmentTypes.ItemsSource = types;

            dgExtraEquipment.ItemsSource = list;
        }

        private void UpdateCommodityList()
        {
            lbCommodities.ItemsSource = null;

            if (dgLoads.SelectedItem == null) return;

            var load = (Load)dgLoads.SelectedItem;
            var checkboxList = new List<CheckBox>();

            foreach (var item in SelectedJob.JobCommodities)
            {
                bool isLoaded = (from lc in load.LoadedCommodities where lc.JobCommodity == item select lc).Count() > 0;
                var cb = new CheckBox { Content = item.NameAndUnit, DataContext = item, IsChecked = isLoaded };

                cb.Checked += CommodityCheckBox_Checked;
                cb.Unchecked += CommodityCheckBox_Unchecked;

                checkboxList.Add(cb);
            }

            lbCommodities.ItemsSource = checkboxList;
        }
        
        private void CommodityCheckBox_Checked(object sender, RoutedEventArgs e)
        {            
            var cb = (CheckBox)sender;
            var commodity = (JobCommodity)cb.DataContext;
            var load = (Load)dgLoads.SelectedItem;
            
            if (load == null || commodity == null) return;

            var loaded = new LoadedCommodity { JobCommodity = commodity, Load = load };

            load.LoadedCommodities.Add(loaded);
            cmbLoadedCommodities.ItemsSource = load.LoadedCommodities.GetNewBindingList();
            load.Notify("LoadedCommodities");
        }

        private void CommodityCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {            
            var cb = (CheckBox)sender;
            var commodity = (JobCommodity)cb.DataContext;
            var load = (Load)dgLoads.SelectedItem;

            if (load == null || commodity == null) return;

            foreach (var item in (from lc in load.LoadedCommodities where lc.JobCommodity == commodity select lc).ToList())
            {
                load.LoadedCommodities.Remove(item);
            }

            cmbLoadedCommodities.ItemsSource = load.LoadedCommodities.GetNewBindingList();
            load.Notify("LoadedCommodities");           
        }

        private System.Collections.IEnumerable GetCompanyRates(Company company)
        {
            if (company == null)
            {
                return null;
            }

            var rates = from r in Database.Rates where r.RateType.Name == "Trailer" select r;
            var discount = company.RateAdjustment ?? 0.00m;
            var enterprise = company.CustomerType != null && company.CustomerType.IsEnterprise == true;

            foreach (var rate in rates)
            {
                if (enterprise && rate.HourlyEnterprise != null)
                {
                    rate.Hourly = rate.HourlySpecialized;
                    rate.Adjusted = rate.Hourly + discount;
                }
                else if (!enterprise && rate.HourlySpecialized != null)
                {
                    rate.Hourly = rate.HourlyEnterprise;
                    rate.Adjusted = rate.Hourly + discount;
                }
            }

            return rates;
        }

        private void cmbRates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbTrailerCombinations.ItemsSource = from tc in Database.TrailerCombinations where tc.Rate == cmbRates.SelectedItem select tc;
        }

        private void AddEquipment_Click(object sender, RoutedEventArgs e)
        {
            var load = (Load)dgLoads.SelectedItem;

            if (load == null) return;

            var equipment = new ExtraEquipment();

            load.ExtraEquipment.Add(equipment);
            ((ObservableCollection<ExtraEquipment>)dgExtraEquipment.ItemsSource).Add(equipment);
            dgExtraEquipment.ScrollIntoView(equipment);
            dgExtraEquipment.SelectedItem = equipment;

            lbExtraEquipmentTypes.Focus();
        }

        private void RemoveEquipment_Click(object sender, RoutedEventArgs e)
        {
            var equipment = (ExtraEquipment)dgExtraEquipment.SelectedItem;
            
            ((Load)dgLoads.SelectedItem).ExtraEquipment.Remove(equipment);
            ((ObservableCollection<ExtraEquipment>)dgExtraEquipment.ItemsSource).Remove(equipment);
        }

        private void GuessLoadWeights_Click(object sender, RoutedEventArgs e)
        {
            // Populate as many of the estimated weights and the dimensions as you can based on the tractor, trailer combo, and commodities.

            var load = (Load)dgLoads.SelectedItem;

            if (load == null) return;

            load.GrossWeight = 0.0;

            if (load.Equipment != null)
            {
                load.GrossWeight += load.Equipment.Tare ?? 0.0;                
            }

            if (load.TrailerCombination != null)
            {
                load.GrossWeight += load.TrailerCombination.Tare ?? 0.0;
                load.LoadedWidth = load.TrailerCombination.Width ?? 0.0;
                load.LoadedLength = load.TrailerCombination.Length ?? 0.0;
                load.LoadedHeight = load.TrailerCombination.Height ?? 0.0;
            }

            var widest = 0.0;
            var longest = 0.0;
            var highest = 0.0;

            foreach (var commodity in load.LoadedCommodities)
            {
                load.GrossWeight += commodity.JobCommodity.Weight ?? 0.0;

                var length = commodity.JobCommodity.Length ?? 0.0;
                var height = commodity.JobCommodity.Height ?? 0.0;
                var width = commodity.JobCommodity.Width ?? 0.0;

                if (length > longest)
                    longest = length;

                if (height > highest)
                    highest = height;

                if (width > widest)
                    widest = width;
            }


            load.LoadedHeight += highest;

            if (load.LoadedHeight < SingerConstants.MinLoadHeight)
                load.LoadedHeight = SingerConstants.MinLoadHeight;

            if (widest > load.LoadedWidth)
                load.LoadedWidth = widest;

            if (longest > load.LoadedLength)
                load.LoadedLength = longest;
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

        private void cmbLoadingSiteContactCompanies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var company = (Company)cmbLoadingSiteContactCompanies.SelectedItem;

            cmbLoadingSiteContacts.ItemsSource = (from c in Database.Contacts where c.Address.CompanyID == company.ID orderby c.FirstName, c.LastName select c).ToList();
        }

        private void cmbLoadingSiteContacts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbLoadingContactCompanies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var company = (Company)cmbLoadingContactCompanies.SelectedItem;

            cmbLoadingContacts.ItemsSource = (from c in Database.Contacts where c.Address.CompanyID == company.ID orderby c.FirstName, c.LastName select c).ToList();
        }

        private void cmbLoadingContacts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbUnloadingSiteContactCompanies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var company = (Company)cmbUnloadingSiteContactCompanies.SelectedItem;

            cmbUnloadingSiteContacts.ItemsSource = (from c in Database.Contacts where c.Address.CompanyID == company.ID orderby c.FirstName, c.LastName select c).ToList();
        }

        private void cmbUnloadingSiteContacts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbUnloadingContactCompanies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var company = (Company)cmbUnloadingContactCompanies.SelectedItem;

            cmbUnloadingContacts.ItemsSource = (from c in Database.Contacts where c.Address.CompanyID == company.ID orderby c.FirstName, c.LastName select c).ToList();
        }

        private void cmbUnloadingContacts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbLoadedCommodities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var commodity = (LoadedCommodity)cmbLoadedCommodities.SelectedItem;

            if (commodity == null) return;

            cmbLoadingSiteContactCompanies.SelectedItem = (commodity.LoadSiteContact != null) ? commodity.LoadSiteContact.Address.Company : null;
            cmbLoadingContactCompanies.SelectedItem = (commodity.LoadContact != null) ? commodity.LoadContact.Address.Company : null;
            cmbUnloadingSiteContactCompanies.SelectedItem = (commodity.UnloadSiteContact != null) ? commodity.UnloadSiteContact.Address.Company : null;
            cmbUnloadingContactCompanies.SelectedItem = (commodity.UnloadContact != null) ? commodity.UnloadContact.Address.Company : null;
        }

        private void PrintBillOfLading_Click(object sender, RoutedEventArgs e)
        {
            var commodity = (LoadedCommodity)cmbLoadedCommodities.SelectedItem;

            if (commodity == null) return;

            var viewer = new DocumentViewerWindow(new BillOfLadingDocument(), commodity, string.Format("Bill of Lading - {0}", commodity.JobCommodity.NameAndUnit)) { IsMetric = !UseImperialMeasurements, IsSpecializedDocument = SelectedCompany.CustomerType.IsEnterprise != true };
            viewer.DisplayPrintout();
        }
    }
}
