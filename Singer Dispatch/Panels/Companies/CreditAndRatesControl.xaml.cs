using System.Windows;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using SingerDispatch.Controls;

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

            Database = SingerConstants.CommonDataContext;

            SaveCommand = new CommandBinding(CustomCommands.GenericSaveCommand);
            CommandBindings.Add(SaveCommand);
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {            
            SaveCommand.Executed += CommitChanges_Executed;            

            cmbCreditPriority.ItemsSource = from l in Database.CompanyPriorityLevels orderby l.Name select l;
            cmbCreditCustomerType.ItemsSource = from ct in Database.CustomerType select ct;

            dgCreditRates.ItemsSource = GetCompanyRates(SelectedCompany);
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            dgCreditRates.ItemsSource = GetCompanyRates(SelectedCompany);
        }

        private void CommitChanges_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CommitChangesButton.Focus();
            CommitChangesButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, CommitChangesButton));
        }

        private void SaveDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Database.SubmitChanges();
            }
            catch (System.Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }

            dgCreditRates.ItemsSource = GetCompanyRates(SelectedCompany);
        }


        private IEnumerable<Rate> GetCompanyRates(Company company)
        {
            if (company == null)
                return null;

            var rates = from r in Database.Rates select r;
            var discount = company.RateAdjustment ?? 0.00m;
            var enterprise = company.CustomerType != null && company.CustomerType.IsEnterprise == true;

            foreach (var rate in rates)
            {
                if (enterprise && rate.HourlyEnterprise != null)
                {
                    rate.Hourly = rate.HourlySpecialized;
                    rate.Adjusted = rate.Hourly + discount;
                }
                else if (!enterprise && rate.HourlySpecialized != null)
                {
                    rate.Hourly = rate.HourlyEnterprise;
                    rate.Adjusted = rate.Hourly + discount;
                }
            }

            return rates.ToList();
        }
    }
}
