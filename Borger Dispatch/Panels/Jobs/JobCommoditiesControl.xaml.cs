﻿using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using SingerDispatch.Windows;
using SingerDispatch.Printing.Documents;
using System.Windows.Input;
using System;
using SingerDispatch.Database;

namespace SingerDispatch.Panels.Jobs
{
    /// <summary>
    /// Interaction logic for CommodityMovementControl.xaml
    /// </summary>
    public partial class JobCommoditiesControl
    {
        public static DependencyProperty CommonSiteNamesProperty = DependencyProperty.Register("CommonSiteNames", typeof(ObservableCollection<string>), typeof(JobCommoditiesControl));
        public static DependencyProperty CommonSiteAddressesProperty = DependencyProperty.Register("CommonSiteAddresses", typeof(ObservableCollection<string>), typeof(JobCommoditiesControl));

        public ObservableCollection<string> CommonSiteNames
        {
            get
            {
                return (ObservableCollection<string>)GetValue(CommonSiteNamesProperty);
            }
            set
            {
                SetValue(CommonSiteNamesProperty, value);
            }
        }

        public ObservableCollection<string> CommonSiteAddresses
        {
            get
            {
                return (ObservableCollection<string>)GetValue(CommonSiteAddressesProperty);
            }
            set
            {
                SetValue(CommonSiteAddressesProperty, value);
            }
        }

        public JobCommoditiesControl()
        {
            InitializeComponent();

            CommonSiteNames = new ObservableCollection<string>();
            CommonSiteAddresses = new ObservableCollection<string>();

            if (InDesignMode()) return;
            
            Database = SingerConfigs.CommonDataContext;
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode() || IsVisible == false) return;

            if (dgRecordedCommodities.ActualHeight > 0.0)
            {
                dgRecordedCommodities.MaxHeight = dgRecordedCommodities.ActualHeight;
                dgRecordedCommodities.ItemsSource = (SelectedJob == null) ? null : new ObservableCollection<Commodity>(from c in Database.Commodities where c.Company == SelectedJob.Company || c.Company == SelectedJob.CareOfCompany orderby c.Name, c.Unit select c);
            }

            UpdateOwnersList();
            UpdateAddressesAndSites();
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);

