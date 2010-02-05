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

namespace SingerDispatch.Panels.Invoicing
{
    /// <summary>
    /// Interaction logic for InvoiceLineItemsControl.xaml
    /// </summary>
    public partial class InvoiceLineItemsControl : InvoiceUserControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public InvoiceLineItemsControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
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

            list.Remove(item);
            SelectedInvoice.InvoiceLineItems.Remove(item);
        }
    }
}
