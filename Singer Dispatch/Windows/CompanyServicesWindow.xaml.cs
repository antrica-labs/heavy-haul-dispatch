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
using System.Collections.ObjectModel;

namespace SingerDispatch.Windows
{
    /// <summary>
    /// Interaction logic for CompanyServicesWindow.xaml
    /// </summary>
    public partial class CompanyServicesWindow : Window
    {
        private SingerDispatchDataContext _database;
        private Company _company;

        public CompanyServicesWindow(SingerDispatchDataContext database, Company company)
        {
            InitializeComponent();

            _database = database;
            _company = company;

            lbServices.ItemsSource = new ObservableCollection<CheckBox>();

            DataContext = _company;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshServiceList();
        }

        public List<Service> UpdateServices()
        {
            ShowDialog();

            return _company.Services.ToList();
        }
        
        private void RefreshServiceList()
        {
            var list = (ObservableCollection<CheckBox>)lbServices.ItemsSource;

            if (_company == null) return;

            var types = from t in _database.ServiceTypes select t;
            var selected = from s in _company.Services select s.ServiceType;

            list.Clear();

            foreach (var type in types)
            {
                var cb = new CheckBox { Content = type.Name, DataContext = type, IsChecked = selected.Contains(type) };

                cb.Checked += CheckBox_Checked;
                cb.Unchecked += CheckBox_Unchecked;

                list.Add(cb);
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox)sender;
            var serviceType = (ServiceType)cb.DataContext;

            if (_company == null || serviceType == null) return;

            var service = new Service { Company = _company, ServiceType = serviceType };

            _company.Services.Add(service);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox)sender;
            var serviceType = (ServiceType)cb.DataContext;

            if (_company == null || serviceType == null) return;

            var list = (from s in _company.Services where s.ServiceType == serviceType select s).ToList();

            foreach (var item in list)
            {
                _company.Services.Remove(item);
            }
        }

        private void UpdateServices_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
