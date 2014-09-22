using System.Windows;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using SingerDispatch.Controls;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System;
using SingerDispatch.Windows;

namespace SingerDispatch.Panels.Companies
{
    /// <summary>
    /// Interaction logic for CreditAndRatesControl.xaml
    /// </summary>
    public partial class CreditAndRatesControl
    {
        private CommandBinding SaveCommand { get; set; }

        public CreditAndRatesControl()
        {
            InitializeComponent();

            if (InDesignMode()) return;

            Database = SingerConfigs.CommonDataContext;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode() || IsVisible == false) return;

            cmbCreditPriority.ItemsSource = from l in Database.CompanyPriorityLevels orderby l.Level select l;
            cmbCreditCustomerType.ItemsSource = from ct in Database.CustomerType select ct;

            UpdateDefaultRates();
            UpdateAdjustedRates();
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            UpdateDefaultRates();
            UpdateAdjustedRates();
        }

        private void cmbCreditCustomerType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateDefaultRates();
        }

        private void UpdateDefaultRates()
        {
            dgCreditRates.ItemsSource = GetDefaultRates();
        }

        private void UpdateAdjustedRates()
        {
            dgAdjustedCreditRates.ItemsSource = (SelectedCompany == null) ? null : new ObservableCollection<RateAdjustment>(from ra in Database.RateAdjustments where ra.Rate.Archived != true && ra.Company == SelectedCompany orderby ra.Rate.RateType.Name, ra.Rate.Name select ra);
        }

        private void CreateAdjustment_Click(object sender, RoutedEventArgs e)
        {
            var rate = (Rate)dgCreditRates.SelectedItem;
            var list = (ObservableCollection<RateAdjustment>)dgAdjustedCreditRates.ItemsSource;

            if (SelectedCompany == null || rate == null) return;
            
            RateAdjustment adjustment;

            try
            {
                // Check if an adjustment for this rate already exists
                var query = from ra in list where ra.Rate == rate select ra;
                adjustment = query.First();
            }
            catch
            {
                adjustment = new RateAdjustment { Company = SelectedCompany, Rate = rate, AdjustedRate = rate.Hourly };

                SelectedCompany.RateAdjustments.Add(adjustment);
                list.Add(adjustment);
            }
            
            dgAdjustedCreditRates.ScrollIntoView(adjustment);
            dgAdjustedCreditRates.SelectedItem = adjustment;

            DataGridHelper.EditSecondColumn(dgAdjustedCreditRates, adjustment);
        }

        private void RemoveAdjustment_Click(object sender, RoutedEventArgs e)
        {
            var adjustment = (RateAdjustment)dgAdjustedCreditRates.SelectedItem;

            if (SelectedCompany == null || adjustment == null) return;

            try
            {
                SelectedCompany.RateAdjustments.Remove(adjustment);

                Database.SubmitChanges();

                ((ObservableCollection<RateAdjustment>)dgAdjustedCreditRates.ItemsSource).Remove(adjustment);
            }
            catch (Exception ex)
            {
                NoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);

                Database.RevertChanges();
            }
        }

        private IEnumerable<Rate> GetDefaultRates()
        {
            if (SelectedCompany == null)
                return null;

            var rates = from r in Database.Rates where r.Archived != true orderby r.RateType.Name, r.Name select r;
            var enterprise = SelectedCompany.CustomerType != null && SelectedCompany.CustomerType.IsEnterprise == true;

            foreach (var rate in rates)
            {
                rate.Hourly = (enterprise) ? rate.HourlyEnterprise : rate.HourlySpecialized;
            }

            return rates.ToList();
        }

        private void dgAdjustedCreditRates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var adjustment = (RateAdjustment)dgAdjustedCreditRates.SelectedItem;

            if (adjustment == null || adjustment.Rate == null) return;

            dgCreditRates.ScrollIntoView(adjustment.Rate);
            dgCreditRates.SelectedItem = adjustment.Rate;
        }
    }
}
