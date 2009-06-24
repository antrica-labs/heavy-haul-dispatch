using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Windows.Controls;

namespace SingerDispatch.Panels.Companies
{
    /// <summary>
    /// Interaction logic for AddressesAndContactsControl.xaml
    /// </summary>
    public partial class AddressesAndContactsControl : CompanyUserControl
    {       
        private SingerDispatchDataContext database;

        public AddressesAndContactsControl()
        {
            InitializeComponent();
            this.Width = double.NaN;

            database = SingerConstants.CommonDataContext;            
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            cmbContactPreferedContactMethod.ItemsSource = SingerConstants.ContactMethods;
            cmbProvinceOrState.ItemsSource = (from p in database.ProvincesAndStates orderby p.CountryID, p.Name select p).ToList();
            cmbContactType.ItemsSource = (from ct in database.ContactTypes select ct).ToList();
            cmbAddressType.ItemsSource = (from at in database.AddressTypes select at).ToList();
        }


        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            Company company = newValue;

            if (company != null)
            {       
                dgAddresses.ItemsSource = new ObservableCollection<Address>(
                    (from a in database.Addresses where a.CompanyID == company.ID select a).ToList()
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
                    (from c in database.Contacts where c.AddressID == address.ID orderby c.LastName select c).ToList()
                );
            }
            else
            {
                dgContacts.ItemsSource = null;
            }
        }
     
        private void btnRemoveAddress_Click(object sender, RoutedEventArgs e)
        {
            Address selected = (Address)dgAddresses.SelectedItem;

            if (selected == null)
            {
                return;
            }

            MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to remove this address and all of its coresponding contacts?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation == MessageBoxResult.Yes)
            {
                database.Addresses.DeleteOnSubmit(selected);
                ((ObservableCollection<Address>)dgAddresses.ItemsSource).Remove(selected);

                database.SubmitChanges();
            }
        }

        private void btnRemoveContact_Click(object sender, RoutedEventArgs e)
        {
            Contact selected = (Contact)dgContacts.SelectedItem;

            if (selected == null)
            {
                return;
            }

            MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to remove this contact?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation == MessageBoxResult.Yes)
            {
                database.Contacts.DeleteOnSubmit(selected);
                ((ObservableCollection<Contact>)dgContacts.ItemsSource).Remove(selected);

                database.SubmitChanges();
            }
        }

        private void btnNewAddress_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCompany == null)
            {
                MessageBox.Show("You must select a company from the company list before you can add or edit any addresses.");
                return;
            }

            dgAddresses.SelectedItem = null;
            txtAddress1.Focus();            
        }

        private void btnNewContact_Click(object sender, RoutedEventArgs e)
        {
            if (dgAddresses.SelectedItem == null)
            {
                MessageBox.Show("You must select an address from the address list before you can add or edit any contacts.");
                return;
            }

            dgContacts.SelectedItem = null;
            txtContactFirstName.Focus();
        }

        private void bttnSaveAddress_Click(object sender, RoutedEventArgs e)
        {
            Company company = SelectedCompany;
            Address address = (Address)dgAddresses.SelectedItem;

            if (company == null)
            {
                MessageBox.Show("You must select a company from the company list before you can add or edit any addresses.");
                return;
            }

            if (address == null)
            {
                AddNewAddress(company);
            }

            database.SubmitChanges();
        }
        
        private void bttnSaveContact_Click(object sender, RoutedEventArgs e)
        {
            Address address = (Address)dgAddresses.SelectedItem;
            Contact contact = (Contact)dgContacts.SelectedItem;

            if (address == null)
            {
                MessageBox.Show("You must select an address from the address list before you can add or edit any contacts.");
                return;
            }

            if (contact == null)
            {
                AddNewContact(address);
            }            

            database.SubmitChanges();            
        }

        private void AddNewContact(Address address)
        {
            Contact contact = new Contact();
            contact.AddressID = address.ID;
            contact.FirstName = txtContactFirstName.Text;
            contact.LastName = txtContactLastName.Text;
            contact.Email = txtContactEmail.Text;
            contact.PrimaryPhone = txtContactPrimaryPhone.Text;
            contact.SecondaryPhone = txtContactSecondaryPhone.Text;
            contact.ContactType = (ContactType)cmbContactType.SelectedItem;            
            contact.PreferedContactMethod = (string)cmbContactPreferedContactMethod.SelectedItem;
            contact.Notes = txtContactNotes.Text;

            database.Contacts.InsertOnSubmit(contact);
            ((ObservableCollection<Contact>)dgContacts.ItemsSource).Add(contact);
            dgContacts.SelectedItem = contact;
        }
        
        private void AddNewAddress(Company company)
        {
            ProvincesAndState provinceOrState = (ProvincesAndState)cmbProvinceOrState.SelectedItem;

            Address address = new Address();
            address.CompanyID = company.ID;
            address.Line1 = txtAddress1.Text;
            address.Line2 = txtAddress2.Text;
            address.City = txtCity.Text;
            address.PostalZip = txtPostalZip.Text;
            address.PrimaryPhone = txtSiteMainPhone.Text;
            address.SecondaryPhone = txtSiteSecondaryPhone.Text;
            address.Fax = txtSiteFax.Text;
            address.Notes = txtAddressNotes.Text;
            address.ProvinceStateID = provinceOrState.ID;
            address.AddressType = (AddressType)cmbAddressType.SelectedItem;

            database.Addresses.InsertOnSubmit(address);
            ((ObservableCollection<Address>)dgAddresses.ItemsSource).Add(address);
            dgAddresses.SelectedItem = address;
        }

        private void DataGridCommit(object sender, DataGridRowEditEndingEventArgs e)
        {
            database.SubmitChanges();
        }
    }
}
