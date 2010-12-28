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

namespace SingerDispatch.Panels.Storage
{
    /// <summary>
    /// Interaction logic for StoragePanel.xaml
    /// </summary>
    public partial class StoragePanel 
    {
        public SingerDispatchDataContext Database { get; set; }

        private CommandBinding SaveCommand { get; set; }

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
            SaveCommand.Executed += CommitJobChanges_Executed;

            if (InDesignMode()) return;
                        
            dgStorageItems.ItemsSource = new ObservableCollection<StorageItem>(from si in Database.StorageItems where si.IsVisible == true select si);
            UpdateComboBoxes();
        }
        
        private void CommitJobChanges_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CommitChangesButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, CommitChangesButton));
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<StorageItem>)dgStorageItems.ItemsSource;
            var item = new StorageItem { DateEntered = DateTime.Now };

            list.Add(item);
            Database.StorageItems.InsertOnSubmit(item);

            dgStorageItems.ScrollIntoView(item);
            dgStorageItems.SelectedItem = item;
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<StorageItem>)dgStorageItems.ItemsSource;
            var item = (StorageItem)dgStorageItems.SelectedItem;

            if (item == null) return;

            item.DateRemoved = item.DateRemoved ?? DateTime.Now;
            item.IsVisible = false;

            list.Remove(item);

            try
            {                
                Database.SubmitChanges();
            }
            catch (System.Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }

        private void CommitChangesButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (ButtonBase)sender;

            try
            {
                button.Focus();

                Database.SubmitChanges();

                button.IsEnabled = false;
            }
            catch (System.Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }

        private void CommitChangesButton_LostFocus(object sender, RoutedEventArgs e)
        {
            var button = (ButtonBase)sender;

            button.IsEnabled = true;
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

            var window = new CreateCommodityWindow(Database) { Owner = Application.Current.MainWindow };
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
            var item = (StorageItem)dgStorageItems.SelectedItem;

            if (item != null || item.Company == null) return;

            var window = new CreateContactWindow(Database, item.Company, null) { Owner = Application.Current.MainWindow };
            var contact = window.CreateContact();

            if (contact == null || contact.Company == null) return;

            var list = (ObservableCollection<Contact>)cmb.ItemsSource;

            contact.Company.Contacts.Add(contact);
            list.Add(contact);

            cmb.SelectedItem = contact;
        }
    }
}
