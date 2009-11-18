using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace SingerDispatch.Panels.Companies
{
    /// <summary>
    /// Interaction logic for CommoditiesControl.xaml
    /// </summary>
    public partial class CommoditiesControl : CompanyUserControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public CommoditiesControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            if (newValue != null)
            {
                dgCommodities.ItemsSource = new ObservableCollection<Commodity>(
                    (from c in Database.Commodities where c.CompanyID == newValue.ID select c).ToList()
                );
            }
            else
            {
                dgCommodities.ItemsSource = null;
            }
        }

        private void NewCommodity_Click(object sender, RoutedEventArgs e)
        {
            var commodity = new Commodity();

            SelectedCompany.Commodities.Add(commodity);
            ((ObservableCollection<Commodity>)dgCommodities.ItemsSource).Add(commodity);
            dgCommodities.SelectedItem = commodity;

            txtCommodityName.Focus();            
        }

        private void DeleteCommodity_Click(object sender, RoutedEventArgs e)
        {
            var selected = (Commodity)dgCommodities.SelectedItem;

            if (selected == null)
            {                
                return;
            }

            MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to remove this commodity and all of it's history?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation == MessageBoxResult.Yes)
            {                
                SelectedCompany.Commodities.Remove(selected);
                ((ObservableCollection<Commodity>)dgCommodities.ItemsSource).Remove(selected);

                Database.SubmitChanges();
            }
        }

        private void SaveCommodity_Click(object sender, RoutedEventArgs e)
        {
            Database.SubmitChanges();           
        }

        private void CreateNewCommodity(Company company)
        {
            var commodity = new Commodity();
            
            SelectedCompany.Commodities.Add(commodity);            
            ((ObservableCollection<Commodity>)dgCommodities.ItemsSource).Add(commodity);

            dgCommodities.SelectedItem = commodity;
        }

        private void DataGridCommit(object sender, Microsoft.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {
            Database.SubmitChanges();
        }
    }
}
