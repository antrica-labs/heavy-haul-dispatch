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
    /// Interaction logic for LoadInvoicingControl.xaml
    /// </summary>
    public partial class LoadInvoicingControl : JobUserControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public LoadInvoicingControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;

            dgCustomerDetails.ItemsSource = new ObservableCollection<CustomerNumber>();
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

        }

        private void cmbRates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbTrailerCombinations.ItemsSource = from tc in Database.TrailerCombinations where tc.Rate == cmbRates.SelectedItem select tc;
        }

        private void NewCustomerDetail_Click(object sender, RoutedEventArgs e)
        {
            var number = new CustomerNumber();
            var list = (ObservableCollection<CustomerNumber>)dgCustomerDetails.ItemsSource;

            list.Insert(0, number);
            dgCustomerDetails.SelectedItem = number;
        }

        private void RemoveCustomerDetail_Click(object sender, RoutedEventArgs e)
        {
            var number = (CustomerNumber)dgCustomerDetails.SelectedItem;

            if (number == null) return;

            var list = (ObservableCollection<CustomerNumber>)dgCustomerDetails.ItemsSource;
            list.Remove(number);
        }
    }
}
