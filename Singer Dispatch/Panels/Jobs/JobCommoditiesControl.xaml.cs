using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using SingerDispatch.Windows;
using SingerDispatch.Printing.Documents;

namespace SingerDispatch.Panels.Jobs
{
    /// <summary>
    /// Interaction logic for CommodityMovementControl.xaml
    /// </summary>
    public partial class JobCommoditiesControl
    {
        public static DependencyProperty CommonSiteNamesProperty = DependencyProperty.Register("CommonSiteNames", typeof(ObservableCollection<string>), typeof(JobCommoditiesControl));
        public static DependencyProperty CommonSiteAddressesProperty = DependencyProperty.Register("CommonSiteAddresses", typeof(ObservableCollection<string>), typeof(JobCommoditiesControl));

        public SingerDispatchDataContext Database { get; set; }

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
            if (InDesignMode()) return;
            
            cmbCommodityName.ItemsSource = (SelectedJob == null) ? null : from c in Database.Commodities where c.Company == SelectedJob.Company || c.Company == SelectedJob.CareOfCompany orderby c.Name, c.Unit select c;
            UpdateAddressesAndSites();
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);

            dgCommodities.ItemsSource = (newValue == null) ? null : new ObservableCollection<JobCommodity>(newValue.JobCommodities);
        }

        private void UpdateAddressesAndSites()
        {
            var list = (ObservableCollection<JobCommodity>)dgCommodities.ItemsSource;

            if (list != null)
            {
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
            }
        }

        private void NewCommodity_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob == null) return;

            var commodity = new JobCommodity { JobID = SelectedJob.ID };
            var list = (ObservableCollection<JobCommodity>)dgCommodities.ItemsSource;
                        
            SelectedJob.JobCommodities.Add(commodity);
            list.Add(commodity);
            dgCommodities.SelectedItem = commodity;
            dgCommodities.ScrollIntoView(commodity);

            cmbCommodityName.Focus();
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

        private void CommodityName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var original = (Commodity)cmbCommodityName.SelectedItem;
            var commodity = (JobCommodity)dgCommodities.SelectedItem;

            if (commodity == null || commodity.OriginalCommodity == original) return;            

            if (original != null)
            {
                commodity.OriginalCommodityID = original.ID;
                commodity.Name = original.Name;
                commodity.Value = original.Value;
                commodity.Serial = original.Serial;
                commodity.Unit = original.Unit;
                commodity.Owner = original.Owner;
                commodity.Length = original.Length;
                commodity.Width = original.Width;
                commodity.Height = original.Height;
                commodity.Weight = original.Weight;
                commodity.DimensionsEstimated = original.DimensionsEstimated;
                commodity.Notes = original.Notes;
                commodity.DepartureSiteName = original.LastLocation;
                commodity.DepartureAddress = original.LastAddress;
            }
            else
            {
                commodity.OriginalCommodityID = null;
                commodity.Name = null;
                commodity.Value = null;
                commodity.Serial = null;
                commodity.Unit = null;
                commodity.Owner = null;
                commodity.Length = null;
                commodity.Width = null;
                commodity.Height = null;
                commodity.Weight = null;
                commodity.DimensionsEstimated = null;
                commodity.Notes = null;
                commodity.DepartureSiteName = null;
                commodity.DepartureAddress = null;
                commodity.ArrivalSiteName = null;
                commodity.ArrivalAddress = null;
            }
        }

        private void dgCommodities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAddressesAndSites();
        }

        
    }
}
