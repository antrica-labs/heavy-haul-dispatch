using System.Windows;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using SingerDispatch.Controls;

namespace SingerDispatch.Panels.Companies
{
    /// <summary>
    /// Interaction logic for CreditAndRatesControl.xaml
    /// </summary>
    public partial class CreditAndRatesControl : CompanyUserControl
    {
        private CommandBinding SaveCommand { get; set; }

        public SingerDispatchDataContext Database { get; set; }        

        public CreditAndRatesControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;

            SaveCommand = new CommandBinding(CustomCommands.GenericSaveCommand);
            CommandBindings.Add(SaveCommand);

            cmbCreditCustomerType.ItemsSource = SingerConstants.CustomerTypes;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {            
            SaveCommand.Executed += new ExecutedRoutedEventHandler(CommitChanges_Executed);            

            cmbCreditPriority.ItemsSource = from l in Database.CompanyPriorityLevels orderby l.Name select l;

            dgCreditRates.ItemsSource = GetCompanyRates(SelectedCompany);
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);        
        }

        private void CommitChanges_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CommitChangesButton.Focus();
            CommitChangesButton.RaiseEvent(new System.Windows.RoutedEventArgs(System.Windows.Controls.Button.ClickEvent, CommitChangesButton));
        }

        private void SaveDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Database.SubmitChanges();
            }
            catch (System.Exception ex)
            {
                SingerDispatch.Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }

            dgCreditRates.ItemsSource = GetCompanyRates(SelectedCompany);
        }


        private List<Rate> GetCompanyRates(Company company)
        {
            if (company == null)
                return null;

            var rates = from r in Database.Rates select r;
            var discount = company.RateAdjustment != null ? company.RateAdjustment : 0.00m;
            var enterprise = company.Type == "M.E. Signer Enterprise";

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
