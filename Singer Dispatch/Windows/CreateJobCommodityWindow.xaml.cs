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
    /// Interaction logic for CreateJobCommodityWindow.xaml
    /// </summary>
    public partial class CreateJobCommodityWindow : Window
    {
        private JobCommodity _commodity;
        private SingerDispatchDataContext _database;
        private Boolean _created;
        private IEnumerable<Commodity> _recordedCommodities;
        
        public CreateJobCommodityWindow(SingerDispatchDataContext database, Company company, Company careOfCompany)
        {
            InitializeComponent();

            _database = database;
            _commodity = new JobCommodity();
            _created = false;

            var list = new List<Company>();

            list.Add(company);

            if (careOfCompany != null)
                list.Add(careOfCompany);

            cmbOwners.ItemsSource = list;
            _commodity.Owner = list.First();

            if (careOfCompany != null)
                _recordedCommodities = new ObservableCollection<Commodity>(from c in _database.Commodities where c.Company == company || c.Company == careOfCompany orderby c.Name, c.Unit select c);
            else
                _recordedCommodities = new ObservableCollection<Commodity>(from c in _database.Commodities where c.Company == company orderby c.Name, c.Unit select c);
            
            DataContext = _commodity;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (dgRecordedCommodities.ActualHeight > 0.0)
            {
                dgRecordedCommodities.MaxHeight = dgRecordedCommodities.ActualHeight;
                dgRecordedCommodities.ItemsSource = _recordedCommodities;
            }
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

        private void CreateCommodity_Click(object sender, RoutedEventArgs e)
        {
            _created = true;
            Close();
        }

        private void dgRecordedCommodities_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           _commodity.OriginalCommodity = null;
        }

        private void dgRecordedCommodities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgRecordedCommodities.SelectedItem != null)
                dgRecordedCommodities.ScrollIntoView(dgRecordedCommodities.SelectedItem);
        }

        private void AddRecordedCommodity_Click(object sender, RoutedEventArgs e)
        {
            var confirmation = MessageBox.Show("Are you sure you want to add this job commodity as a recorded commondity? If so, please ensure that this commodity does not already exist", "Add confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            var recorded = _commodity.ToRecordedCommodity();

            _commodity.Owner.Commodities.Add(recorded);
            ((ObservableCollection<Commodity>)dgRecordedCommodities.ItemsSource).Add(recorded);
            dgRecordedCommodities.ScrollIntoView(recorded);
            dgRecordedCommodities.SelectedItem = recorded;
        }

        public JobCommodity CreateCommodity()
        {
            ShowDialog();

            return _commodity;
        }
    }
}
