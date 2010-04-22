using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace SingerDispatch.Panels.Jobs
{
    /// <summary>
    /// Interaction logic for CommodityMovementControl.xaml
    /// </summary>
    public partial class JobCommoditiesControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public JobCommoditiesControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            cmbLoads.ItemsSource = (SelectedJob == null) ? null : SelectedJob.Loads.ToList();
            cmbCommodityName.ItemsSource = (SelectedJob == null) ? null : from c in Database.Commodities where c.Company == SelectedJob.Company || c.Company == SelectedJob.CareOfCompany orderby c.Name, c.Unit select c;

            var methods = (SelectedJob == null) ? null : (from m in Database.LoadUnloadMethods select m).ToList();

            cmbLoadMethods.ItemsSource = methods;
            cmbUnloadMethods.ItemsSource = methods;

            List<Contact> contacts = null;

            if (SelectedJob != null && SelectedJob.CareOfCompanyID != null)
            {
                contacts = (from c in Database.Contacts where c.Address.CompanyID == SelectedCompany.ID || c.Address.CompanyID == SelectedJob.CareOfCompanyID select c).ToList();
            }
            else if (SelectedJob != null)
            {
                contacts = (from c in Database.Contacts where c.Address.CompanyID == SelectedCompany.ID select c).ToList();
            }

            cmbLoadingContacts.ItemsSource = contacts;
            cmbUnloadingContacts.ItemsSource = contacts;
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);

            dgCommodities.ItemsSource = (newValue == null) ? null : new ObservableCollection<JobCommodity>(newValue.JobCommodities);
        }

        private void NewCommodity_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob == null) return;

            var commodity = new JobCommodity { JobID = SelectedJob.ID, LoadDate = SelectedJob.StartDate, UnloadDate = SelectedJob.EndDate };
            var list = (ObservableCollection<JobCommodity>)dgCommodities.ItemsSource;
            
            SelectedJob.JobCommodities.Insert(0, commodity);
            list.Add(commodity);
            dgCommodities.SelectedItem = commodity;
            dgCommodities.ScrollIntoView(commodity);

            cmbCommodityName.Focus();
        }

        private void DuplicateCommodity_Click(object sender, RoutedEventArgs e)
        {
            var commodity = (JobCommodity)dgCommodities.SelectedItem;

            if (commodity == null)
                return;

            commodity = commodity.Duplicate();

            SelectedJob.JobCommodities.Add(commodity);
            ((ObservableCollection<JobCommodity>)dgCommodities.ItemsSource).Insert(0, commodity);
        }

        private void RemoveCommodity_Click(object sender, RoutedEventArgs e)
        {
            var commodity = (JobCommodity)dgCommodities.SelectedItem;

            if (commodity == null) return;

            var confirmation = MessageBox.Show(SingerConstants.DefaultRemoveItemMessage, "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

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
                commodity.SizeEstimated = original.SizeEstimated;
                commodity.WeightEstimated = original.WeightEstimated;
                commodity.Notes = original.Notes;
                commodity.LastAddress = original.LastAddress;
                commodity.LastLocation = original.LastLocation;
                commodity.LoadLocation = original.LastAddress;                
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
                commodity.SizeEstimated = null;
                commodity.WeightEstimated = null;
                commodity.Notes = null;
                commodity.LastAddress = null;
                commodity.LastLocation = null;
                commodity.LoadLocation = null;                
            }
        }
    }
}
