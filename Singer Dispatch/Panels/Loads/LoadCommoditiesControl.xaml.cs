using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using SingerDispatch.Printing.Documents;
using SingerDispatch.Windows;
using System.Windows.Input;
using SingerDispatch.Controls;
using System;

namespace SingerDispatch.Panels.Loads
{
    /// <summary>
    /// Interaction logic for LoadCommoditiesControl.xaml
    /// </summary>
    public partial class LoadCommoditiesControl
    {
        public static DependencyProperty CommonSiteLocationsProperty = DependencyProperty.Register("CommonSiteLocations", typeof(ObservableCollection<string>), typeof(LoadCommoditiesControl));
        public static DependencyProperty CommonSiteAddressesProperty = DependencyProperty.Register("CommonSiteAddresses", typeof(ObservableCollection<string>), typeof(LoadCommoditiesControl));
        public static DependencyProperty CommonRoutesProperty = DependencyProperty.Register("CommonRoutes", typeof(ObservableCollection<string>), typeof(LoadCommoditiesControl));
        public static DependencyProperty CommonInstructionsProperty = DependencyProperty.Register("CommonInstructions", typeof(ObservableCollection<string>), typeof(LoadCommoditiesControl));

        public SingerDispatchDataContext Database { get; set; }

        public ObservableCollection<string> CommonRoutes
        {
            get
            {
                return (ObservableCollection<string>)GetValue(CommonRoutesProperty);
            }
            set
            {
                SetValue(CommonRoutesProperty, value);
            }
        }

        public ObservableCollection<string> CommonInstructions
        {
            get
            {
                return (ObservableCollection<string>)GetValue(CommonInstructionsProperty);
            }
            set
            {
                SetValue(CommonInstructionsProperty, value);
            }
        }

        public ObservableCollection<string> CommonSiteLocations
        {
            get
            {
                return (ObservableCollection<string>)GetValue(CommonSiteLocationsProperty);
            }
            set
            {
                SetValue(CommonSiteLocationsProperty, value);
            }
        }

        public ObservableCollection<string> CommonSiteAddresses
        {
            get
            {
                return (ObservableCollection<string>)GetValue(CommonSiteAddressesProperty);
            }
            set
            {
                SetValue(CommonSiteAddressesProperty, value);
            }
        }


        public LoadCommoditiesControl()
        {
            InitializeComponent();

            CommonRoutes = new ObservableCollection<string>();
            CommonSiteAddresses = new ObservableCollection<string>();
            CommonSiteLocations = new ObservableCollection<string>();
            CommonInstructions = new ObservableCollection<string>();

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

            if (newValue != null)
            {
                var list = new ObservableCollection<LoadedCommodity>(newValue.LoadedCommodities);

                dgCommodities.ItemsSource = list;

                if (list.Count > 0)
                    dgCommodities.SelectedItem = list.First();
            }
            else
                dgCommodities.ItemsSource = null;
            
            UpdateComboBoxes();
        }

        protected override void UseImperialMeasurementsChanged(bool value)
        {
            base.UseImperialMeasurementsChanged(value);
        }

        private void UpdateComboBoxes()
        {
            if (SelectedLoad == null) return;

            cmbSeasons.ItemsSource = from s in Database.Seasons select s;
            cmbRates.ItemsSource = GetCompanyRates(SelectedCompany);
            cmbUnits.ItemsSource = (SelectedLoad == null) ? null : from u in Database.Equipment where u.Archived != true && u.EquipmentType.EquipmentClass.Name == "Tractor" orderby u.UnitNumber select u;

            if (cmbRates.SelectedItem != null)
            {
                cmbTrailerCombinations.ItemsSource = (from tc in Database.TrailerCombinations where tc.Rate == cmbRates.SelectedItem select tc).ToList();
            }

            cmbCommodityName.ItemsSource = SelectedLoad.Job.JobCommodities.ToList();

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

            var singerList = from c in CompanyList where c.Name.Contains(SingerConfigs.SingerSearchString) select c;

            if (singerList.Count() > 0)
            {
                var company = singerList.First();

                loaded.LoadingCompany = company;
                loaded.UnloadingCompany = company;
            }
            
            try
            {
                loaded.ShipperCompany = loaded.ConsigneeCompany = SelectedCompany;

                loaded.ShipperAddress = loaded.ConsigneeAddress = (from a in loaded.ShipperCompany.Addresses where a.AddressType.Name == "Head Office" select a).First();
            }
            catch
            {
                try
                {
                    loaded.ShipperAddress = loaded.ConsigneeAddress = loaded.ShipperCompany.Addresses.First();
                }
                catch { }
            }

            cmbLoadingSiteContactCompanies.SelectedItem = SelectedCompany;
            cmbUnloadingSiteContactCompanies.SelectedItem = SelectedCompany;

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
            UpdateContactList(cmbLoadingSiteContacts, (Company)cmbLoadingSiteContactCompanies.SelectedItem);
        }
        
