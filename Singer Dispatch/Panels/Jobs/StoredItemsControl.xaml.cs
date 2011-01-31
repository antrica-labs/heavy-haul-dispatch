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
using SingerDispatch.Database;
using SingerDispatch.Windows;
using SingerDispatch.Printing.Documents;

namespace SingerDispatch.Panels.Jobs
{
    /// <summary>
    /// Interaction logic for StoredItemsControl.xaml
    /// </summary>
    public partial class StoredItemsControl
    {        
        public SingerDispatchDataContext Database { get; set; }

        public StoredItemsControl()
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

            if (SelectedJob == null) return;

            dgStorageItems.ItemsSource = new ObservableCollection<StorageItem>(from si in SelectedJob.StoredItems orderby si.Number select si);
            cmbCommodities.ItemsSource = (from c in SelectedJob.JobCommodities select c).ToList();
            cmbContacts.ItemsSource = new ObservableCollection<Contact>(from c in Database.Contacts where c.Company == SelectedCompany || c.Company == SelectedJob.CareOfCompany orderby c.FirstName, c.LastName select c);
        }
        
        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob == null) return;

            var list = (ObservableCollection<StorageItem>)dgStorageItems.ItemsSource;
            var item = new StorageItem { DateEntered = DateTime.Now, Job = SelectedJob };

            try
            {
                EntityHelper.SaveAsNewStorageItem(item, Database);

                SelectedJob.StoredItems.Add(item);
                list.Insert(0, item);
                dgStorageItems.SelectedItem = item;

                dgStorageItems.ScrollIntoView(item);
                dgStorageItems.SelectedItem = item;
            }
            catch (Exception ex)
            {
                NoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<StorageItem>)dgStorageItems.ItemsSource;
            var item = (StorageItem)dgStorageItems.SelectedItem;

            if (SelectedJob == null || item == null) return;

            var confirmation = MessageBox.Show(SingerConfigs.DefaultRemoveItemMessage, "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            SelectedJob.StoredItems.Remove(item);
            list.Remove(item);
        }
        
        private void AddContact_Click(object sender, RoutedEventArgs e)
        {
            var cmb = (ComboBox)((Button)sender).DataContext;

            if (SelectedJob == null) return;

            var window = new CreateContactWindow(Database, SelectedJob.Company, SelectedJob.CareOfCompany) { Owner = Application.Current.MainWindow };
            var contact = window.CreateContact();

            if (contact == null || contact.Company == null) return;

            var list = (ObservableCollection<Contact>)cmb.ItemsSource;

            contact.Company.Contacts.Add(contact);
            list.Add(contact);

            cmb.SelectedItem = contact;
        }

        private void ViewStorageContract_Click(object sender, RoutedEventArgs e)
        {
            var item = (StorageItem)dgStorageItems.SelectedItem;

            if (SelectedJob == null) return;

            if (item == null || item.JobCommodity == null)
            {
                Windows.NoticeWindow.ShowError("Unable to create storage contract", "Storage items need an associated commodity.");
                return;
            }

            var title = String.Format("Storage Contract #{0}", string.Format("{0} - {1}", SelectedJob.Number, item.Number));
            var specialized = SelectedJob.Company.CustomerType.IsEnterprise != true;
            
            var viewer = new Windows.DocumentViewerWindow(new StorageContractDocument(), item, title) { IsMetric = !UseImperialMeasurements, IsSpecializedDocument = specialized };
            viewer.DisplayPrintout();
        }

        private void ViewStorageSticker_Click(object sender, RoutedEventArgs e)
        {
            var item = (StorageItem)dgStorageItems.SelectedItem;

            if (SelectedJob == null) return;

            if (item == null || item.JobCommodity == null)
            {
                Windows.NoticeWindow.ShowError("Unable to create storage contract", "Storage items need a commodity.");
                return;
            }

            var owner = item.JobCommodity.Owner;

            if (owner == null)
            {
                owner = SelectedJob.CareOfCompany;
            }

            var title = string.Format("Storage Sticker #{0}", string.Format("{0} - {1}", SelectedJob.Number, item.Number));
            var specialized = (owner != null && owner.CustomerType.IsEnterprise != true);

            var viewer = new Windows.DocumentViewerWindow(new StorageStickerDocument(), item, title) { IsMetric = !UseImperialMeasurements, IsSpecializedDocument = specialized };
            viewer.DisplayPrintout();
        }

        private void ViewCombinedContract_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob == null) return;

            var title = String.Format("Storage Contract #{0}", string.Format("{0}", SelectedJob.Number));
            var specialized = SelectedJob.Company.CustomerType.IsEnterprise != true;

            var viewer = new Windows.DocumentViewerWindow(new StorageContractDocument(), SelectedJob, title) { IsMetric = !UseImperialMeasurements, IsSpecializedDocument = specialized };
            viewer.DisplayPrintout();
        }
    }
}
