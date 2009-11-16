using System.Windows;
using System.Linq;
using System.Collections.ObjectModel;

namespace SingerDispatch.Panels.Companies
{
    /// <summary>
    /// Interaction logic for CreditAndRatesControl.xaml
    /// </summary>
    public partial class CreditAndRatesControl : CompanyUserControl
    {        
        public static DependencyProperty SelectedRateDiscountProperty = DependencyProperty.Register("SelectedRateDiscount", typeof(RateDiscount), typeof(CreditAndRatesControl));

        public RateDiscount SelectedRateDiscount
        {
            get
            {
                return (RateDiscount)GetValue(SelectedRateDiscountProperty);
            }
            set
            {
                SetValue(SelectedRateDiscountProperty, value);
            }
        }

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

            if (newValue == null)
            {
                return;
            }

            var rateDiscounts = from d in Database.RateDiscounts where d.CompanyID == newValue.ID select d;

            SelectedRateDiscount = rateDiscounts.Count() > 0 ? rateDiscounts.First() : new RateDiscount { CompanyID = newValue.ID };
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

            if (SelectedRateDiscount != null && SelectedRateDiscount.ID == 0)
            {
                Database.RateDiscounts.InsertOnSubmit(SelectedRateDiscount);
            }

            Database.SubmitChanges();
        }
    }
}
