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
using Microsoft.Windows.Controls;

namespace SingerDispatch.Panels.Quotes
{
    /// <summary>
    /// Interaction logic for QuoteSupplements.xaml
    /// </summary>
    public partial class QuoteSupplements : QuoteUserControl
    {
        SingerDispatchDataContext database;

        public QuoteSupplements()
        {
            InitializeComponent();

            database = SingerConstants.CommonDataContext;

            cmbBillingType.ItemsSource = (from bt in database.BillingTypes select bt).ToList();
        }

        private void QuoteUserControl_Loaded(object sender, RoutedEventArgs e)
        {            
        }

        protected override void SelectedQuoteChanged(Quote newValue, Quote oldValue)
        {
            base.SelectedQuoteChanged(newValue, oldValue);

            if (newValue != null)
            {
                dgSupplements.ItemsSource = new ObservableCollection<QuoteSupplement>(newValue.QuoteSupplements);
                grpSupplementDetails.DataContext = new QuoteSupplement() { QuoteID = newValue.ID };
            }
            else
            {
                dgSupplements.ItemsSource = null;
                grpSupplementDetails.DataContext = null;
            }
        }

        private void dgSupplements_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            grpSupplementDetails.DataContext = dgSupplements.SelectedItem;
        }

        private void btnNewSupplement_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<QuoteSupplement> list = (ObservableCollection<QuoteSupplement>)dgSupplements.ItemsSource;
            QuoteSupplement supplement = new QuoteSupplement() { QuoteID = SelectedQuote.ID };
            
            list.Insert(0, supplement);
            SelectedQuote.QuoteSupplements.Add(supplement);
            dgSupplements.SelectedItem = supplement;
            
            txtName.Focus();
        }

        private void btnRemoveSupplement_Click(object sender, RoutedEventArgs e)
        {
            QuoteSupplement supplement = (QuoteSupplement)dgSupplements.SelectedItem;

            if (supplement == null)
            {
                return;
            }

            MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to remove this supplement?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation == MessageBoxResult.Yes)
            {
                SelectedQuote.QuoteSupplements.Remove(supplement);
                ((ObservableCollection<QuoteSupplement>)dgSupplements.ItemsSource).Remove(supplement);
            }
        }

        private void cmbBillingType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BillingType type = (BillingType)((ComboBox)sender).SelectedItem;

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
