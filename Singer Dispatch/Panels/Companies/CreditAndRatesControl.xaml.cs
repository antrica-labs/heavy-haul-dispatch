using System.Windows.Controls;
using System.Windows;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SingerDispatch.Panels.Companies
{
    /// <summary>
    /// Interaction logic for CreditAndRatesControl.xaml
    /// </summary>
    public partial class CreditAndRatesControl : CompanyUserControl
    {
        private RateDiscount discount;
        private SingerDispatchDataContext database;

        public CreditAndRatesControl()
        {
            InitializeComponent();

            database = SingerConstants.CommonDataContext;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            cmbCreditCustomerType.ItemsSource = SingerConstants.CustomerTypes;
            cmbCreditPriority.ItemsSource = (from l in database.CompanyPriorityLevels orderby l.Name select l).ToList();

            dgCreditRates.ItemsSource = new ObservableCollection<Rate>(
                (from r in database.Rates select r).ToList()
            );
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            if (newValue != null)
            {                               
                var rateDiscounts = from d in database.RateDiscounts where d.CompanyID == newValue.ID select d;

                if (rateDiscounts.Count() > 0)
                {
                    discount = (RateDiscount)rateDiscounts.First();                    
                }
                else
                {
                    discount = new RateDiscount() { CompanyID = newValue.ID };                    
                }

                grpAdministration.DataContext = newValue;
                grpRateAdjustment.DataContext = discount;
            }
            else
            {
                grpRateAdjustment.DataContext = null;
            }
        }

        private void DataGridCommit(object sender, Microsoft.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {
            database.SubmitChanges();
        }

        private void SaveDetails(object sender, RoutedEventArgs e)
        {
            if (SelectedCompany != null)
            {
                if (discount != null && discount.ID == 0)
                {
                    database.RateDiscounts.InsertOnSubmit(discount);
                }

                database.SubmitChanges();
            }
        }
    }
}
