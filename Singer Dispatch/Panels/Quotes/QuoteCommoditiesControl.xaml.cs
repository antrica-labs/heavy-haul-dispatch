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

            if (original != null)
            {
                QuoteCommodity commodity = (QuoteCommodity)gbDetails.DataContext;

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
                QuoteCommodity commodity = new QuoteCommodity() { QuoteID = SelectedQuote.ID };
                
                gbDetails.DataContext = commodity;
                gbDepature.DataContext = commodity;
                gbDestination.DataContext = commodity;
            }
        }

        private void btnSaveCommodity_Click(object sender, RoutedEventArgs e)
        {
            QuoteCommodity commodity = (QuoteCommodity)gbDetails.DataContext;

            if (commodity != null)
            {
                SelectedQuote.QuoteCommodities.Add(commodity);
                ((ObservableCollection<QuoteCommodity>)dgQuoteCommodities.ItemsSource).Insert(0, commodity);
                dgQuoteCommodities.SelectedItem = commodity;
            }
        }

        private void btnNewCommodity_Click(object sender, RoutedEventArgs e)
        {
            QuoteCommodity commodity = new QuoteCommodity() { QuoteID = SelectedQuote.ID };

            gbDetails.DataContext = commodity;
            gbDepature.DataContext = commodity;
            gbDestination.DataContext = commodity;

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
        
    }
}
