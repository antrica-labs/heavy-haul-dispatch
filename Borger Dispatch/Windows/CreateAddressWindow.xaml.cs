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
using System.Windows.Shapes;

namespace SingerDispatch.Windows
{
    /// <summary>
    /// Interaction logic for CreateAddressWindow.xaml
    /// </summary>
    public partial class CreateAddressWindow : Window
    {
        private SingerDispatchDataContext _database;
        private Address _address;
        private Boolean _created;

        public CreateAddressWindow(SingerDispatchDataContext database, Company company, Company careOfCompany)
        {
            InitializeComponent();

            _created = false;
            _database = database;
            _address = new Address();

            var list = new List<Company>();

            list.Add(company);

            if (careOfCompany != null)
                list.Add(careOfCompany);

            cmbCompanies.ItemsSource = list;
            _address.Company = list.First();

            DataContext = _address;
        }

        public Address CreateAddress()
        {   
            ShowDialog();

            return _address;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var addressTypes = from at in _database.AddressTypes select at;

            cmbProvinceOrState.ItemsSource = from ps in _database.ProvincesAndStates orderby ps.CountryID, ps.Name select ps;
            cmbAddressType.ItemsSource = addressTypes;
        }

        private void CreateAddress_Click(object sender, RoutedEventArgs e)
        {
            _created = true;
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                _address = null;
                Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_created)
                _address = null;
        }
    }
}
