using System.Windows;
using System.Collections.ObjectModel;

namespace SingerDispatch.Panels.Invoicing
{
    /// <summary>
    /// Interaction logic for InvoiceExtrasControl.xaml
    /// </summary>
    public partial class InvoiceExtrasControl
    {
        public InvoiceExtrasControl()
        {
            InitializeComponent();
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {

        }

        protected override void SelectedInvoiceChanged(Invoice newValue, Invoice oldValue)
        {
            base.SelectedInvoiceChanged(newValue, oldValue);

            dgExtras.ItemsSource = (newValue == null) ? null : new ObservableCollection<InvoiceExtra>(newValue.InvoiceExtras);
        }

        private void NewExtra_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<InvoiceExtra>)dgExtras.ItemsSource;
            var item = new InvoiceExtra();

            SelectedInvoice.InvoiceExtras.Add(item);
            list.Add(item);
            dgExtras.SelectedItem = item;
            dgExtras.ScrollIntoView(item);

            txtDescription.Focus();
        }

        private void RemoveExtra_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<InvoiceExtra>)dgExtras.ItemsSource;
            var item = (InvoiceExtra)dgExtras.SelectedItem;

            if (item == null) return;

            list.Remove(item);
            SelectedInvoice.InvoiceExtras.Remove(item);
        }
    }
}
