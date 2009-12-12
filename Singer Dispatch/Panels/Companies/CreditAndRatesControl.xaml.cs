using System.Windows;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace SingerDispatch.Panels.Companies
{
    /// <summary>
    /// Interaction logic for CreditAndRatesControl.xaml
    /// </summary>
    public partial class CreditAndRatesControl : CompanyUserControl
    {   
        public SingerDispatchDataContext Database { get; set; }
        
        public CreditAndRatesControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            cmbCreditCustomerType.ItemsSource = SingerConstants.CustomerTypes;
            cmbCreditPriority.ItemsSource = (from l in Database.CompanyPriorityLevels orderby l.Name select l).ToList();

            if (SelectedCompany != null)
            {
                dgCreditRates.ItemsSource = GetCompanyRates(SelectedCompany);
            }
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);        
        }

        private void SaveDetails(object sender, RoutedEventArgs e)
        {
            Database.SubmitChanges();

            dgCreditRates.ItemsSource = GetCompanyRates(SelectedCompany);
        }


        private List<Rate> GetCompanyRates(Company company)
        {
            var rates = from r in Database.Rates select r;
            var discount = company.RateAdjustment != null ? company.RateAdjustment : 0.00m;
            var enterprise = company.Type == "M.E. Signer Enterprise";

            foreach (var rate in rates)
            {
                if (enterprise && rate.HourlyEnterprise != null)
                {
                    rate.Hourly = rate.HourlySpecialized;
                    rate.Adjusted = rate.Hourly * (1 + (discount / 100));
                }
                else if (!enterprise && rate.HourlySpecialized != null)
                {
                    rate.Hourly = rate.HourlyEnterprise;
                    rate.Adjusted = rate.Hourly * (1+ (discount / 100));
                }
            }

            return rates.ToList();
        }
    }
}
