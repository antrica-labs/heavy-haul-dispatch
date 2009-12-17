﻿using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace SingerDispatch.Panels.Quotes
{
    /// <summary>
    /// Interaction logic for QuoteSupplements.xaml
    /// </summary>
    public partial class QuoteSupplementsControl : QuoteUserControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public QuoteSupplementsControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;

            cmbBillingType.ItemsSource = from bt in Database.BillingTypes select bt;
        }

        private void QuoteUserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        protected override void SelectedQuoteChanged(Quote newValue, Quote oldValue)
        {
            base.SelectedQuoteChanged(newValue, oldValue);

            dgSupplements.ItemsSource = (newValue == null) ? null : new ObservableCollection<QuoteSupplement>(newValue.QuoteSupplements);
        }

        private void NewSupplement_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<QuoteSupplement>)dgSupplements.ItemsSource;
            var supplement = new QuoteSupplement { QuoteID = SelectedQuote.ID };
            
            list.Insert(0, supplement);
            SelectedQuote.QuoteSupplements.Add(supplement);
            dgSupplements.SelectedItem = supplement;
            
            txtName.Focus();
        }

        private void RemoveSupplement_Click(object sender, RoutedEventArgs e)
        {
            var supplement = (QuoteSupplement)dgSupplements.SelectedItem;

            if (supplement == null)
            {
                return;
            }

            var confirmation = MessageBox.Show("Are you sure you want to remove this supplement?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation == MessageBoxResult.Yes)
            {
                SelectedQuote.QuoteSupplements.Remove(supplement);
                ((ObservableCollection<QuoteSupplement>)dgSupplements.ItemsSource).Remove(supplement);
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
