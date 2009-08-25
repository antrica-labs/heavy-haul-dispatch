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

            dgCustomerDetails.ItemsSource = new ObservableCollection<CustomerNumber>();
        }

        public static void SelectedCompanyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            JobPriceControl control = (JobPriceControl)d;
            Company company = (Company)e.NewValue;

            if (company == null)
            {
                control.dgCustomerDetails.ItemsSource = new ObservableCollection<CustomerNumber>();                
            }
            else
            {
                control.dgCustomerDetails.ItemsSource = new ObservableCollection<CustomerNumber>(company.CustomerNumbers);
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