            dgCommodities.ItemsSource = (newValue == null) ? null : new ObservableCollection<JobCommodity>(newValue.JobCommodities);
        }

        private void UpdateOwnersList()
        {
            if (SelectedJob == null) return;

            var list = new List<Company>();

            list.Add(SelectedJob.Company);

            if (SelectedJob.CareOfCompany != null)
                list.Add(SelectedJob.CareOfCompany);

            cmbOwners.ItemsSource = list;
        }

        private void UpdateAddressesAndSites()
        {
            var list = (ObservableCollection<JobCommodity>)dgCommodities.ItemsSource;

            if (list == null) return;

            foreach (var item in list)
            {
                if (!string.IsNullOrWhiteSpace(item.DepartureAddress) && !CommonSiteAddresses.Contains(item.DepartureAddress))
                    CommonSiteAddresses.Add(item.DepartureAddress);
                if (!string.IsNullOrWhiteSpace(item.ArrivalAddress) && !CommonSiteAddresses.Contains(item.ArrivalAddress))
                    CommonSiteAddresses.Add(item.ArrivalAddress);
                if (!string.IsNullOrWhiteSpace(item.DepartureSiteName) && !CommonSiteNames.Contains(item.DepartureSiteName))
                    CommonSiteNames.Add(item.DepartureSiteName);
                if (!string.IsNullOrWhiteSpace(item.ArrivalSiteName) && !CommonSiteNames.Contains(item.ArrivalSiteName))
                    CommonSiteNames.Add(item.ArrivalSiteName);
            }

            List<Address> knownAddresses;

            if (SelectedJob != null && SelectedJob.CareOfCompany != null)
                knownAddresses = (from a in Database.Addresses where a.Company == SelectedCompany || a.Company == SelectedJob.CareOfCompany select a).ToList();                
            else
                knownAddresses = (from a in Database.Addresses where a.Company == SelectedCompany select a).ToList();
                

            foreach (var item in knownAddresses)
            {
                if (!CommonSiteAddresses.Contains(item.ToString()))
                    CommonSiteAddresses.Add(item.ToString());
            }
        }

        private void NewCommodity_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob == null) return;

            var commodity = new JobCommodity { Job = SelectedJob, Owner = SelectedJob.Company };
            var list = (ObservableCollection<JobCommodity>)dgCommodities.ItemsSource;
                        
            SelectedJob.JobCommodities.Add(commodity);
            list.Add(commodity);
            dgCommodities.SelectedItem = commodity;
            dgCommodities.ScrollIntoView(commodity);

            txtCommodityName.Focus();
        }

        private void DuplicateCommodity_Click(object sender, RoutedEventArgs e)
        {
            var commodity = (JobCommodity)dgCommodities.SelectedItem;
            var list = (ObservableCollection<JobCommodity>)dgCommodities.ItemsSource;

            if (commodity == null)
                return;

            commodity = commodity.Duplicate();

            SelectedJob.JobCommodities.Add(commodity);
            list.Add(commodity);
            dgCommodities.SelectedItem = commodity;
            dgCommodities.ScrollIntoView(commodity);
        }

        private void RemoveCommodity_Click(object sender, RoutedEventArgs e)
        {
            var commodity = (JobCommodity)dgCommodities.SelectedItem;

            if (commodity == null) return;

            var confirmation = MessageBox.Show(SingerConfigs.DefaultRemoveItemMessage, "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            SelectedJob.JobCommodities.Remove(commodity);
            ((ObservableCollection<JobCommodity>)dgCommodities.ItemsSource).Remove(commodity);
        }

        private void dgRecordedCommodities_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var jc = (JobCommodity)dgCommodities.SelectedItem;

            if (jc != null)
                jc.OriginalCommodity = null;
        }

        
        private void dgCommodities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAddressesAndSites();
        }

        private void dgRecordedCommodities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgRecordedCommodities.SelectedItem != null)
                dgRecordedCommodities.ScrollIntoView(dgRecordedCommodities.SelectedItem);
        }

        private void AddRecordedCommodity_Click(object sender, RoutedEventArgs e)
        {            
            var jobCommodity = (JobCommodity)dgCommodities.SelectedItem;

            if (jobCommodity == null) return;

            var confirmation = MessageBox.Show("Are you sure you want to add this job commodity as a recorded commondity? If so, please ensure that this commodity does not already exist", "Add confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            var recorded = jobCommodity.ToRecordedCommodity();

            if (jobCommodity.Owner == null) // Some job commodities lost their owner setting when moved from quote commodity to job commodity... ugh
                jobCommodity.Owner = jobCommodity.Job.Company;

            jobCommodity.Owner.Commodities.Add(recorded);
            ((ObservableCollection<Commodity>)dgRecordedCommodities.ItemsSource).Add(recorded);
            dgRecordedCommodities.ScrollIntoView(recorded);
            dgRecordedCommodities.SelectedItem = recorded;
        }

        private void MoveToStorage_Click(object sender, RoutedEventArgs e)
        {
            var jobCommodity = (JobCommodity)dgCommodities.SelectedItem;

            if (jobCommodity == null) return;

            var existing = from s in SelectedJob.StoredItems where s.JobCommodity == jobCommodity select s;

            if (existing.Count() > 0)
            {
                NoticeWindow.ShowError("Item already exists", "Item already exists in storage - Switch to the storage tab to view stored items for this job.");
                return;
            }

            var item = new StorageItem { DateEntered = DateTime.Now, Job = SelectedJob, JobCommodity = jobCommodity };

            try
            {
                EntityHelper.SaveAsNewStorageItem(item, Database);
                SelectedJob.StoredItems.Add(item);

                NoticeWindow.ShowMessage("Assigned to storage", "Commodity assigned to storage - Switch to the storage tab to view stored items for this job.");
            }
            catch (Exception ex)
            {
                NoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }

        }

        
    }
}
