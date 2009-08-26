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
        public static DependencyProperty SelectedCompanyProperty = DependencyProperty.Register("SelectedCompanyProperty", typeof(Company), typeof(JobPriceControl), new PropertyMetadata(null, JobPriceControl.SelectedCompanyPropertyChanged));

        private SingerDispatchDataContext database;

        public Company SelectedCompany
        {
            get
            {
                return (Company)GetValue(SelectedCompanyProperty);
            }
            set
            {
                SetValue(SelectedCompanyProperty, value);
            }
        }

        public JobPriceControl()
        {
            InitializeComponent();

            database = SingerConstants.CommonDataContext;
            dgCustomerDetails.ItemsSource = new ObservableCollection<CustomerNumber>();
        }

        public static void SelectedCompanyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            JobPriceControl control = (JobPriceControl)d;
            
            control.SelectedCompanyChanged((Company)e.NewValue, (Company)e.OldValue);            
        }

        private void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            if (newValue == null)
            {                
                dgCustomerDetails.ItemsSource = new ObservableCollection<CustomerNumber>();
                cmbCareOfCompanies.ItemsSource = null;
            }
            else
            {
                dgJobs.ItemsSource = new ObservableCollection<Job>((from j in database.Jobs where j.CompanyID == newValue.ID orderby j.EndDate descending select j).ToList());
                dgCustomerDetails.ItemsSource = new ObservableCollection<CustomerNumber>(newValue.CustomerNumbers);
                cmbCareOfCompanies.ItemsSource = (from c in database.Companies where c.ID != newValue.ID select c).ToList();
            }
        }

        private void btnNewCustomerDetail_Click(object sender, RoutedEventArgs e)
        {
            CustomerNumber number = new CustomerNumber();
            ObservableCollection<CustomerNumber> list = (ObservableCollection<CustomerNumber>)dgCustomerDetails.ItemsSource;

            list.Insert(0, number);
            dgCustomerDetails.SelectedItem = number;
        }

        private void btnRemoveCustomerDetail_Click(object sender, RoutedEventArgs e)
        {
            CustomerNumber number = (CustomerNumber)dgCustomerDetails.SelectedItem;

            if (number != null)
            {
                ObservableCollection<CustomerNumber> list = (ObservableCollection<CustomerNumber>)dgCustomerDetails.ItemsSource;
                list.Remove(number);
            }
        }
    }
}
