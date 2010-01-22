using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace SingerDispatch.Panels.Quotes
{
    /// <summary>
    /// Interaction logic for StoragQuoteStorageControleControl.xaml
    /// </summary>
    public partial class QuoteStorageControl : QuoteUserControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public QuoteStorageControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;

            cmbBillingType.ItemsSource = from bt in Database.BillingTypes select bt;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            // refresh the commodity list
            cmbCommodities.ItemsSource = (SelectedQuote == null) ? null : from c in Database.Commodities where c.Company == SelectedCompany || c.Company == SelectedQuote.CareOfCompany select c;
        }

        protected override void SelectedQuoteChanged(Quote newValue, Quote oldValue)
        {
            base.SelectedQuoteChanged(newValue, oldValue);

            dgStorageList.ItemsSource = (newValue == null) ? null : new ObservableCollection<QuoteStorageItem>(newValue.QuoteStorageItems);
        }

        private void NewStorageItem_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<QuoteStorageItem>)dgStorageList.ItemsSource;
            var item = new QuoteStorageItem() { QuoteID = SelectedQuote.ID };

            SelectedQuote.QuoteStorageItems.Add(item);
            list.Add(item);
            dgStorageList.SelectedItem = item;
            dgStorageList.ScrollIntoView(item);

            cmbCommodities.Focus();
        }

        private void DuplicateStorageItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (QuoteStorageItem)dgStorageList.SelectedItem;

            if (item == null)
                return;

            item = item.Duplicate();

            SelectedQuote.QuoteStorageItems.Add(item);
            ((ObservableCollection<QuoteStorageItem>)dgStorageList.ItemsSource).Insert(0, item);
        }

        private void RemoveStorageItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (QuoteStorageItem)dgStorageList.SelectedItem;

            if (item == null)
                return;

            var confirmation = MessageBox.Show("Are you sure you want to remove this storage item?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation == MessageBoxResult.Yes)
            {
                ((ObservableCollection<QuoteStorageItem>)dgStorageList.ItemsSource).Remove(item);
                SelectedQuote.QuoteStorageItems.Remove(item);
            }
        }


        private void BillingType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var type = (BillingType)((ComboBox)sender).SelectedItem;

            if (type != null && type.Name == "Cost Included")
            {
                txtQuantity.Text = null;
                txtQuantity.IsEnabled = false;
                txtCostPerItem.Text = null;
                txtCostPerItem.IsEnabled = false;
            }
            else
            {
                txtQuantity.IsEnabled = true;
                txtCostPerItem.IsEnabled = true;
            }
        }
    }
}
