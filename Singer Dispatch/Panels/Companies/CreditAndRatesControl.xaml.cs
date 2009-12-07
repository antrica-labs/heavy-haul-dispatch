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
                dgCreditRates.ItemsSource = GetAdjustedRates(SelectedCompany.RateAdjustment);
            }
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);
        }

        private List<Rate> GetAdjustedRates(double? discount)
        {
            var rates = from r in Database.Rates select r;

            if (discount == null)
            {
                return rates.ToList();
            }
            
            foreach (var rate in rates)
            {
                if (rate.Hourly != null)
                {
                    rate.Adjusted = rate.Hourly * (1 + (discount / 100));
                }
            }            

            return rates.ToList();
        }

        private void SaveDetails(object sender, RoutedEventArgs e)
        {
            Database.SubmitChanges();

            dgCreditRates.ItemsSource = GetAdjustedRates(SelectedCompany.RateAdjustment);
        }
    }
}
