using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace SingerDispatch.Panels.Loads
{
    /// <summary>
    /// Interaction logic for LoadEquipmentControl.xaml
    /// </summary>
    public partial class LoadEquipmentControl 
    {
        public SingerDispatchDataContext Database { get; set; }

        public LoadEquipmentControl()
        {
            InitializeComponent();

            if (InDesignMode()) return;

            Database = SingerConfigs.CommonDataContext;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode()) return;

            cmbSeasons.ItemsSource = from s in Database.Seasons select s;
            cmbRates.ItemsSource = GetCompanyRates(SelectedCompany);
            cmbUnits.ItemsSource = (SelectedLoad == null) ? null : from u in Database.Equipment where u.EquipmentClass.Name == "Tractor" || u.EquipmentClass.Name == "Trailor" orderby u.UnitNumber select u;
            cmbStatuses.ItemsSource = from s in Database.Statuses select s;

            if (cmbRates.SelectedItem != null)
            {
                cmbTrailerCombinations.ItemsSource = (from tc in Database.TrailerCombinations where tc.Rate == cmbRates.SelectedItem select tc).ToList();
            }

            UpdateExtras();
        }

        protected override void SelectedLoadChanged(Load newValue, Load oldValue)
        {
            base.SelectedLoadChanged(newValue, oldValue);

            UpdateExtras();
        }

        private void UpdateExtras()
        {
            if (SelectedLoad == null) return;

            var types = (from et in Database.ExtraEquipmentTypes orderby et.Name select et).ToList();
            var list = new ObservableCollection<ExtraEquipment>(SelectedLoad.ExtraEquipment);

            lbExtraEquipmentTypes.ItemsSource = types;

            dgExtraEquipment.ItemsSource = list;
        }

        private void AddEquipment_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedLoad == null) return;

            var equipment = new ExtraEquipment();

            SelectedLoad.ExtraEquipment.Add(equipment);
            ((ObservableCollection<ExtraEquipment>)dgExtraEquipment.ItemsSource).Add(equipment);
            dgExtraEquipment.ScrollIntoView(equipment);
            dgExtraEquipment.SelectedItem = equipment;

            lbExtraEquipmentTypes.Focus();
        }

        private void RemoveEquipment_Click(object sender, RoutedEventArgs e)
        {
            var equipment = (ExtraEquipment)dgExtraEquipment.SelectedItem;

            SelectedLoad.ExtraEquipment.Remove(equipment);
            ((ObservableCollection<ExtraEquipment>)dgExtraEquipment.ItemsSource).Remove(equipment);
        }

        private void GuessLoadWeights_Click(object sender, RoutedEventArgs e)
        {
            // Populate as many of the estimated weights and the dimensions as you can based on the tractor, trailer combo, and commodities.
            var load = SelectedLoad;

            if (load == null) return;

            load.GrossWeight = 0.0;

            if (load.Equipment != null)
            {
                load.GrossWeight += load.Equipment.Tare ?? 0.0;
            }

            if (load.TrailerCombination != null)
            {
                load.GrossWeight += load.TrailerCombination.Tare ?? 0.0;
                load.LoadedWidth = load.TrailerCombination.Width ?? 0.0;
                load.LoadedLength = load.TrailerCombination.Length ?? 0.0;
                load.LoadedHeight = load.TrailerCombination.Height ?? 0.0;
            }

            var widest = 0.0;
            var longest = 0.0;
            var highest = 0.0;

            foreach (var commodity in load.LoadedCommodities)
            {
                load.GrossWeight += commodity.JobCommodity.Weight ?? 0.0;

                var length = commodity.JobCommodity.Length ?? 0.0;
                var height = commodity.JobCommodity.Height ?? 0.0;
                var width = commodity.JobCommodity.Width ?? 0.0;

                if (length > longest)
                    longest = length;

                if (height > highest)
                    highest = height;

                if (width > widest)
                    widest = width;
            }


            load.LoadedHeight += highest;

            if (load.LoadedHeight < SingerConfigs.MinLoadHeight)
                load.LoadedHeight = SingerConfigs.MinLoadHeight;

            if (widest > load.LoadedWidth)
                load.LoadedWidth = widest;

            if (longest > load.LoadedLength)
                load.LoadedLength = longest;
        }

        private void cmbRates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbTrailerCombinations.ItemsSource = from tc in Database.TrailerCombinations where tc.Rate == cmbRates.SelectedItem select tc;
        }

        private System.Collections.IEnumerable GetCompanyRates(Company company)
        {
            if (company == null)
            {
                return null;
            }

            var rates = from r in Database.Rates where r.RateType.Name == "Trailer" select r;
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

            return rates;
        }

        private void AddReferenceNumber_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveReferenceNumber_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
