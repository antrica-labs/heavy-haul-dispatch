using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace SingerDispatch.Panels.Quotes
{
    /// <summary>
    /// Interaction logic for StorageControl.xaml
    /// </summary>
    public partial class StorageControl : QuoteUserControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public StorageControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;

            cmbBillingType.ItemsSource = from bt in Database.BillingTypes select bt;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            cmbCommodities.ItemsSource = null;

            if (SelectedCompany != null)
            {
                cmbCommodities.ItemsSource = from c in Database.Commodities where c.Company == SelectedCompany select c;
            }
        }

        protected override void SelectedQuoteChanged(Quote newValue, Quote oldValue)
        {
            base.SelectedQuoteChanged(newValue, oldValue);

            dgStorageList.ItemsSource = null;

            if (newValue != null)
            {
                dgStorageList.ItemsSource = new ObservableCollection<StorageItem>(newValue.StorageItems);
            }
        }

        private void NewStorageItem_Click(object sender, RoutedEventArgs e)
        {
            var item = new StorageItem() { QuoteID = SelectedQuote.ID };

            SelectedQuote.StorageItems.Add(item);
            ((ObservableCollection<StorageItem>)dgStorageList.ItemsSource).Insert(0, item);            
            dgStorageList.SelectedItem = item;

            cmbCommodities.Focus();
        }

        private void RemoveStorageItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (StorageItem)dgStorageList.SelectedItem;

            if (item == null)
                return;

            var confirmation = MessageBox.Show("Are you sure you want to remove this storage item?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation == MessageBoxResult.Yes)
            {
                ((ObservableCollection<StorageItem>)dgStorageList.ItemsSource).Remove(item);
                SelectedQuote.StorageItems.Remove(item);
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
