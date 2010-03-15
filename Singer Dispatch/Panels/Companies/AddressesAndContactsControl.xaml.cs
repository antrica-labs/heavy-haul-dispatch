using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Windows.Controls;
using SingerDispatch.Controls;
using System.Windows.Input;

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

            Database = SingerConstants.CommonDataContext;

            SaveCommand = new CommandBinding(CustomCommands.GenericSaveCommand);
            CommandBindings.Add(SaveCommand);

            lbContactTypes.ItemsSource = new ObservableCollection<CheckBox>();
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {            
            SaveCommand.Executed += CommitChanges_Executed;

            var list = (ObservableCollection<CheckBox>)lbContactTypes.ItemsSource;

            list.Clear();

            cmbProvinceOrState.ItemsSource = from p in Database.ProvincesAndStates orderby p.CountryID, p.Name select p;
            cmbAddressType.ItemsSource = from at in Database.AddressTypes select at;
            cmbContactPreferedContactMethod.ItemsSource = from cm in Database.ContactMethods select cm;

            lbContactTypes.MaxHeight = lbContactTypes.ActualHeight;
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            if (newValue != null)
            {
                var addressQuery = from a in Database.Addresses where a.Company == newValue select a;
                var contactQuery = from c in Database.Contacts where addressQuery.Contains(c.Address) select c;

                dgAddresses.ItemsSource = new ObservableCollection<Address>(addressQuery);
                //dgContacts.ItemsSource = new ObservableCollection<Contact>(contactQuery);
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
            var control = (DataGrid)sender;
            var address = (Address)control.SelectedItem;

            dgContacts.ItemsSource = (address == null) ? null : new ObservableCollection<Contact>(from c in Database.Contacts where c.AddressID == address.ID orderby c.LastName select c);
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

            if (selected == null)
            {
                return;
            }

            var confirmation = MessageBox.Show("Are you sure you want to remove this address and all of its coresponding contacts?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes)
            {
                return;
            }

            SelectedCompany.Addresses.Remove(selected);
            ((ObservableCollection<Address>)dgAddresses.ItemsSource).Remove(selected);

            try
            {
                Database.SubmitChanges();
            }
            catch (System.Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void RemoveContact_Click(object sender, RoutedEventArgs e)
        {
            var selected = (Contact)dgContacts.SelectedItem;

            if (selected == null)
            {
                return;
            }

            var confirmation = MessageBox.Show("Are you sure you want to remove this contact?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes)
            {
                return;
            }

            selected.Address.Contacts.Remove(selected);
            ((ObservableCollection<Contact>)dgContacts.ItemsSource).Remove(selected);

            try
            {
                Database.SubmitChanges();
            }
            catch (System.Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void NewAddress_Click(object sender, RoutedEventArgs e)
        {
            var address = new Address();
            
            SelectedCompany.Addresses.Add(address);
            ((ObservableCollection<Address>)dgAddresses.ItemsSource).Insert(0, address);
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

            address.Contacts.Add(contact);
            ((ObservableCollection<Contact>)dgContacts.ItemsSource).Insert(0, contact);
            dgContacts.SelectedItem = contact;
            dgContacts.ScrollIntoView(contact);

            txtContactFirstName.Focus();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Database.SubmitChanges();
            }
            catch (System.Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void CommitChanges_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                Database.SubmitChanges();
            }
            catch (System.Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }
    }
}
