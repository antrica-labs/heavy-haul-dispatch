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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace SingerDispatch.Panels.Quotes
{
    /// <summary>
    /// Interaction logic for QuoteCommoditiesControl.xaml
    /// </summary>
    public partial class QuoteCommoditiesControl : QuoteUserControl
    {
        SingerDispatchDataContext database;

        public QuoteCommoditiesControl()
        {
            InitializeComponent();

            database = SingerConstants.CommonDataContext;
        }

        private void QuoteUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (SelectedQuote != null)
            {
                cmbCommodityName.ItemsSource = new ObservableCollection<Commodity>((from c in database.Commodities where c.CompanyID == SelectedQuote.CompanyID || c.CompanyID == SelectedQuote.CareOfCompanyID select c).ToList());
            }
        }

        protected override void SelectedQuoteChanged(Quote newValue, Quote oldValue)
        {
            base.SelectedQuoteChanged(newValue, oldValue);

            if (newValue != null)
            {
                cmbCommodityName.ItemsSource = new ObservableCollection<Commodity>((from c in database.Commodities where c.CompanyID == newValue.CompanyID || c.CompanyID == newValue.CareOfCompanyID select c).ToList());

                QuoteCommodity commodity = new QuoteCommodity() { QuoteID = newValue.ID };

                gbDetails.DataContext = commodity;
                gbDepature.DataContext = commodity;
                gbDestination.DataContext = commodity;
                                
                dgQuoteCommodities.ItemsSource = new ObservableCollection<QuoteCommodity>(newValue.QuoteCommodities);
            }
        }

        private void cmbCommodityName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Commodity original = (Commodity)cmbCommodityName.SelectedItem;
            QuoteCommodity commodity = (QuoteCommodity)dgQuoteCommodities.SelectedItem;

            if (commodity == null)
            {
                return;
            }

            if (original != null)
            {
                commodity.OriginalCommodityID = original.ID;
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
            }
            else
            {
                commodity.OriginalCommodityID = null;
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
            }
        }

        private void btnNewCommodity_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<QuoteCommodity> list = (ObservableCollection<QuoteCommodity>)dgQuoteCommodities.ItemsSource;
            QuoteCommodity commodity = new QuoteCommodity() { QuoteID = SelectedQuote.ID };

            SelectedQuote.QuoteCommodities.Add(commodity);
            list.Insert(0, commodity);
            dgQuoteCommodities.SelectedItem = commodity;

            cmbCommodityName.Focus();
        }

        private void dgQuoteCommodities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            QuoteCommodity commodity = (QuoteCommodity)dgQuoteCommodities.SelectedItem;

            if (commodity != null)
            {
                gbDetails.DataContext = commodity;
                gbDepature.DataContext = commodity;
                gbDestination.DataContext = commodity;
            }
        }

        private void btnRemoveCommodity_Click(object sender, RoutedEventArgs e)
        {
            QuoteCommodity commodity = (QuoteCommodity)dgQuoteCommodities.SelectedItem;

            if (commodity == null)
            {
                return;
            }

            MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to remove this commodity?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation == MessageBoxResult.Yes)
            {
                dgQuoteCommodities.SelectedItem = null;
                SelectedQuote.QuoteCommodities.Remove(commodity);
                ((ObservableCollection<QuoteCommodity>)dgQuoteCommodities.ItemsSource).Remove(commodity);
            }
        }
        
    }
}
