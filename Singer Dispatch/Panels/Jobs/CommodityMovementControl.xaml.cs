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

namespace SingerDispatch.Panels.Jobs
{
    /// <summary>
    /// Interaction logic for CommodityMovementControl.xaml
    /// </summary>
    public partial class CommodityMovementControl : JobUserControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public CommodityMovementControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            if (SelectedJob != null)
            {                
                cmbCommodityName.ItemsSource = (from c in Database.Commodities where c.Company == SelectedJob.Company || c.Company == SelectedJob.Company1 select c).ToList();                
            }
            else
            {                
                cmbCommodityName.ItemsSource = null;
            }
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);

            dgCommodities.ItemsSource = new ObservableCollection<JobCommodity>((from jc in Database.JobCommodities where jc.Job == SelectedJob select jc).ToList());
        }

        private void NewCommodity_Click(object sender, RoutedEventArgs e)
        {
            var commodity = new JobCommodity { JobID = SelectedJob.ID };
            var list = (ObservableCollection<JobCommodity>)dgCommodities.ItemsSource;
                
            list.Insert(0, commodity);
            SelectedJob.JobCommodities.Add(commodity);
            dgCommodities.SelectedItem = commodity;

            cmbCommodityName.Focus();
        }

        private void RemoveCommodity_Click(object sender, RoutedEventArgs e)
        {
            var commodity = (JobCommodity)dgCommodities.SelectedItem;

            if (commodity == null)
            {
                return;
            }

            MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to remove this commodity?", "Remove commodity", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes)
            {
                return;
            }

            SelectedJob.JobCommodities.Remove(commodity);
            ((ObservableCollection<JobCommodity>)dgCommodities.ItemsSource).Remove(commodity);
        }

        private void cmbCommodityName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var original = (Commodity)cmbCommodityName.SelectedItem;
            var commodity = (JobCommodity)dgCommodities.SelectedItem;

            if (commodity != null)
            {
                return;
            }

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
            }
        }
    }
}
