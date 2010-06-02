using System;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using SingerDispatch.Database;
using SingerDispatch.Windows;
using System.Collections.Generic;
using System.Windows.Data;
using SingerDispatch.Controls;
using SingerDispatch.Printing.Documents;

namespace SingerDispatch.Panels.Jobs
{
    /// <summary>
    /// Interaction logic for DispatchesControl.xaml
    /// </summary>
    public partial class DispatchesControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public DispatchesControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;

            var provider = (ObjectDataProvider)FindResource("ProvStateDropList");

            if (provider != null)
            {
                var regions = from ps in Database.ProvincesAndStates orderby ps.Name select ps;
                var list = (ProvStateDropList)provider.Data;

                list.Clear();

                foreach (var item in regions)
                {
                    list.Add(item);
                }
            }
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            cmbLoads.ItemsSource = (SelectedJob == null) ? null : SelectedJob.Loads.ToList();
            cmbUnits.ItemsSource = (SelectedJob == null) ? null : from u in Database.Equipment select u;
            cmbServiceTypes.ItemsSource = (SelectedJob == null) ? null : from r in Database.Rates where r.RateType.Name == "Service" select r;
            cmbEmployees.ItemsSource = (SelectedJob == null) ? null : from emp in Database.Employees orderby emp.FirstName, emp.LastName select emp;
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);

            dgDispatches.ItemsSource = (newValue == null) ? null : new ObservableCollection<Dispatch>(newValue.Dispatches);
        }

        private void NewDispatch_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob == null) return;

            var list = (ObservableCollection<Dispatch>)dgDispatches.ItemsSource;
            var dispatch = new Dispatch { JobID = SelectedJob.ID, MeetingTime = SelectedJob.StartDate, Description = "Supply men and equipment to transport " };

            SelectedJob.Dispatches.Add(dispatch);
            list.Add(dispatch);
            dgDispatches.SelectedItem = dispatch;
            dgDispatches.ScrollIntoView(dispatch);

            try
            {
                EntityHelper.SaveAsNewDispatch(dispatch, Database);

                cmbLoads.Focus();
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
            
        }

        private void DuplicateDispatch_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<Dispatch>)dgDispatches.ItemsSource;
            var dispatch = (Dispatch)dgDispatches.SelectedItem;

            if (dispatch == null)
                return;

            dispatch = dispatch.Duplicate();

            SelectedJob.Dispatches.Add(dispatch);
            list.Add(dispatch);

            dgDispatches.ScrollIntoView(dispatch);
            dgDispatches.SelectedItem = dispatch;

            try
            {
                EntityHelper.SaveAsNewDispatch(dispatch, Database);

                cmbLoads.Focus();
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void ViewDispatch_Click(object sender, RoutedEventArgs e)
        {
            var dispatch = (Dispatch)dgDispatches.SelectedItem;

            if (dispatch == null) return;

            var result = MessageBox.Show("Do you wish to inlcude a file copy with this printout?", "Include drivers copy?", MessageBoxButton.YesNo, MessageBoxImage.Question);

            var viewer = new DocumentViewerWindow(new DispatchDocument(result == MessageBoxResult.Yes), dgDispatches.SelectedItem, String.Format("Dispatch #{0}", dispatch.Name));
            viewer.DisplayPrintout();
        }

        private void RemoveDispatch_Click(object sender, RoutedEventArgs e)
        {
            if (dgDispatches.SelectedItem == null) return;

            var confirmation = MessageBox.Show(SingerConstants.DefaultRemoveItemMessage, "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            var dispatch = (Dispatch)dgDispatches.SelectedItem;

            SelectedJob.Dispatches.Remove(dispatch);
            ((ObservableCollection<Dispatch>)dgDispatches.ItemsSource).Remove(dispatch);
        }

        private void cmbLoads_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var load = (Load)cmbLoads.SelectedItem;

            if (load == null || String.IsNullOrEmpty(load.Schedule)) return;

            ((Dispatch)dgDispatches.SelectedItem).Schedule = load.Schedule;
        }

        private void AddTravel_Click(object sender, RoutedEventArgs e)
        {
            var dispatch = (Dispatch)dgDispatches.SelectedItem;

            if (dispatch == null) return;

            var travel = new OutOfProvinceTravel { Dispatch = dispatch };
            
            var provider = (ObjectDataProvider)FindResource("ProvStateDropList");
            if (provider != null)
            {
                var list = (ProvStateDropList)provider.Data;
                travel.ProvinceOrState = list.First();
            }

            dispatch.OutOfProvinceTravels.Add(travel);
            ((ObservableCollection<OutOfProvinceTravel>)dgOutOfProvince.ItemsSource).Add(travel);

            dgOutOfProvince.ScrollIntoView(travel);
            dgOutOfProvince.SelectedItem = travel;

            DataGridHelper.EditFirstColumn(dgOutOfProvince, travel);
        }

        private void RemoveTravel_Click(object sender, RoutedEventArgs e)
        {
            var travel = (OutOfProvinceTravel)dgOutOfProvince.SelectedItem;
            var dispatch = (Dispatch)dgDispatches.SelectedItem;

            if (travel == null || dispatch == null) return;

            var confirmation = MessageBox.Show(SingerConstants.DefaultRemoveItemMessage, "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            dispatch.OutOfProvinceTravels.Remove(travel);
            ((ObservableCollection<OutOfProvinceTravel>)dgOutOfProvince.ItemsSource).Remove(travel);
        }

        private void dgDispatches_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {            
            var dispatch = (Dispatch)dgDispatches.SelectedItem;

            dgOutOfProvince.ItemsSource = (dispatch == null) ? null : new ObservableCollection<OutOfProvinceTravel>(dispatch.OutOfProvinceTravels);
        }
    }

    public class ProvStateDropList : ObservableCollection<ProvincesAndState>
    {
    }
}
