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
    /// Interaction logic for CreateCommodityWindow.xaml
    /// </summary>
    public partial class CreateCommodityWindow : Window
    {
        private Commodity _commodity;
        private SingerDispatchDataContext _database;
        private Boolean _created;

        public CreateCommodityWindow(SingerDispatchDataContext database, Company company, Company careOfCompany)
        {
            InitializeComponent();

            _database = database;
            _commodity = new Commodity();
            _created = false;

            var list = new List<Company>();

            list.Add(company);

            if (careOfCompany != null)
                list.Add(careOfCompany);

            cmbCompanies.ItemsSource = list;
            _commodity.Company = list.First();

            DataContext = _commodity;
        }

        public Commodity CreateCommodity()
        {            
            ShowDialog();

            return _commodity;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void CreateCommodity_Click(object sender, RoutedEventArgs e)
        {
            _created = true;
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_created)
                _commodity = null;
        }
    }
}
