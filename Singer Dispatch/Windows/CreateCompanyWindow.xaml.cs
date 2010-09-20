using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;


namespace SingerDispatch.Windows
{
    /// <summary>
    /// Interaction logic for CreateCompany.xaml
    /// </summary>
    public partial class CreateCompanyWindow
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
            
            Address = new Address();
            addressDetails.DataContext = Address;

            Company.CustomerType = (from ct in Database.CustomerType where ct.IsEnterprise == false select ct).First();            
        }

        public Company CreateCompany()
        {
            ShowDialog();

            return Company;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var addressTypes = from at in Database.AddressTypes select at;

            cmbProvinceOrState.ItemsSource = from ps in Database.ProvincesAndStates orderby ps.CountryID, ps.Name select ps;
            cmbAddressType.ItemsSource = addressTypes;

            Address.AddressType = addressTypes.First();
        }

        private void CreateCompanyHandler()
        {
            if (cmbProvinceOrState.SelectedItem != null)
            {
                Company.Addresses.Add(Address);
            }

            Database.Companies.InsertOnSubmit(Company);

            try
            {
                Database.SubmitChanges();
                Companies.Add(Company);

                Close();
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Error while building the database", ex.Message);
            }
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
            else if (e.Key == Key.Enter)
            {
                CreateCompanyHandler();
            }
        }        
    }
}
