using System.Windows;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using SingerDispatch.Controls;
using System.Windows.Controls;

namespace SingerDispatch.Panels.Companies
{
    /// <summary>
    /// Interaction logic for CreditAndRatesControl.xaml
    /// </summary>
    public partial class CreditAndRatesControl
    {
        private CommandBinding SaveCommand { get; set; }

        public SingerDispatchDataContext Database { get; set; }        

        public CreditAndRatesControl()
        {
            InitializeComponent();

            if (InDesignMode()) return;

            Database = SingerConfigs.CommonDataContext;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode()) return;

            cmbCreditPriority.ItemsSource = from l in Database.CompanyPriorityLevels orderby l.Name select l;
            cmbCreditCustomerType.ItemsSource = from ct in Database.CustomerType select ct;
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            dgCreditRates.ItemsSource = GetCompanyRates(SelectedCompany);
        }

        private void cmbCreditCustomerType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            dgCreditRates.ItemsSource = GetCompanyRates(SelectedCompany);
        }

        private IEnumerable<Rate> GetCompanyRates(Company company)
        {
            if (company == null)
                return null;

            var rates = from r in Database.Rates where r.Archived != true orderby r.RateType.Name, r.Name select r;
            var discount = company.RateAdjustment ?? 0.00m;
            var enterprise = company.CustomerType != null && company.CustomerType.IsEnterprise == true;

            foreach (var rate in rates)
            {
                if (enterprise)
                {
                    if (rate.HourlyEnterprise != null)
                    {
                        rate.Hourly = rate.HourlyEnterprise;
                        rate.Adjusted = rate.Hourly + discount;
                    }
                }
                else
                {
                    if (rate.HourlySpecialized != null)
                    {
                        rate.Hourly = rate.HourlySpecialized;
                        rate.Adjusted = rate.Hourly + discount;
                    }
                }
            }

            return rates.ToList();
        }

        private bool _adjustmentChanged;

        private void txtCreditRateHourly_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                UpdateAdjustment(sender as TextBox);                
            }
        }

        private void txtCreditRateHourly_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_adjustmentChanged)
            {
                UpdateAdjustment(sender as TextBox);
                _adjustmentChanged = false;
            }
        }

        private void txtCreditRateHourly_GotFocus(object sender, RoutedEventArgs e)
        {
            _adjustmentChanged = false;
        }

        private void txtCreditRateHourly_TextChanged(object sender, TextChangedEventArgs e)
        {   
            _adjustmentChanged = true;
        }

        private void UpdateAdjustment(TextBox tb)
        {
            var expr = System.Windows.Data.BindingOperations.GetBindingExpressionBase(tb, TextBox.TextProperty);

            if (expr != null)
            {
                expr.UpdateSource();
            }

            dgCreditRates.ItemsSource = GetCompanyRates(SelectedCompany);
            _adjustmentChanged = false;
        }
    }
}
