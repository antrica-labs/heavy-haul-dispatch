using System.Windows;
using System.Collections.ObjectModel;
using SingerDispatch.Controls;

namespace SingerDispatch.Panels.Invoicing
{
    /// <summary>
    /// Interaction logic for InvoiceLineItemsControl.xaml
    /// </summary>
    public partial class InvoiceLineItemsControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public InvoiceLineItemsControl()
        {
            InitializeComponent();

            Database = SingerConfigs.CommonDataContext;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        protected override void SelectedInvoiceChanged(Invoice newValue, Invoice oldValue)
        {
            base.SelectedInvoiceChanged(newValue, oldValue);

            dgLineItems.ItemsSource = (newValue == null) ? null : new ObservableCollection<InvoiceLineItem>(newValue.InvoiceLineItems);
        }

        private void NewLineItem_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<InvoiceLineItem>)dgLineItems.ItemsSource;
            var item = new InvoiceLineItem();

            SelectedInvoice.InvoiceLineItems.Add(item);
            list.Add(item);
            dgLineItems.SelectedItem = item;
            dgLineItems.ScrollIntoView(item);

            txtDescription.Focus();
        }

        private void AutoGenerate_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to add everything from the job to this invoice?", "Generate confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            dgLineItems.ItemsSource = new ObservableCollection<InvoiceLineItem>(SelectedInvoice.InvoiceLineItems);
        }

        private void RemoveLineItem_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<InvoiceLineItem>)dgLineItems.ItemsSource;
            var item = (InvoiceLineItem)dgLineItems.SelectedItem;

            if (item == null) return;

            var confirmation = MessageBox.Show(SingerConfigs.DefaultRemoveItemMessage, "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            list.Remove(item);
            SelectedInvoice.InvoiceLineItems.Remove(item);
        }

        private void NewLineExtra_Click(object sender, RoutedEventArgs e)
        {
            var item = (InvoiceLineItem)dgLineItems.SelectedItem;
            var extra = new InvoiceExtra { LineItem = item };

            if (item == null) return;
            
            ((ObservableCollection<InvoiceExtra>)dgLineExtras.ItemsSource).Add(extra);
            item.Extras.Add(extra);

            DataGridHelper.EditFirstColumn(dgLineExtras, extra);
        }

        private void RemoveLineExtra_Click(object sender, RoutedEventArgs e)
        {
            var item = (InvoiceLineItem)dgLineItems.SelectedItem;
            var extra = (InvoiceExtra)dgLineExtras.SelectedItem;

            if (item == null || extra == null) return;

            ((ObservableCollection<InvoiceExtra>)dgLineExtras.ItemsSource).Remove(extra);
            item.Extras.Remove(extra);
        }

        private void dgLineItems_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var item = (InvoiceLineItem)dgLineItems.SelectedItem;

            if (item == null)
            {
                dgLineExtras.ItemsSource = null;
            }
            else
            {
                dgLineExtras.ItemsSource = new ObservableCollection<InvoiceExtra>(item.Extras);
            }
        }
    }
}
