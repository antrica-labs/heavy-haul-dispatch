using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Windows.Controls;

namespace SingerDispatch.Panels.Companies
{
    /// <summary>
    /// Interaction logic for AddressesAndContactsControl.xaml
    /// </summary>
    public partial class AddressesAndContactsControl : CompanyUserControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public AddressesAndContactsControl()
        {
            InitializeComponent();
            this.Width = double.NaN;

            Database = SingerConstants.CommonDataContext;            
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            cmbContactPreferedContactMethod.ItemsSource = SingerConstants.ContactMethods;
            cmbProvinceOrState.ItemsSource = (from p in Database.ProvincesAndStates orderby p.CountryID, p.Name select p).ToList();
            cmbContactType.ItemsSource = (from ct in Database.ContactTypes select ct).ToList();
            cmbAddressType.ItemsSource = (from at in Database.AddressTypes select at).ToList();
        }


        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            if (newValue != null)
            {       
                dgAddresses.ItemsSource = new ObservableCollection<Address>(
                    (from a in Database.Addresses where a.CompanyID == newValue.ID select a).ToList()
                );
            }
            else
            {
                dgAddresses.ItemsSource = null;
            }                                                   
        }

        private void dgAddresses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var control = (DataGrid)sender;
            var address = (Address)control.SelectedItem;

            if (address != null)
            {
                dgContacts.ItemsSource = new ObservableCollection<Contact>(
                    (from c in Database.Contacts where c.AddressID == address.ID orderby c.LastName select c).ToList()
                );
            }
            else
            {
                dgContacts.ItemsSource = null;
            }
        }
     
        private void RemoveAddress_Click(object sender, RoutedEventArgs e)
        {
            var selected = (Address)dgAddresses.SelectedItem;

            if (selected == null)
            {
                return;
            }

            MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to remove this address and all of its coresponding contacts?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes)
            {
                return;
            }

            SelectedCompany.Addresses.Remove(selected);
            ((ObservableCollection<Address>)dgAddresses.ItemsSource).Remove(selected);

            Database.SubmitChanges();            
        }

        private void RemoveContact_Click(object sender, RoutedEventArgs e)
        {
            var selected = (Contact)dgContacts.SelectedItem;

            if (selected == null)
            {
                return;
            }

            MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to remove this contact?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes)
            {
                return;
            }

            selected.Address.Contacts.Remove(selected);
            ((ObservableCollection<Contact>)dgContacts.ItemsSource).Remove(selected);

            Database.SubmitChanges();
        }

        private void NewAddress_Click(object sender, RoutedEventArgs e)
        {
            var address = new Address();
            
            SelectedCompany.Addresses.Add(address);
            ((ObservableCollection<Address>)dgAddresses.ItemsSource).Add(address);
            dgAddresses.SelectedItem = address;

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
            ((ObservableCollection<Contact>)dgContacts.ItemsSource).Add(contact);
            dgContacts.SelectedItem = contact;

            txtContactFirstName.Focus();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {               
            Database.SubmitChanges();
        }
    }
}
