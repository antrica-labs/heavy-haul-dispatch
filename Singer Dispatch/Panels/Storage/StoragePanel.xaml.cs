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
using SingerDispatch.Controls;
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;
using SingerDispatch.Windows;
using SingerDispatch.Printing.Documents;
using SingerDispatch.Database;

namespace SingerDispatch.Panels.Storage
{
    /// <summary>
    /// Interaction logic for StoragePanel.xaml
    /// </summary>
    public partial class StoragePanel 
    {
        public static DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(StorageItem), typeof(StoragePanel), new PropertyMetadata(null, SelectedItemPropertyChanged));
        public SingerDispatchDataContext Database { get; set; }

        private CommandBinding SaveCommand { get; set; }

        public StorageItem SelectedItem
        {
            get
            {
                return (StorageItem)GetValue(SelectedItemProperty);               
            }
            set
            {
                SetValue(SelectedItemProperty, value);   
            }
        }

        public StoragePanel()
        {
            InitializeComponent();

            SaveCommand = new CommandBinding(CustomCommands.GenericSaveCommand);
            CommandBindings.Add(SaveCommand);

            if (InDesignMode()) return;
            // Work below can only be done when the real app is running. It fails during design time.

            Database = SingerConfigs.CommonDataContext;

            cmbBillingIntervals.ItemsSource = from bi in Database.BillingIntervals select bi;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode()) return;

            var storage = from si in Database.StorageItems orderby si.Number descending select si;

            dgCurrentStorageItems.ItemsSource = new ObservableCollection<StorageItem>(from s in storage where s.Archived != true select s);
            dgPreviousStorageItems.ItemsSource = new ObservableCollection<StorageItem>(from s in storage where s.Archived == true select s);

