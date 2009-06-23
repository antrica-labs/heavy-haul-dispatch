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

            database = new SingerDispatchDataContext();
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
                    grpRateAdjustment.DataContext = discount;
                }
                else
                {
                    discount = new RateDiscount() { CompanyID = newValue.ID };
                    grpRateAdjustment.DataContext = discount;
                }
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

        private void btnSaveAdminDetails_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCompany != null)
            {                
                Company company = (from c in database.Companies where c.ID == SelectedCompany.ID select c).Single();                

                company.Type = SelectedCompany.Type;
                company.AvailableCredit = SelectedCompany.AvailableCredit;             
                company.AccPacVendorCode = SelectedCompany.AccPacVendorCode;
                company.PriorityLevelID = SelectedCompany.PriorityLevelID;
                company.Notes = SelectedCompany.Notes;
                company.EquifaxComplete = SelectedCompany.EquifaxComplete;                
                
                database.SubmitChanges();
            }
        }

        private void btnSaveAdjustments_Click(object sender, RoutedEventArgs e)
        {
            if (discount != null)
            {
                if (discount.ID == 0)
                {
                    database.RateDiscounts.InsertOnSubmit(discount);
                }

                database.SubmitChanges();
            }
        }
     
    }
}
