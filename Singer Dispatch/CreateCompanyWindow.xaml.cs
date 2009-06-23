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
        private Company company;
        private Address address;
        private ObservableCollection<Company> companies;
        private SingerDispatchDataContext database;

        public CreateCompanyWindow(SingerDispatchDataContext database, ObservableCollection<Company> companies)
        {
            InitializeComponent();
                        
            this.database = database;
            this.companies = companies;

            this.company = new Company();            
            this.companyDetails.DataContext = company;

            this.address = new Address() { Company = company };
            this.addressDetails.DataContext = address;
        }

        public Company CreateCompany()
        {
            this.ShowDialog();

            return company;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SingerDispatchDataContext db = new SingerDispatchDataContext();
            var provinces = from p in db.ProvincesAndStates orderby p.CountryID, p.Name select p;
            
            cmbProvince.ItemsSource = provinces.ToList();
        }

        private void bttnCreateCompany_Click(object sender, RoutedEventArgs e)
        {
            address.ProvinceStateID = ((ProvincesAndState)cmbProvince.SelectedItem).ID;

            database.Companies.InsertOnSubmit(company);
            database.Addresses.InsertOnSubmit(address);

            try
            {
                database.SubmitChanges();
                companies.Add(company);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }        
    }
}