        private void cmbUnloadingSiteContactCompanies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateContactList(cmbUnloadingSiteContacts, (Company)cmbUnloadingSiteContactCompanies.SelectedItem);
        }

        private void cmbLoadingContactCompanies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateContactList(cmbLoadingContacts, (Company)cmbLoadingContactCompanies.SelectedItem);            
        }

        private void cmbUnloadingContactCompanies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateContactList(cmbUnloadingContacts, (Company)cmbUnloadingContactCompanies.SelectedItem);
        }

        private void UpdateContactList(ComboBox cmb, Company company)
        {
            cmb.ItemsSource = (company == null) ? null : new ObservableCollection<Contact>(from c in Database.Contacts where c.Company == company orderby c.FirstName, c.LastName select c);
        }

        private void ShipperCompany_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = (ComboBox)sender;
            var company = (Company)cmb.SelectedItem;

            UpdatedAddressList(cmbShipperAddresses, company);
        }

        private void ConsigneeCompany_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = (ComboBox)sender;
            var company = (Company)cmb.SelectedItem;

            UpdatedAddressList(cmbConsigneeAddresses, (Company)cmb.SelectedItem);
        }

        private void UpdatedAddressList(ComboBox cmb, Company company)
        {
            cmb.ItemsSource = (company == null) ? null : new ObservableCollection<Address>(from a in Database.Addresses where a.Company == company select a);
        }

        private void dgCommodities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var grid = (DataGrid)sender;

            if (grid.SelectedItem == null) return;

            grid.ScrollIntoView(grid.SelectedItem);
            grid.UpdateLayout();

            UpdateComboBoxes();
            UpdateCommonLists();

            if (SelectedLoad != null)
                SelectedLoad.Notify("LoadedCommodities");
        }

        private void UpdateCommonLists()
        {
            var list = (ObservableCollection<LoadedCommodity>)dgCommodities.ItemsSource;

            if (list == null) return;

            foreach (var item in list)
            {
                if (!string.IsNullOrWhiteSpace(item.LoadLocation) && !CommonSiteLocations.Contains(item.LoadLocation))
                    CommonSiteLocations.Add(item.LoadLocation);
                if (!string.IsNullOrWhiteSpace(item.UnloadLocation) && !CommonSiteLocations.Contains(item.UnloadLocation))
                    CommonSiteLocations.Add(item.UnloadLocation);

                if (!string.IsNullOrWhiteSpace(item.LoadAddress) && !CommonSiteAddresses.Contains(item.LoadAddress))
                    CommonSiteAddresses.Add(item.LoadAddress);
                if (!string.IsNullOrWhiteSpace(item.UnloadAddress) && !CommonSiteAddresses.Contains(item.UnloadAddress))
                    CommonSiteAddresses.Add(item.UnloadAddress);

                if (!string.IsNullOrWhiteSpace(item.LoadRoute) && !CommonRoutes.Contains(item.LoadRoute))
                    CommonRoutes.Add(item.LoadRoute);
                if (!string.IsNullOrWhiteSpace(item.UnloadRoute) && !CommonRoutes.Contains(item.UnloadRoute))
                    CommonRoutes.Add(item.UnloadRoute);

                if (!string.IsNullOrWhiteSpace(item.LoadInstructions) && !CommonInstructions.Contains(item.LoadInstructions))
                    CommonInstructions.Add(item.LoadInstructions);
                if (!string.IsNullOrWhiteSpace(item.UnloadInstructions) && !CommonInstructions.Contains(item.UnloadInstructions))
                    CommonInstructions.Add(item.UnloadInstructions);
            }

            List<Address> knownAddresses;

            if (SelectedLoad.Job != null && SelectedLoad.Job.CareOfCompany != null)
                knownAddresses = (from a in Database.Addresses where a.Company == SelectedCompany || a.Company == SelectedLoad.Job.CareOfCompany select a).ToList();
            else
                knownAddresses = (from a in Database.Addresses where a.Company == SelectedCompany select a).ToList();
            
            foreach (var item in knownAddresses)
            {
                if (!CommonSiteAddresses.Contains(item.ToString()))
                    CommonSiteAddresses.Add(item.ToString());
            }
        }

        private static void ReindexCollection(ObservableCollection<LoadedCommodity> list)
        {
            int index = 1;

            foreach (var item in list)
            {
                item.OrderIndex = index;

                index++;
            }
        }

        private System.Collections.IEnumerable GetCompanyRates(Company company)
        {
            if (company == null)
            {
                return null;
            }

            var rates = from r in Database.Rates where r.Archived != true && r.RateType.Name == "Trailer" select r;
            var discount = company.RateAdjustment ?? 0.00m;
            var enterprise = company.CustomerType != null && company.CustomerType.IsEnterprise == true;

            foreach (var rate in rates)
            {
                if (enterprise)
                {
                    if (rate.HourlyEnterprise != null)
                    {
                        rate.Hourly = rate.HourlyEnterprise;
                        rate.Adjusted = rate.Hourly + discount;
                    }
                }
                else
                {
                    if (rate.HourlySpecialized != null)
                    {
                        rate.Hourly = rate.HourlySpecialized;
                        rate.Adjusted = rate.Hourly + discount;
                    }
                }
            }

            return rates;
        }

        #region DraggedItem

        /// <summary>
        /// DraggedItem Dependency Property
        /// </summary>
        public static readonly DependencyProperty DraggedItemProperty = DependencyProperty.Register("DraggedItem", typeof(LoadedCommodity), typeof(LoadCommoditiesControl));

        /// <summary>
        /// Gets or sets the DraggedItem property.  This dependency property 
        /// indicates ....
        /// </summary>
        public LoadedCommodity DraggedItem
        {
            get { return (LoadedCommodity)GetValue(DraggedItemProperty); }
            set { SetValue(DraggedItemProperty, value); }
        }

        #endregion

        #region edit mode monitoring

        /// <summary>
        /// State flag which indicates whether the grid is in edit
        /// mode or not.
        /// </summary>
        public bool IsEditing { get; set; }

        private void OnBeginEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            IsEditing = true;
            //in case we are in the middle of a drag/drop operation, cancel it...
            if (IsDragging) ResetDragDrop();
        }

        private void OnEndEdit(object sender, DataGridCellEditEndingEventArgs e)
        {
            IsEditing = false;
        }

        #endregion

        #region Drag and Drop Rows

        /// <summary>
        /// Keeps in mind whether
        /// </summary>
        public bool IsDragging { get; set; }

        /// <summary>
        /// Initiates a drag action if the grid is not in edit mode.
        /// </summary>
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsEditing) return;

            var row = UIHelpers.TryFindFromPoint<DataGridRow>((UIElement)sender, e.GetPosition(dgCommodities));
            if (row == null || row.IsEditing) return;

            //set flag that indicates we're capturing mouse movements
            IsDragging = true;
            DraggedItem = (LoadedCommodity)row.Item;
        }


        /// <summary>
        /// Completes a drag/drop operation.
        /// </summary>
        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsDragging || IsEditing)
            {
                return;
            }

            //get the target item
            var targetItem = (LoadedCommodity)dgCommodities.SelectedItem;

            if (targetItem == null || !ReferenceEquals(DraggedItem, targetItem))
            {
                var list = (ObservableCollection<LoadedCommodity>)dgCommodities.ItemsSource;

                //remove the source from the list
                list.Remove(DraggedItem);

                //get target index
                var targetIndex = list.IndexOf(targetItem);

                //move source at the target's location
                list.Insert(targetIndex, DraggedItem);

                //select the dropped item
                dgCommodities.SelectedItem = DraggedItem;

                ReindexCollection((ObservableCollection<LoadedCommodity>)dgCommodities.ItemsSource);
            }

            //reset
            ResetDragDrop();
        }


        /// <summary>
        /// Closes the popup and resets the
        /// grid to read-enabled mode.
        /// </summary>
        private void ResetDragDrop()
        {
            IsDragging = false;
            popup.IsOpen = false;
            dgCommodities.IsReadOnly = false;
        }


        /// <summary>
        /// Updates the popup's position in case of a drag/drop operation.
        /// </summary>
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!IsDragging || e.LeftButton != MouseButtonState.Pressed) return;

            //display the popup if it hasn't been opened yet
            if (!popup.IsOpen)
            {
                //switch to read-only mode
                dgCommodities.IsReadOnly = true;

                //make sure the popup is visible
                popup.IsOpen = true;
            }


            Size popupSize = new Size(popup.ActualWidth, popup.ActualHeight);
            popup.PlacementRectangle = new Rect(e.GetPosition(this), popupSize);

            //make sure the row under the grid is being selected
            Point position = e.GetPosition(dgCommodities);
            var row = UIHelpers.TryFindFromPoint<DataGridRow>(dgCommodities, position);
            if (row != null) dgCommodities.SelectedItem = row.Item;
        }

        #endregion

        private void cmbRates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbTrailerCombinations.ItemsSource = from tc in Database.TrailerCombinations where tc.Rate == cmbRates.SelectedItem select tc;
        }

        private void cmbCommodityName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedLoad == null) return;

            SelectedLoad.Notify("LoadedCommodities");
        }

        private void FillLocations_Click(object sender, RoutedEventArgs e)
        {
            var commodity = (LoadedCommodity)dgCommodities.SelectedItem;

            if (commodity == null || commodity.JobCommodity == null) return;

            commodity.LoadLocation = commodity.JobCommodity.DepartureSiteName;
            commodity.LoadAddress = commodity.JobCommodity.DepartureAddress;
            commodity.UnloadLocation = commodity.JobCommodity.ArrivalSiteName;
            commodity.UnloadAddress = commodity.JobCommodity.ArrivalAddress;

            if (commodity.JobCommodity.OriginalCommodity == null) return;

            commodity.LoadRoute = commodity.JobCommodity.OriginalCommodity.LastRoute;
            commodity.LoadInstructions = commodity.JobCommodity.OriginalCommodity.LastLoadInstructions;
        }

        private void AddContact_Click(object sender, RoutedEventArgs e)
        {
            var cmbContacts = (ComboBox)((Button)sender).DataContext;
            var cmbCompanies = (ComboBox)cmbContacts.DataContext;

            var company = (Company)cmbCompanies.SelectedItem;

            if (SelectedLoad == null && company != null) return;

            var window = new CreateContactWindow(Database, company, null) { Owner = Application.Current.MainWindow };
            var contact = window.CreateContact();

            if (contact == null || contact.Company == null) return;

            contact.Company.Contacts.Add(contact);
                        
            if ((Company)cmbLoadingSiteContactCompanies.SelectedItem == contact.Company)
                ((ObservableCollection<Contact>)cmbLoadingSiteContacts.ItemsSource).Add(contact);

            if ((Company)cmbUnloadingSiteContactCompanies.SelectedItem == contact.Company)
                ((ObservableCollection<Contact>)cmbUnloadingSiteContacts.ItemsSource).Add(contact);

            if ((Company)cmbLoadingContactCompanies.SelectedItem == contact.Company)
                ((ObservableCollection<Contact>)cmbLoadingContacts.ItemsSource).Add(contact);

            if ((Company)cmbUnloadingContactCompanies.SelectedItem == contact.Company)
                ((ObservableCollection<Contact>)cmbUnloadingContacts.ItemsSource).Add(contact);

            cmbContacts.SelectedItem = contact;
        }

        private void AddAddress_Click(object sender, RoutedEventArgs e)
        {
            var cmbAddresses = (ComboBox)((Button)sender).DataContext;
            var cmbCompanies = (ComboBox)cmbAddresses.DataContext;

            var company = (Company)cmbCompanies.SelectedItem;
            
            if (SelectedLoad == null && company != null) return;

            var window = new CreateAddressWindow(Database, company, null) { Owner = Application.Current.MainWindow };
            var address = window.CreateAddress();

            if (address == null || address.Company == null) return;

            address.Company.Addresses.Add(address);
                        
            if ((Company)cmbShipperCompanies.SelectedItem == address.Company)
                ((ObservableCollection<Address>)cmbShipperAddresses.ItemsSource).Add(address);

            if ((Company)cmbConsigneeCompanies.SelectedItem == address.Company)
                ((ObservableCollection<Address>)cmbConsigneeAddresses.ItemsSource).Add(address);
            
            cmbAddresses.SelectedItem = address;
        }

        private void AddCompany_Click(object sender, RoutedEventArgs e)
        {
            var cmbCompanies = (ComboBox)((Button)sender).DataContext;

            var window = new CreateCompanyWindow(Database) { Owner = Application.Current.MainWindow };
            var company = window.CreateCompany();

            if (company == null) return;

            try
            {
                Database.SubmitChanges();
                CompanyList.Add(company);

                cmbCompanies.SelectedItem = company;
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Error while adding company to database", ex.Message);
            }
        }        
    }
}
