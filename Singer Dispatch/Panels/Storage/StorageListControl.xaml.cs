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
using SingerDispatch.Printing.Documents;
using SingerDispatch.Windows;

namespace SingerDispatch.Panels.Storage
{
    /// <summary>
    /// Interaction logic for StorageListControl.xaml
    /// </summary>
    public partial class StorageListControl
    {
        public static DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(StorageItem), typeof(StorageListControl), new PropertyMetadata(null, SelectedItemPropertyChanged));
        public SingerDispatchDataContext Database { get; set; }

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

        public StorageListControl()
        {
            InitializeComponent();

            if (InDesignMode()) return;
            // Work below can only be done when the real app is running. It fails during design time.

            Database = SingerConfigs.CommonDataContext;

            cmbBillingIntervals.ItemsSource = from bi in Database.BillingIntervals select bi;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode() || IsVisible == false) return;

            if (currentOrPreviousTabs.SelectedIndex == 0)
                UpdateCurrentStorageList();
            else if (currentOrPreviousTabs.SelectedIndex == 1)
                UpdatePreviouslyStoredList();

            UpdateComboBoxes();
        }
        
        protected static void SelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (StorageListControl)d;

            control.SelectedItemChanged((StorageItem)e.NewValue, (StorageItem)e.OldValue);                
        }

        private void SelectedItemChanged(StorageItem newValue, StorageItem oldValue)
        {           
            UpdateComboBoxes();
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
                Windows.NoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }

        private void Company_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateComboBoxes();
        }

        private void UpdateComboBoxes()
        {
            var selected = cmbContacts.SelectedItem;
            
            cmbContacts.ItemsSource = (SelectedItem == null) ? null : new ObservableCollection<Contact>(from c in Database.Contacts where c.Company != null && c.Company == SelectedItem.Job.Company || c.Company == SelectedItem.Job.CareOfCompany orderby c.FirstName, c.LastName select c);
            cmbContacts.SelectedItem = selected;
        }                
        
        private void UpdateCurrentStorageList()
        {
            var storage = from si in Database.StorageItems orderby si.Number descending select si;
            var query = from s in storage where s.JobCommodity != null && (s.DateRemoved == null || s.DateRemoved.Value.Date > DateTime.Today.Date) orderby s.JobCommodity.Owner.Name select s;

            dgCurrentStorageItems.ItemsSource = new ObservableCollection<StorageItem>(query);
        }

        private void UpdatePreviouslyStoredList()
        {
            var storage = from si in Database.StorageItems orderby si.Number descending select si;
            var query = from s in storage where s.JobCommodity != null && (s.DateRemoved != null && s.DateRemoved.Value.Date <= DateTime.Today.Date) orderby s.JobCommodity.Owner.Name select s;

            dgPreviousStorageItems.ItemsSource = new ObservableCollection<StorageItem>(query);
        }

        private void AddContact_Click(object sender, RoutedEventArgs e)
        {
            var cmb = (ComboBox)((Button)sender).DataContext;

            if (SelectedItem == null || SelectedItem.Job.Company == null) return;

            var window = new CreateContactWindow(Database, SelectedItem.Job.Company, null) { Owner = Application.Current.MainWindow };
            var contact = window.CreateContact();

            if (contact == null || contact.Company == null) return;

            var list = (ObservableCollection<Contact>)cmb.ItemsSource;

            contact.Company.Contacts.Add(contact);
            list.Add(contact);

            cmb.SelectedItem = contact;
        }

        private void ViewStorageContract_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null || SelectedItem.Job.Company == null || SelectedItem.JobCommodity == null)
            {
                Windows.NoticeWindow.ShowError("Unable to create storage contract", "Storage items need at least a company and commodity.");
                return;
            }

            var title = String.Format("Storage Contract #{0}", SelectedItem.Number);
            var specialized = (SelectedItem.Job.Company.CustomerType != null) ? SelectedItem.Job.Company.CustomerType.IsEnterprise != true : true;
            var viewer = new Windows.DocumentViewerWindow(new StorageContractDocument(), SelectedItem, title) { IsMetric = !UseImperialMeasurements, IsSpecializedDocument = specialized };
            viewer.DisplayPrintout();
        }

        private void StorageItemGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var grid = (DataGrid)sender;
            var item = (StorageItem)grid.SelectedItem;

            SelectedItem = null;
            SelectedItem = item;

            e.Handled = true;
        }

        private void ViewStorageSticker_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null || SelectedItem.Job.Company == null || SelectedItem.JobCommodity == null)
            {
                Windows.NoticeWindow.ShowError("Unable to create storage contract", "Storage items need a commodity.");
                return;
            }

            var title = string.Format("Storage Sticker #{0}", SelectedItem.Number);
            var specialized = (SelectedItem.Job.Company.CustomerType != null) ? SelectedItem.Job.Company.CustomerType.IsEnterprise != true : true;
            var viewer = new Windows.DocumentViewerWindow(new StorageStickerDocument(), SelectedItem, title) { IsMetric = !UseImperialMeasurements, IsSpecializedDocument = specialized };
            viewer.DisplayPrintout();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tb = (TabControl)sender;

            SelectedItem = null;

            if (tb.SelectedIndex == 0)
            {
                UpdateCurrentStorageList();
                SelectedItem = (StorageItem)dgCurrentStorageItems.SelectedItem;
            }
            else if (tb.SelectedIndex == 1)
            {
                UpdatePreviouslyStoredList();
                SelectedItem = (StorageItem)dgPreviousStorageItems.SelectedItem;
            }                
        }

        private void ArchiveItem_Click(object sender, RoutedEventArgs e)
        {
            var clist = (ObservableCollection<StorageItem>)dgCurrentStorageItems.ItemsSource;

            if (SelectedItem == null) return;

            var confirmation = MessageBox.Show("Are you sure you want to archive this storage item? This will set the removed date if it is not already.", "Archive confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            SelectedItem.DateRemoved = SelectedItem.DateRemoved ?? DateTime.Now;            

            try
            {
                Database.SubmitChanges();

                clist.Remove(SelectedItem);
            }
            catch (System.Exception ex)
            {
                Windows.NoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }

        private void PrintCurrentStorage_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<StorageItem>)dgCurrentStorageItems.ItemsSource;
            var items = from i in list select i;
            var title = "Singer Storage List - Current";

            var viewer = new DocumentViewerWindow(new StorageListDocument(), items, title) { IsMetric = !UseImperialMeasurements };
            viewer.DisplayPrintout();            
        }

        private void PrintPreviousStorage_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<StorageItem>)dgPreviousStorageItems.ItemsSource;
            var items = from i in list select i;
            var title = "Singer Storage List - Archive";

            var viewer = new DocumentViewerWindow(new StorageListDocument(), items, title) { IsMetric = !UseImperialMeasurements };
            viewer.DisplayPrintout();     
        }
    }
}