            UpdateComboBoxes();
        }

        protected static void SelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (StoragePanel)d;

            //control.SelectedItemChanged((StorageItem)e.NewValue, (StorageItem)e.OldValue);                
        }

        private void SelectedItemChanged(StorageItem newValue, StorageItem oldValue)
        {
            if (newValue == null)
            {
                dgCurrentStorageItems.SelectedItem = null;
                dgPreviousStorageItems.SelectedItem = null;
            }
            else if (newValue.Archived == true)
            {
                dgPreviousStorageItems.SelectedItem = newValue;
            }
            else
            {
                dgCurrentStorageItems.SelectedItem = newValue;
            }
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<StorageItem>)dgCurrentStorageItems.ItemsSource;
            var item = new StorageItem { DateEntered = DateTime.Now };

            list.Insert(0, item);
            Database.StorageItems.InsertOnSubmit(item);

            try
            {
                EntityHelper.SaveAsNewStorageItem(item, Database);

                dgCurrentStorageItems.ScrollIntoView(item);
                dgCurrentStorageItems.SelectedItem = item;
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<StorageItem>)dgPreviousStorageItems.ItemsSource;            

            if (SelectedItem == null) return;

            var confirmation = MessageBox.Show("Are you sure you want to permanently remove this storage item from the archive?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            Database.StorageItems.DeleteOnSubmit(SelectedItem);

            try
            {                
                Database.SubmitChanges();

                list.Remove(SelectedItem);
            }
            catch (System.Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }   

        private void Company_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateComboBoxes();
        }

        private void UpdateComboBoxes()
        {   
            var company = (Company)cmbCompanies.SelectedItem;

            if (company != null)
            {
                cmbCommodities.ItemsSource = new ObservableCollection<Commodity>(company.Commodities);
                cmbContacts.ItemsSource = new ObservableCollection<Contact>(company.Contacts);
            }
        }

        private void QuickAddCommodity_Click(object sender, RoutedEventArgs e)
        {
            var company = (Company)cmbCompanies.SelectedItem;
            var commodities = (ObservableCollection<Commodity>)cmbCommodities.ItemsSource;

            if (company == null) return;

            var window = new CreateCommodityWindow(Database, company, null) { Owner = Application.Current.MainWindow };
            var commodity = window.CreateCommodity();

            if (commodity == null) return;

            company.Commodities.Add(commodity);
            commodities.Add(commodity);

            cmbCommodities.SelectedItem = commodity;
        }

        private void QuickAddCompany_Click(object sender, RoutedEventArgs e)
        {
            var window = new CreateCompanyWindow(Database) { Owner = Application.Current.MainWindow };
            var company = window.CreateCompany();

            if (company == null) return;

            try
            {
                Database.Companies.InsertOnSubmit(company);
                Database.SubmitChanges();
                CompanyList.Add(company);

                cmbCompanies.SelectedItem = company;
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Error while adding company to database", ex.Message);
            }
        }

        private void AddContact_Click(object sender, RoutedEventArgs e)
        {
            var cmb = (ComboBox)((Button)sender).DataContext;

            if (SelectedItem == null || SelectedItem.Company == null) return;

            var window = new CreateContactWindow(Database, SelectedItem.Company, null) { Owner = Application.Current.MainWindow };
            var contact = window.CreateContact();

            if (contact == null || contact.Company == null) return;

            var list = (ObservableCollection<Contact>)cmb.ItemsSource;

            contact.Company.Contacts.Add(contact);
            list.Add(contact);

            cmb.SelectedItem = contact;
        }

        private void ViewStorageContract_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null || SelectedItem.Company == null || SelectedItem.Commodity == null)
            {
                Windows.ErrorNoticeWindow.ShowError("Unable to create storage contract", "Storage items need at least a company and commodity.");
                return;
            }

            var title = String.Format("Storage Contract #{0}", SelectedItem.Number);

            var viewer = new Windows.DocumentViewerWindow(new StorageContractDocument(), SelectedItem, title) { IsMetric = !UseImperialMeasurements, IsSpecializedDocument = SelectedItem.Company.CustomerType.IsEnterprise != true };
            viewer.DisplayPrintout();
        }

        private void dgCurrentStorageItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (StorageItem)dgCurrentStorageItems.SelectedItem;

            SelectedItem = null;
            SelectedItem = item;
        }

        private void dgPreviousStorageItems_SelectionChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var item = (StorageItem)dgPreviousStorageItems.SelectedItem;

            SelectedItem = null;
            SelectedItem = item;
        }

        private void ViewStorageSticker_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null || SelectedItem.Company == null || SelectedItem.Commodity == null)
            {
                Windows.ErrorNoticeWindow.ShowError("Unable to create storage contract", "Storage items need a commodity.");
                return;
            }

            var title = string.Format("Storage Sticker #{0}", SelectedItem.Number);

            var viewer = new Windows.DocumentViewerWindow(new StorageStickerDocument(), SelectedItem, title) { IsMetric = !UseImperialMeasurements, IsSpecializedDocument = SelectedItem.Commodity.Company.CustomerType.IsEnterprise != true };
            viewer.DisplayPrintout();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tb = (TabControl)sender;

            SelectedItem = null;

            if (tb.SelectedIndex == 0)
                SelectedItem = (StorageItem)dgCurrentStorageItems.SelectedItem;
            else if (tb.SelectedIndex == 1)
                SelectedItem = (StorageItem)dgPreviousStorageItems.SelectedItem;
        }

        private void ArchiveItem_Click(object sender, RoutedEventArgs e)
        {
            var clist = (ObservableCollection<StorageItem>)dgCurrentStorageItems.ItemsSource;
            var plist = (ObservableCollection<StorageItem>)dgPreviousStorageItems.ItemsSource;

            if (SelectedItem == null) return;

            var confirmation = MessageBox.Show("Are you sure you want to archive this storage item? This cannot be undone.", "Archive confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            SelectedItem.DateRemoved = SelectedItem.DateRemoved ?? DateTime.Now;
            SelectedItem.Archived = true;

            try
            {
                Database.SubmitChanges();

                clist.Remove(SelectedItem);
                //plist.Add(SelectedItem);
            }
            catch (System.Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }
    }
}
