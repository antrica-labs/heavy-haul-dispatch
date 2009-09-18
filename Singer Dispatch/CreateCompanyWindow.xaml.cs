using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;


namespace SingerDispatch
{
    /// <summary>
    /// Interaction logic for CreateCompany.xaml
    /// </summary>
    public partial class CreateCompanyWindow : Window
    {
        private Company Company { get; set; }
        private Address Address { get; set; }
        private ObservableCollection<Company> Companies { get; set; }
        private SingerDispatchDataContext Database { get; set; }

        public CreateCompanyWindow(SingerDispatchDataContext database, ObservableCollection<Company> companies)
        {
            InitializeComponent();
                        
            Database = database;
            Companies = companies;
            
            Company = new Company();            
            companyDetails.DataContext = Company;

            Address = new Address() { Company = Company };
            addressDetails.DataContext = Address;
        }

        public Company CreateCompany()
        {
            ShowDialog();

            return Company;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cmbProvinceOrState.ItemsSource = (from ps in Database.ProvincesAndStates orderby ps.CountryID, ps.Name select ps).ToList();
            cmbAddressType.ItemsSource = (from at in Database.AddressTypes select at).ToList();
        }

        private void bttnCreateCompany_Click(object sender, RoutedEventArgs e)
        {
            Address.ProvinceStateID = ((ProvincesAndState)cmbProvinceOrState.SelectedItem).ID;

            Database.Companies.InsertOnSubmit(Company);
            Database.Addresses.InsertOnSubmit(Address);

            try
            {
                Database.SubmitChanges();
                Companies.Add(Company);

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }        
    }
}
