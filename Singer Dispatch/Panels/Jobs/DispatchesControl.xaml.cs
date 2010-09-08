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
            cmbEquipmentTypes.ItemsSource = (SelectedJob == null) ? null : from et in Database.EquipmentTypes orderby et.Prefix select et;
            cmbEmployees.ItemsSource = (SelectedJob == null) ? null : from emp in Database.Employees orderby emp.FirstName, emp.LastName select emp;
            cmbDispatchedByEmployees.ItemsSource = (SelectedJob == null) ? null : from emp in Database.Employees orderby emp.FirstName, emp.LastName select emp;

            var provider = (ObjectDataProvider)FindResource("EmployeeDropList");

            if (provider != null)
            {
                var employees = from emp in Database.Employees orderby emp.FirstName, emp.LastName select emp;
                var list = (EmployeeDropList)provider.Data;

                list.Clear();

                foreach (var person in employees)
                {
                    list.Add(person);
                }
            }
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
            var dispatch = new Dispatch { JobID = SelectedJob.ID, DispatchedBy = SingerConstants.OperatingEmployee };

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

            bool printFileCopy;

            if (SelectedCompany.CustomerType.IsEnterprise == true)
            {
                printFileCopy = Convert.ToBoolean(SingerConstants.GetConfig("Dispatch-EnterprisePrintFileCopy") ?? "false");
            }
            else
            {
                printFileCopy = Convert.ToBoolean(SingerConstants.GetConfig("Dispatch-SingerPrintFileCopy") ?? "false");
            }

            var viewer = new DocumentViewerWindow(new DispatchDocument(printFileCopy), dgDispatches.SelectedItem, String.Format("Dispatch #{0}", dispatch.Name)) { IsMetric = !UseImperialMeasurements, IsSpecializedDocument = SelectedCompany.CustomerType.IsEnterprise != true };
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

            var dispatch = (Dispatch)dgDispatches.SelectedItem;

            dispatch.Description = string.Format(SingerConstants.DefaultDispatchDescription, Load.PrintCommodityList(load));
            dispatch.Schedule = load.Schedule;
        }

        private void cmbEquipmentTypes_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var type = (EquipmentType)cmbEquipmentTypes.SelectedItem;

            if (type == null) return;

            var prefix = string.Format("{0}-", type.Prefix);

            cmbUnits.ItemsSource = (SelectedJob == null) ? null : from u in Database.Equipment where u.IsDispatchable == true && u.UnitNumber.StartsWith(prefix) orderby u.UnitNumber select u;
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

        private void AddSwamper_Click(object sender, RoutedEventArgs e)
        {
            var dispatch = (Dispatch)dgDispatches.SelectedItem;

            if (dispatch == null) return;

            var swamper = new Swamper { Dispatch = dispatch };

            var provider = (ObjectDataProvider)FindResource("EmployeeDropList");
            if (provider != null)
            {
                var list = (EmployeeDropList)provider.Data;
                swamper.Employee = list.First();
            }

            dispatch.Swampers.Add(swamper);
            ((ObservableCollection<Swamper>)dgSwampers.ItemsSource).Add(swamper);

            dgSwampers.ScrollIntoView(swamper);
            dgSwampers.SelectedItem = swamper;

            DataGridHelper.EditFirstColumn(dgSwampers, swamper);
        }

        private void RemoveSwamper_Click(object sender, RoutedEventArgs e)
        {
            var swamper = (Swamper)dgSwampers.SelectedItem;
            var dispatch = (Dispatch)dgDispatches.SelectedItem;

            if (swamper == null || dispatch == null) return;

            var confirmation = MessageBox.Show(SingerConstants.DefaultRemoveItemMessage, "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            dispatch.Swampers.Remove(swamper);
            ((ObservableCollection<Swamper>)dgSwampers.ItemsSource).Remove(swamper);
        }

        private void dgDispatches_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {            
            var dispatch = (Dispatch)dgDispatches.SelectedItem;

            dgOutOfProvince.ItemsSource = (dispatch == null) ? null : new ObservableCollection<OutOfProvinceTravel>(dispatch.OutOfProvinceTravels);
            dgSwampers.ItemsSource = (dispatch == null) ? null : new ObservableCollection<Swamper>(dispatch.Swampers);
        }

        
    }

    public class EmployeeDropList : ObservableCollection<Employee>
    {
    }
    
    public class ProvStateDropList : ObservableCollection<ProvincesAndState>
    {
    }
}
