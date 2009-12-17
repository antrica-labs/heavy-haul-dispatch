using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace SingerDispatch.Panels.Jobs
{
    /// <summary>
    /// Interaction logic for LoadsControl.xaml
    /// </summary>
    public partial class LoadsControl : JobUserControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public LoadsControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {            
            cmbSeasons.ItemsSource = from s in Database.Seasons select s;
            cmbRates.ItemsSource = GetCompanyRates(SelectedCompany);
            cmbUnits.ItemsSource = (SelectedJob == null) ? null : from u in Database.Equipment where u.EquipmentClass.Name == "Tractor" || u.EquipmentClass.Name == "Trailor" select u;            
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);

            dgLoads.ItemsSource = (newValue == null) ? null : new ObservableCollection<Load>(newValue.Loads);
        }

        private void NewLoad_Click(object sender, RoutedEventArgs e)
        {
            var load = new Load { JobID = SelectedJob.ID };

            SelectedJob.Loads.Add(load);
            ((ObservableCollection<Load>)dgLoads.ItemsSource).Insert(0, load);
            dgLoads.SelectedItem = load;

            txtServiceDescription.Focus();
        }

        private void AxleWeightChanged(object sender, TextChangedEventArgs e)
        {
            var load = (Load)dgLoads.SelectedItem;

            if (load == null)
            {
                return;
            }
            
            int? gross = 0;

            gross += load.WeightSteer;
            gross += load.WeightDrive;
            gross += load.WeightGroup1;
            gross += load.WeightGroup2;
            gross += load.WeightGroup3;
            gross += load.WeightGroup4;
            gross += load.WeightGroup5;
            gross += load.WeightGroup6;
            gross += load.WeightGroup7;
            gross += load.WeightGroup8;
            gross += load.WeightGroup9;
            gross += load.WeightGroup10;

            load.GrossWeight = gross;
        }

        private void RemoveLoad_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private System.Collections.IEnumerable GetCompanyRates(Company company)
        {
            if (company == null)
            {
                return null;
            }

            var rates = from r in Database.Rates where r.RateType.Name == "Trailer" select r;
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
                    rate.Adjusted = rate.Hourly * (1 + (discount / 100));
                }
            }

            return rates;
        }

        private void cmbRates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbTrailerCombinations.ItemsSource = from tc in Database.TrailerCombinations where tc.Rate == cmbRates.SelectedItem select tc;
        }
    }
}
