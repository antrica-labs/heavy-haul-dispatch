using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;


namespace SingerDispatch.Windows
{
    /// <summary>
    /// Interaction logic for CreateCompany.xaml
    /// </summary>
    public partial class CreateCompanyWindow
    {
        private Company Company { get; set; }
        private Address Address { get; set; }
        private Contact Contact { get; set; }        
        private SingerDispatchDataContext Database { get; set; }

        public CreateCompanyWindow(SingerDispatchDataContext database)
        {
            InitializeComponent();

            Database = database;
            
            Company = new Company();
            companyDetails.DataContext = Company;
            
            Address = new Address();
            addressDetails.DataContext = Address;

            Contact = new Contact();
            contactDetails.DataContext = Contact;

            lbContactTypes.ItemsSource = new ObservableCollection<CheckBox>();

            try
            {
                Company.CustomerType = (from ct in Database.CustomerType where ct.IsEnterprise == false select ct).First();
            }
            catch { }
        }

        public Company CreateCompany()
        {
            ShowDialog();

            return Company;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lbContactTypes.MaxHeight = lbContactTypes.ActualHeight;

            var addressTypes = from at in Database.AddressTypes select at;

            cmbProvinceOrState.ItemsSource = from ps in Database.ProvincesAndStates orderby ps.CountryID, ps.Name select ps;
            cmbAddressType.ItemsSource = addressTypes;
            cmbContactPreferedContactMethod.ItemsSource = from cm in Database.ContactMethods select cm;

            try
            {
                Address.AddressType = (from at in addressTypes where at.Name == "Head Office" select at).First();
            }
            catch { }

            var list = new ObservableCollection<CheckBox>();            

            foreach (var type in from ct in Database.ContactTypes select ct)
            {
                var cb = new CheckBox { Content = type.Name, DataContext = type, IsChecked = false };

                cb.Checked += ContactTypeCheckBox_Checked;
                cb.Unchecked += ContactTypeCheckBox_Unchecked;

                list.Add(cb);
            }

            lbContactTypes.ItemsSource = list;
        }

        private void ContactTypeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox)sender;
            var type = (ContactType)cb.DataContext;            

            var role = new ContactRoles { Contact = Contact, ContactType = type };

            Contact.ContactRoles.Add(role);
        }

        private void ContactTypeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox)sender;
            var type = (ContactType)cb.DataContext;

            foreach (var item in (from role in Contact.ContactRoles where role.ContactType == type select role).ToList())
            {
                Contact.ContactRoles.Remove(item);
            }
        }

        private void CreateCompanyHandler()
        {
            if (cmbProvinceOrState.SelectedItem != null)            
                Company.Addresses.Add(Address);

            if (!string.IsNullOrWhiteSpace(txtContactFirstName.Text))
                Company.Contacts.Add(Contact);

            Close();
        }

        private void CreateCompany_Click(object sender, RoutedEventArgs e)
        {
            CreateCompanyHandler();
        }        

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }        
    }
}
