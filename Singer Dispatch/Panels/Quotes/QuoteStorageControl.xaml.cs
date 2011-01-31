using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace SingerDispatch.Panels.Quotes
{
    /// <summary>
    /// Interaction logic for StoragQuoteStorageControleControl.xaml
    /// </summary>
    public partial class QuoteStorageControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public QuoteStorageControl()
        {
            InitializeComponent();

            if (InDesignMode()) return;

            Database = SingerConfigs.CommonDataContext;

            cmbBillingIntervals.ItemsSource = from bi in Database.BillingIntervals select bi;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode() || IsVisible == false) return;

            // refresh the commodity list
            cmbCommodities.ItemsSource = (SelectedQuote == null) ? null : SelectedQuote.QuoteCommodities.ToList();
        }

        protected override void SelectedQuoteChanged(Quote newValue, Quote oldValue)
        {
            base.SelectedQuoteChanged(newValue, oldValue);

            dgStorageList.ItemsSource = (newValue == null) ? null : new ObservableCollection<QuoteStorageItem>(newValue.QuoteStorageItems);
        }

        private void NewStorageItem_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<QuoteStorageItem>)dgStorageList.ItemsSource;
            var item = new QuoteStorageItem { QuoteID = SelectedQuote.ID };

            SelectedQuote.QuoteStorageItems.Add(item);
            list.Add(item);
            dgStorageList.SelectedItem = item;
            dgStorageList.ScrollIntoView(item);

            cmbCommodities.Focus();
        }

        private void DuplicateStorageItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (QuoteStorageItem)dgStorageList.SelectedItem;
            var list = (ObservableCollection<QuoteStorageItem>)dgStorageList.ItemsSource;

            if (item == null)
                return;

            item = item.Duplicate();

            SelectedQuote.QuoteStorageItems.Add(item);
            list.Add(item);
            dgStorageList.SelectedItem = item;
            dgStorageList.ScrollIntoView(item);
        }

        private void RemoveStorageItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (QuoteStorageItem)dgStorageList.SelectedItem;

            if (item == null) return;

            var confirmation = MessageBox.Show(SingerConfigs.DefaultRemoveItemMessage, "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;
            
            ((ObservableCollection<QuoteStorageItem>)dgStorageList.ItemsSource).Remove(item);
            SelectedQuote.QuoteStorageItems.Remove(item);            
        }
    }
}
