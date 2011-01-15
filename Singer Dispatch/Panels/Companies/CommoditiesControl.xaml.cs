using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using SingerDispatch.Controls;
using System.Windows.Media;
using System.Windows.Controls;

namespace SingerDispatch.Panels.Companies
{
    /// <summary>
    /// Interaction logic for CommoditiesControl.xaml
    /// </summary>
    public partial class CommoditiesControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public CommoditiesControl()
        {
            InitializeComponent();

            if (InDesignMode()) return;

            Database = SingerConfigs.CommonDataContext;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            dgCommodities.ItemsSource = (SelectedCompany == null) ? null : new ObservableCollection<Commodity>(from c in Database.Commodities where c.Company == SelectedCompany orderby c.Name, c.Unit select c);
        }
        
        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);            
        }

        private void NewCommodity_Click(object sender, RoutedEventArgs e)
        {
            var commodity = new Commodity();

            SelectedCompany.Commodities.Add(commodity);
            ((ObservableCollection<Commodity>)dgCommodities.ItemsSource).Add(commodity);
            dgCommodities.SelectedItem = commodity;
            dgCommodities.ScrollIntoView(commodity);

            txtCommodityName.Focus();            
        }

        private void DuplicatedCommodity_Click(object sender, RoutedEventArgs e)
        {
            var commodity = (Commodity)dgCommodities.SelectedItem;
            var list = (ObservableCollection<Commodity>)dgCommodities.ItemsSource;

            if (commodity == null)
                return;

            commodity = commodity.Duplicate();

            SelectedCompany.Commodities.Add(commodity);
            list.Add(commodity);
            dgCommodities.SelectedItem = commodity;
            dgCommodities.ScrollIntoView(commodity);
        }

        private void DeleteCommodity_Click(object sender, RoutedEventArgs e)
        {
            var selected = (Commodity)dgCommodities.SelectedItem;

            if (selected == null) return;

            var confirmation = MessageBox.Show("Are you sure you want to remove this commodity and all of it's history?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            try
            {
                SelectedCompany.Commodities.Remove(selected);
                Database.SubmitChanges();

                ((ObservableCollection<Commodity>)dgCommodities.ItemsSource).Remove(selected);
            }
            catch (System.Exception ex)
            {
                Windows.NoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }
        
    }
}
