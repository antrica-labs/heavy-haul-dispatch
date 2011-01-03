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

namespace SingerDispatch.Panels.Loads
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

            if (InDesignMode()) return;

            Database = SingerConfigs.CommonDataContext;

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
            if (InDesignMode()) return;

            if (dgSwampers.ActualHeight > 0.0) 
                dgSwampers.MaxHeight = dgSwampers.ActualHeight;
            
            if (dgOutOfProvince.ActualHeight > 0.0)
                dgOutOfProvince.MaxHeight = dgOutOfProvince.ActualHeight;

            cmbEquipmentTypes.ItemsSource = (SelectedLoad == null) ? null : (from et in Database.EquipmentTypes orderby et.Prefix select et).ToList();
            cmbEmployees.ItemsSource = (SelectedLoad == null) ? null : from emp in Database.Employees where emp.Archived != true orderby emp.FirstName, emp.LastName select emp;
            cmbDispatchedByEmployees.ItemsSource = (SelectedLoad == null) ? null : from emp in Database.Employees where emp.Archived != true orderby emp.FirstName, emp.LastName select emp;

            var provider = (ObjectDataProvider)FindResource("EmployeeDropList");

            if (provider != null)
            {
                var employees = from emp in Database.Employees where emp.Archived != true orderby emp.FirstName, emp.LastName select emp;
                var list = (EmployeeDropList)provider.Data;

                list.Clear();

                foreach (var person in employees)
                {
                    list.Add(person);
                }
            }
        }

        protected override void SelectedLoadChanged(Load newValue, Load oldValue)
        {
            base.SelectedLoadChanged(newValue, oldValue);

            dgDispatches.ItemsSource = (newValue == null) ? null : new ObservableCollection<Dispatch>(newValue.Dispatches);
        }

        private void NewDispatch_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedLoad == null) return;

            var list = (ObservableCollection<Dispatch>)dgDispatches.ItemsSource;
            var dispatch = new Dispatch { LoadID = SelectedLoad.ID, DispatchedBy = SingerConfigs.OperatingEmployee };

            if (!String.IsNullOrEmpty(SelectedLoad.Schedule))
            {   
                dispatch.Description = string.Format(SingerConfigs.DefaultDispatchDescription, Load.PrintCommodityList(SelectedLoad));
                dispatch.Schedule = SelectedLoad.Schedule;
            }

            if (SelectedLoad.Dispatches.Count == 0 && SelectedLoad.Equipment != null)
            {
                // Assume they want a dispatch for the tractor pulling this load
                dispatch.EquipmentType = SelectedLoad.Equipment.EquipmentType;
                dispatch.Equipment = SelectedLoad.Equipment;
                dispatch.Employee = dispatch.Equipment.DefaultDriver;
            }

            SelectedLoad.Dispatches.Add(dispatch);
            list.Add(dispatch);
            dgDispatches.SelectedItem = dispatch;
            dgDispatches.ScrollIntoView(dispatch);

            try
            {
                EntityHelper.SaveAsNewDispatch(dispatch, Database);

                cmbDispatchedByEmployees.Focus();
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }            
        }

        private void CopyDispatch_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<Dispatch>)dgDispatches.ItemsSource;
            var dispatch = (Dispatch)dgDispatches.SelectedItem;

            if (dispatch == null)
                return;

            dispatch = dispatch.Duplicate();
            dispatch.DispatchedBy = SingerConfigs.OperatingEmployee;

            SelectedLoad.Dispatches.Add(dispatch);
            list.Add(dispatch);

            dgDispatches.ScrollIntoView(dispatch);
            dgDispatches.SelectedItem = dispatch;

            try
            {
                EntityHelper.SaveAsNewDispatch(dispatch, Database);

                cmbDispatchedByEmployees.Focus();
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }

        private void ViewDispatch_Click(object sender, RoutedEventArgs e)
        {
            var dispatch = (Dispatch)dgDispatches.SelectedItem;

            if (dispatch == null) return;

            bool printFileCopy;

            if (SelectedCompany.CustomerType.IsEnterprise == true)
            {
                printFileCopy = Convert.ToBoolean(SingerConfigs.GetConfig("Dispatch-EnterprisePrintFileCopy") ?? "false");
            }
            else
            {
                printFileCopy = Convert.ToBoolean(SingerConfigs.GetConfig("Dispatch-SingerPrintFileCopy") ?? "false");
            }

            var viewer = new DocumentViewerWindow(new DispatchDocument(printFileCopy), dgDispatches.SelectedItem, String.Format("Dispatch #{0}", dispatch.Name)) { IsMetric = !UseImperialMeasurements, IsSpecializedDocument = SelectedCompany.CustomerType.IsEnterprise != true };
            viewer.DisplayPrintout();
        }

        private void RemoveDispatch_Click(object sender, RoutedEventArgs e)
        {
            if (dgDispatches.SelectedItem == null) return;

            var confirmation = MessageBox.Show(SingerConfigs.DefaultRemoveItemMessage, "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            var dispatch = (Dispatch)dgDispatches.SelectedItem;

            SelectedLoad.Dispatches.Remove(dispatch);
            ((ObservableCollection<Dispatch>)dgDispatches.ItemsSource).Remove(dispatch);
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
                        
            dispatch.Swampers.Remove(swamper);
            ((ObservableCollection<Swamper>)dgSwampers.ItemsSource).Remove(swamper);
        }

        private void dgDispatches_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {            
            var dispatch = (Dispatch)dgDispatches.SelectedItem;

            dgOutOfProvince.ItemsSource = (dispatch == null) ? null : new ObservableCollection<OutOfProvinceTravel>(dispatch.OutOfProvinceTravels);
            dgSwampers.ItemsSource = (dispatch == null) ? null : new ObservableCollection<Swamper>(dispatch.Swampers);
        }
        
        private void AssignDriver_Click(object sender, RoutedEventArgs e)
        {
            var dispatch = (Dispatch)dgDispatches.SelectedItem;

            if (dispatch == null || dispatch.Equipment == null) return;

            dispatch.Employee = dispatch.Equipment.DefaultDriver;
        }

        private void AutoFillOPT_Click(object sender, RoutedEventArgs e)
        {
            var dispatch = (Dispatch)dgDispatches.SelectedItem;

            if (dispatch == null || dispatch.Equipment == null || dispatch.Equipment.EquipmentClass.Name != "Tractor") return;

            // If this is a tractor, try and fill out the out of province distance.
            // Go through the loaded commodities and add any out of province that can be found
            var commodities = SelectedLoad.LoadedCommodities.Where(l => (l.LoadingProvince != null && l.LoadingProvince.Abbreviation != "AB") || (l.UnloadingProvince != null && l.UnloadingProvince.Abbreviation != "AB"));
            foreach (var commodity in commodities)
            {
                var travels = from t in dispatch.OutOfProvinceTravels select t.ProvinceOrState;

                if (commodity.LoadingProvince != null && commodity.LoadingProvince.Abbreviation != "AB" && !travels.Contains(commodity.LoadingProvince))
                {
                    var opt = new OutOfProvinceTravel { ProvinceOrState = commodity.LoadingProvince, Distance = 0 };
                    ((ObservableCollection<OutOfProvinceTravel>)dgOutOfProvince.ItemsSource).Add(opt);
                    dispatch.OutOfProvinceTravels.Add(opt);
                }

                if (commodity.UnloadingProvince != null && commodity.UnloadingProvince.Abbreviation != "AB" && !travels.Contains(commodity.UnloadingProvince))
                {
                    var opt = new OutOfProvinceTravel { ProvinceOrState = commodity.UnloadingProvince, Distance = 0 };
                    ((ObservableCollection<OutOfProvinceTravel>)dgOutOfProvince.ItemsSource).Add(opt);
                    dispatch.OutOfProvinceTravels.Add(opt);
                }
            }
        }
    }

    public class EmployeeDropList : ObservableCollection<Employee>
    {
    }
    
    public class ProvStateDropList : ObservableCollection<ProvincesAndState>
    {
    }
}
