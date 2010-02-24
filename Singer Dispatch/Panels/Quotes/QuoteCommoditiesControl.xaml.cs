using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace SingerDispatch.Panels.Quotes
{
    /// <summary>
    /// Interaction logic for QuoteCommoditiesControl.xaml
    /// </summary>
    public partial class QuoteCommoditiesControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public QuoteCommoditiesControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            cmbCommodityName.ItemsSource = (SelectedQuote == null) ? null : from c in Database.Commodities where c.Company == SelectedCompany || c.Company == SelectedQuote.CareOfCompany select c;
        }

        protected override void SelectedQuoteChanged(Quote newValue, Quote oldValue)
        {
            base.SelectedQuoteChanged(newValue, oldValue);

            dgQuoteCommodities.ItemsSource = (newValue == null) ? null : new ObservableCollection<QuoteCommodity>(newValue.QuoteCommodities);
        }

        private void CommodityName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var original = (Commodity)cmbCommodityName.SelectedItem;
            var commodity = (QuoteCommodity)dgQuoteCommodities.SelectedItem;

            if (commodity == null || commodity.OriginalCommodity == original) return;

            if (original != null)
            {
                commodity.OriginalCommodity = original;
                commodity.Name = original.Name;
                commodity.Value = original.Value;
                commodity.Serial = original.Serial;
                commodity.Unit = original.Unit;
                commodity.Owner = original.Owner;
                commodity.Length = original.Length;
                commodity.Width = original.Width;
                commodity.Height = original.Height;
                commodity.Weight = original.Weight;
                commodity.SizeEstimated = original.SizeEstimated;
                commodity.WeightEstimated = original.WeightEstimated;
                commodity.Notes = original.Notes;
                commodity.LastAddress = original.LastAddress;
                commodity.LastLocation = original.LastLocation;
                commodity.DepartureAddress = original.LastAddress;
                commodity.DepartureSiteName = original.LastLocation;
            }
            else
            {
                commodity.OriginalCommodity = null;
                commodity.Name = null;
                commodity.Value = null;
                commodity.Serial = null;
                commodity.Unit = null;
                commodity.Owner = null;
                commodity.Length = null;
                commodity.Width = null;
                commodity.Height = null;
                commodity.Weight = null;
                commodity.SizeEstimated = null;
                commodity.WeightEstimated = null;
                commodity.Notes = null;
                commodity.LastAddress = null;
                commodity.LastLocation = null;
                commodity.DepartureAddress = null;
                commodity.DepartureSiteName = null;
            }
        }

        private void NewCommodity_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<QuoteCommodity>)dgQuoteCommodities.ItemsSource;
            var commodity = new QuoteCommodity { QuoteID = SelectedQuote.ID };

            SelectedQuote.QuoteCommodities.Add(commodity);
            list.Add(commodity);            
            dgQuoteCommodities.SelectedItem = commodity;
            dgQuoteCommodities.ScrollIntoView(commodity);                

            cmbCommodityName.Focus();
        }

        private void DuplicateCommodity_Click(object sender, RoutedEventArgs e)
        {
            var commodity = (QuoteCommodity)dgQuoteCommodities.SelectedItem;

            if (commodity == null)
                return;

            commodity = commodity.Duplicate();

            SelectedQuote.QuoteCommodities.Add(commodity);
            ((ObservableCollection<QuoteCommodity>)dgQuoteCommodities.ItemsSource).Insert(0, commodity);
        }

        private void RemoveCommodity_Click(object sender, RoutedEventArgs e)
        {
            var commodity = (QuoteCommodity)dgQuoteCommodities.SelectedItem;

            if (commodity == null)
            {
                return;
            }

            MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to remove this commodity?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes)
            {
                return;
            }

            dgQuoteCommodities.SelectedItem = null;
            SelectedQuote.QuoteCommodities.Remove(commodity);
            ((ObservableCollection<QuoteCommodity>)dgQuoteCommodities.ItemsSource).Remove(commodity);
        }

        
        
    }
}
