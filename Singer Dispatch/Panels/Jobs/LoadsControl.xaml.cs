﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using SingerDispatch.Database;
using SingerDispatch.Windows;
using System.Collections.Generic;

namespace SingerDispatch.Panels.Jobs
{
    /// <summary>
    /// Interaction logic for LoadsControl.xaml
    /// </summary>
    public partial class LoadsControl
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

            dgLoads.ItemsSource = (newValue == null) ? null : new ObservableCollection<Load>(from l in newValue.Loads orderby l.Number select l);
        }

        private void NewLoad_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<Load>)dgLoads.ItemsSource;
            var load = new Load { Job = SelectedJob, StartDate = SelectedJob.StartDate, EndDate = SelectedJob.EndDate };

            SelectedJob.Loads.Add(load);
            list.Add(load);
            
            dgLoads.ScrollIntoView(load);
            dgLoads.SelectedItem = load;

            try
            {
                EntityHelper.SaveAsNewLoad(load, Database);
                txtServiceDescription.Focus();
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void DuplicateLoad_Click(object sender, RoutedEventArgs e)
        {
            var load = (Load)dgLoads.SelectedItem;
            var list = (ObservableCollection<Load>)dgLoads.ItemsSource;

            if (load == null)
                return;

            load = load.Duplicate();

            SelectedJob.Loads.Add(load);
            list.Add(load);

            dgLoads.ScrollIntoView(load);
            dgLoads.SelectedItem = load;            

            try
            {
                EntityHelper.SaveAsNewLoad(load, Database);
                txtServiceDescription.Focus();
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void AxleWeightChanged(object sender, TextChangedEventArgs e)
        {
            var load = (Load)dgLoads.SelectedItem;

            if (load == null)
            {
                return;
            }
            
            double? gross = 0;

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

        private void dgLoads_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lbCommodities.ItemsSource = null;

            if (dgLoads.SelectedItem == null) return;

            var load = (Load)dgLoads.SelectedItem;
            var checkboxList = new List<CheckBox>();

            foreach (var item in SelectedJob.JobCommodities)
            {
                var cb = new CheckBox { Content = item.NameAndUnit, IsChecked = (item.Load == load), DataContext = item };

                cb.Checked += CommodityCheckBox_Checked;
                cb.Unchecked += CommodityCheckBox_Unchecked;

                checkboxList.Add(cb);
            }

            lbCommodities.ItemsSource = checkboxList;
        }

        private void CommodityCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox)sender;
            var commodity = (JobCommodity)cb.DataContext;
            var load = (Load)dgLoads.SelectedItem;

            if (load == null || commodity == null) return;

            if (commodity.Load != null && commodity.Load != load)
            {
                var message = string.Format("This commodity is already assigned to load #{0}. Would you like to reassign it?", commodity.Load.Name);
                var confirmation = MessageBox.Show(message, "Commodity reassignment", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (confirmation == MessageBoxResult.Yes)
                    commodity.Load = load;
            }
            else
                commodity.Load = load;
        }

        private void CommodityCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox)sender;
            var commodity = (JobCommodity)cb.DataContext;            

            if (commodity == null) return;

            commodity.Load = null;
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

        private void cmbRates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbTrailerCombinations.ItemsSource = from tc in Database.TrailerCombinations where tc.Rate == cmbRates.SelectedItem select tc;
        }

        
    }
}
