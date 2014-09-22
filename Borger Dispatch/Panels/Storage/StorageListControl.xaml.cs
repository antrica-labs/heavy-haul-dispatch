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
using System.ComponentModel;
using System.Windows.Threading;

namespace SingerDispatch.Panels.Storage
{
    /// <summary>
    /// Interaction logic for StorageListControl.xaml
    /// </summary>
    public partial class StorageListControl
    {
        private BackgroundWorker MainGridWorker;
        private BackgroundWorker ArchiveGridWorker;

        public static DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(StorageItem), typeof(StorageListControl), new PropertyMetadata(null, SelectedItemPropertyChanged));        

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

            cmbBillingIntervals.ItemsSource = (from bi in Database.BillingIntervals select bi).ToList();

            MainGridWorker = new BackgroundWorker();
            MainGridWorker.WorkerSupportsCancellation = true;
            MainGridWorker.DoWork += FillMainGridAsync;

            ArchiveGridWorker = new BackgroundWorker();
            ArchiveGridWorker.WorkerSupportsCancellation = true;
            ArchiveGridWorker.DoWork += FillArchiveGridAsync;

            RegisterThread(MainGridWorker);
            RegisterThread(ArchiveGridWorker);
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode() || IsVisible == false) return;

            UpdateComboBoxes();

            if (currentOrPreviousTabs.SelectedIndex == 0)
                UpdateCurrentStorageList();
            else if (currentOrPreviousTabs.SelectedIndex == 1)
                UpdatePreviouslyStoredList();
            
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

                Database.RevertChanges();
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
            if (MainGridWorker.IsBusy)
                return;

            dgCurrentStorageItems.ItemsSource = new ObservableCollection<StorageItem>();
            MainGridWorker.RunWorkerAsync();
        }

        private void UpdatePreviouslyStoredList()
        {
            if (ArchiveGridWorker.IsBusy)
                return;
            
            dgPreviousStorageItems.ItemsSource = new ObservableCollection<StorageItem>();
            ArchiveGridWorker.RunWorkerAsync();            
        }

        private void SetDataGridAvailability(bool isAvailable)
        {
            dgCurrentStorageItems.IsEnabled = isAvailable;
            dgPreviousStorageItems.IsEnabled = isAvailable;
        }

        private void AddItemToMainGrid(StorageItem item)
        {
            var list = dgCurrentStorageItems.ItemsSource as ObservableCollection<StorageItem>;

            list.Add(item);
        }

        private void AddItemToArchiveGrid(StorageItem item)
        {
            var list = dgPreviousStorageItems.ItemsSource as ObservableCollection<StorageItem>;

            list.Add(item);
        }

        private void FillMainGridAsync(object sender, DoWorkEventArgs e)
        {
            var async = sender as BackgroundWorker;

            if (async.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            FillDataGridAsync(async, (ObservableCollection<StorageItem>)dgCurrentStorageItems.ItemsSource, false);

            if (async.CancellationPending)
                e.Cancel = true;
        }

        private void FillArchiveGridAsync(object sender, DoWorkEventArgs e)
        {
            var async = sender as BackgroundWorker;

            if (async.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            FillDataGridAsync(async, (ObservableCollection<StorageItem>)dgPreviousStorageItems.ItemsSource, true);

            if (async.CancellationPending)
                e.Cancel = true;
        }

        private void FillDataGridAsync(BackgroundWorker thread, ObservableCollection<StorageItem> list, bool archived)
        {
            if (thread.CancellationPending)
                return;

            Dispatcher.Invoke(DispatcherPriority.Render, new Action<bool>(SetDataGridAvailability), false);
                        
            var storage = from si in Database.StorageItems where si.Job != null orderby si.Number descending select si;

            if (archived)
                storage = from s in storage where s.JobCommodity != null && (s.DateRemoved != null && s.DateRemoved.Value.Date <= DateTime.Today.Date) orderby s.JobCommodity.Owner.Name select s;
            else
                storage = from s in storage where s.JobCommodity != null && (s.DateRemoved == null || s.DateRemoved.Value.Date > DateTime.Today.Date) orderby s.JobCommodity.Owner.Name select s;

            foreach (var item in storage)
            {
                if (thread.CancellationPending)
                    break;

                if (archived)
                    Dispatcher.Invoke(DispatcherPriority.Render, new Action<StorageItem>(AddItemToArchiveGrid), item);
                else
                    Dispatcher.Invoke(DispatcherPriority.Render, new Action<StorageItem>(AddItemToMainGrid), item);
            }

            Dispatcher.Invoke(DispatcherPriority.Render, new Action<bool>(SetDataGridAvailability), true);
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

            switch (tb.SelectedIndex)
            {
                case 0:
                    UpdateCurrentStorageList();
                    break;
                case 1:
                    UpdatePreviouslyStoredList();
                    break;
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

                Database.RevertChanges();
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
