﻿using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace SingerDispatch.Panels.Quotes
{
    /// <summary>
    /// Interaction logic for QuoteSupplements.xaml
    /// </summary>
    public partial class QuoteSupplementsControl
    {
        public QuoteSupplementsControl()
        {
            InitializeComponent();

            if (InDesignMode()) return;

            Database = SingerConfigs.CommonDataContext;

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
                        
            SelectedQuote.QuoteSupplements.Add(supplement);
            list.Add(supplement);
            dgSupplements.SelectedItem = supplement;
            dgSupplements.ScrollIntoView(supplement);
            
            txtName.Focus();
        }

        private void DuplicateSupplement_Click(object sender, RoutedEventArgs e)
        {
            var supplement = (QuoteSupplement)dgSupplements.SelectedItem;
            var list = (ObservableCollection<QuoteSupplement>)dgSupplements.ItemsSource;

            if (supplement == null)
                return;

            supplement = supplement.Duplicate();

            SelectedQuote.QuoteSupplements.Add(supplement);
            list.Add(supplement);
            dgSupplements.SelectedItem = supplement;
            dgSupplements.ScrollIntoView(supplement);
        }

        private void RemoveSupplement_Click(object sender, RoutedEventArgs e)
        {
            var supplement = (QuoteSupplement)dgSupplements.SelectedItem;

            if (supplement == null) return;

            var confirmation = MessageBox.Show(SingerConfigs.DefaultRemoveItemMessage, "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            SelectedQuote.QuoteSupplements.Remove(supplement);
            ((ObservableCollection<QuoteSupplement>)dgSupplements.ItemsSource).Remove(supplement);
        }

        private void BillingType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var type = (BillingType)((ComboBox)sender).SelectedItem;

            if (type != null && type.Name == "Cost Included")
            {                   
                txtCost.IsEnabled = false;

                var supplement = (QuoteSupplement)dgSupplements.SelectedItem;

                if (supplement != null)
                {
                    supplement.Cost = null;
                }                    
            }
            else
            {                
                txtCost.IsEnabled = true;
            }
        }
    }
}
