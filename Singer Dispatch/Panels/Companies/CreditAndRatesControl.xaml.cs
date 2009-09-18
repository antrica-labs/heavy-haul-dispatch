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
        private RateDiscount Discount { get; set; }
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

            dgCreditRates.ItemsSource = new ObservableCollection<Rate>(
                (from r in Database.Rates select r).ToList()
            );
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            if (newValue != null)
            {                               
                var rateDiscounts = from d in Database.RateDiscounts where d.CompanyID == newValue.ID select d;

                Discount = rateDiscounts.Count() > 0 ? rateDiscounts.First() : new RateDiscount { CompanyID = newValue.ID };

                grpAdministration.DataContext = newValue;
                grpRateAdjustment.DataContext = Discount;
            }
            else
            {
                grpRateAdjustment.DataContext = null;
            }
        }

        private void DataGridCommit(object sender, Microsoft.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {
            Database.SubmitChanges();
        }

        private void SaveDetails(object sender, RoutedEventArgs e)
        {
            if (SelectedCompany == null)
            {
                return;
            }

            if (Discount != null && Discount.ID == 0)
            {
                Database.RateDiscounts.InsertOnSubmit(Discount);
            }

            Database.SubmitChanges();
        }
    }
}
