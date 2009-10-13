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
using SingerDispatch.Panels.Jobs;

namespace SingerDispatch.Panels.Pricing
{
    /// <summary>
    /// Interaction logic for JobPriceControl.xaml
    /// </summary>
    public partial class JobPriceControl : JobUserControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public JobPriceControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
            dgCustomerDetails.ItemsSource = new ObservableCollection<CustomerNumber>();
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            if (newValue == null)
            {                
                dgCustomerDetails.ItemsSource = new ObservableCollection<CustomerNumber>();
                cmbCareOfCompanies.ItemsSource = null;
            }
            else
            {
                dgJobs.ItemsSource = new ObservableCollection<Job>((from j in Database.Jobs where j.CompanyID == newValue.ID orderby j.EndDate descending select j).ToList());
                dgCustomerDetails.ItemsSource = new ObservableCollection<CustomerNumber>(newValue.CustomerNumbers);
                cmbCareOfCompanies.ItemsSource = (from c in Database.Companies where c.ID != newValue.ID select c).ToList();
            }
        }

        private void btnNewCustomerDetail_Click(object sender, RoutedEventArgs e)
        {
            var number = new CustomerNumber();
            var list = (ObservableCollection<CustomerNumber>)dgCustomerDetails.ItemsSource;

            list.Insert(0, number);
            dgCustomerDetails.SelectedItem = number;
        }

        private void btnRemoveCustomerDetail_Click(object sender, RoutedEventArgs e)
        {
            var number = (CustomerNumber)dgCustomerDetails.SelectedItem;

            if (number == null) return;
            
            var list = (ObservableCollection<CustomerNumber>)dgCustomerDetails.ItemsSource;
            list.Remove(number);
        }
    }
}
