using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SingerDispatch.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;

namespace SingerDispatch.Panels.Companies
{
    /// <summary>
    /// Interaction logic for AddressesAndContactsControl.xaml
    /// </summary>
    public partial class AddressesAndContactsControl
    {
        private CommandBinding SaveCommand { get; set; }

        public SingerDispatchDataContext Database { get; set; }

        public AddressesAndContactsControl()
        {
            InitializeComponent();

            SaveCommand = new CommandBinding(CustomCommands.GenericSaveCommand);
            CommandBindings.Add(SaveCommand);

            lbContactTypes.ItemsSource = new ObservableCollection<CheckBox>();

            if (InDesignMode()) return;

            Database = SingerConfigs.CommonDataContext;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {            
            SaveCommand.Executed += CommitChanges_Executed;

            if (InDesignMode()) return;

            lbContactTypes.MaxHeight = lbContactTypes.ActualHeight;

            var list = (ObservableCollection<CheckBox>)lbContactTypes.ItemsSource;

            list.Clear();
            
            cmbProvinceOrState.ItemsSource = from p in Database.ProvincesAndStates orderby p.CountryID, p.Name select p;
            cmbAddressType.ItemsSource = from at in Database.AddressTypes select at;
            cmbContactPreferedContactMethod.ItemsSource = from cm in Database.ContactMethods select cm;
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            if (newValue != null)
            {                
                dgAddresses.ItemsSource = new ObservableCollection<Address>(from a in Database.Addresses where a.Company == newValue select a);
                dgContacts.ItemsSource = new ObservableCollection<Contact>(from c in Database.Contacts where c.Company == newValue select c);
            }
            else
            {
                dgAddresses.ItemsSource = null;
                dgContacts.ItemsSource = null;
            }
        }

        private void ContactTypeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox) sender;
            var type = (ContactType) cb.DataContext;
            var contact = (Contact) dgContacts.SelectedItem;

            var role = new ContactRoles { Contact = contact, ContactType = type };

            contact.ContactRoles.Add(role);
        }

        private void ContactTypeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox)sender;
            var type = (ContactType)cb.DataContext;
            var contact = (Contact) dgContacts.SelectedItem;
            
            foreach (var item in (from role in contact.ContactRoles where role.ContactType == type select role).ToList())
            {
                contact.ContactRoles.Remove(item);
            }
        }

        private void Addresses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void Contact_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var control = (DataGrid) sender;
            var contact = (Contact) control.SelectedItem;
            var list = (ObservableCollection<CheckBox>)lbContactTypes.ItemsSource;

            list.Clear();

            if (contact == null) return;

            var selected = from types in contact.ContactRoles select types.ContactType;

            foreach (var type in from ct in Database.ContactTypes select ct)
            {
                var cb = new CheckBox { Content = type.Name, DataContext = type, IsChecked = selected.Contains(type) };

                cb.Checked += ContactTypeCheckBox_Checked;
                cb.Unchecked += ContactTypeCheckBox_Unchecked;

                list.Add(cb);
            }
        }
     
        private void RemoveAddress_Click(object sender, RoutedEventArgs e)
        {
            var selected = (Address)dgAddresses.SelectedItem;

            if (selected == null) return;

            var confirmation = MessageBox.Show("Are you sure you want to remove this address and all of its corresponding contacts?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;
           
            try
            {
                SelectedCompany.Addresses.Remove(selected);
                Database.SubmitChanges();

                ((ObservableCollection<Address>)dgAddresses.ItemsSource).Remove(selected);
            }
            catch (System.Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }

        private void RemoveContact_Click(object sender, RoutedEventArgs e)
        {
            var selected = (Contact)dgContacts.SelectedItem;

            if (selected == null) return;

            var confirmation = MessageBox.Show("Are you sure you want to remove this contact?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;
            
            try
            {
                SelectedCompany.Contacts.Remove(selected);
                Database.SubmitChanges();

                ((ObservableCollection<Contact>)dgContacts.ItemsSource).Remove(selected);
            }
            catch (System.Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }

        private void NewAddress_Click(object sender, RoutedEventArgs e)
        {
            var address = new Address();
            
            SelectedCompany.Addresses.Add(address);
            ((ObservableCollection<Address>)dgAddresses.ItemsSource).Add(address);
            dgAddresses.SelectedItem = address;
            dgAddresses.ScrollIntoView(address);

            txtAddress1.Focus();
        }

        private void NewContact_Click(object sender, RoutedEventArgs e)
        {
            var address = (Address)dgAddresses.SelectedItem;

            if (address == null)
            {
                MessageBox.Show("You must select an address from the address list before you can add or edit any contacts.");
                return;
            }

            var contact = new Contact();

            SelectedCompany.Contacts.Add(contact);
            ((ObservableCollection<Contact>)dgContacts.ItemsSource).Add(contact);
            dgContacts.SelectedItem = contact;
            dgContacts.ScrollIntoView(contact);

            txtContactFirstName.Focus();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ((ButtonBase)sender).Focus();

                Database.SubmitChanges();
            }
            catch (System.Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }

        private void CommitChanges_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CommitChangesButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, CommitChangesButton));
        }
    }
}
