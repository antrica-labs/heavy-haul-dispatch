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
        private ObservableCollection<Company> companies;
        private SingerDispatchDataContext database;

        public CreateCompanyWindow(SingerDispatchDataContext database, ObservableCollection<Company> companies)
        {
            InitializeComponent();

            this.company = null;
            this.database = database;
            this.companies = companies;
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
            company = new Company();
            Address address = new Address();            
            ProvincesAndState provinceOrState = (ProvincesAndState)cmbProvince.SelectedItem;

            company.Name = txtName.Text;
            company.OperatingAs = txtOperatingAs.Text;

            address.Line1 = txtAddress1.Text;
            address.Line2 = txtAddress2.Text;
            address.City = txtCity.Text;            
            address.PostalZip = txtPostalZip.Text;
            address.PrimaryPhone = txtSiteMainPhone.Text;
            address.SecondaryPhone = txtSiteSecondaryPhone.Text;
            address.Fax = txtSiteFax.Text;
            address.Notes = txtAddressNotes.Text;
            address.ProvinceStateID = provinceOrState.ID;
            address.Company = company;

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
