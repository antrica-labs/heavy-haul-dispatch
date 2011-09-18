using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using SingerDispatch.Controls;
using System.ComponentModel;
using System.Windows.Threading;
using System;
using System.Collections.Generic;

namespace SingerDispatch.Panels.Admin
{
    /// <summary>
    /// Interaction logic for EquipmentControl.xaml
    /// </summary>
    public partial class EquipmentControl
    {
        private BackgroundWorker MainGridWorker;
        private BackgroundWorker ArchiveGridWorker;

        public static DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(Equipment), typeof(EquipmentControl), new PropertyMetadata(null, SelectedItemPropertyChanged));
        
        public Equipment SelectedItem
        {
            get
            {
                return (Equipment)GetValue(SelectedItemProperty);
            }
            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        public EquipmentControl()
        {
            InitializeComponent();
            
            if (InDesignMode()) return;

            Database = new SingerDispatchDataContext();

            cmbEquipmentTypes.ItemsSource = (from et in Database.EquipmentTypes orderby et.Prefix select et).ToList();

            MainGridWorker = new BackgroundWorker();
            MainGridWorker.WorkerSupportsCancellation = true;
            MainGridWorker.DoWork += FillMainGridAsync;

            ArchiveGridWorker = new BackgroundWorker();
            ArchiveGridWorker.WorkerSupportsCancellation = true;
            ArchiveGridWorker.DoWork += FillArchiveGridAsync;

            RegisterThread(MainGridWorker);
            RegisterThread(ArchiveGridWorker);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode() || IsVisible == false) return;
            
            

            if (currentOrArchivedTabs.SelectedIndex == 0)
                UpdateCurrentEquipment();
            else if (currentOrArchivedTabs.SelectedIndex == 1)
                UpdateArchivedEquipment();
        }

        protected static void SelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (EquipmentControl)d;

            control.SelectedItemChanged((Equipment)e.NewValue, (Equipment)e.OldValue);
        }

        private void SelectedItemChanged(Equipment newValue, Equipment oldValue)
        {
        }
        
        private void UpdateCurrentEquipment()
        {
            if (MainGridWorker.IsBusy)
                return;
         
            dgEquipment.ItemsSource = new ObservableCollection<Equipment>();
            MainGridWorker.RunWorkerAsync();         
        }

        private void UpdateArchivedEquipment()
        {
            if (ArchiveGridWorker.IsBusy)
                return;
            
            dgArchivedEquipment.ItemsSource = new ObservableCollection<Equipment>();
            ArchiveGridWorker.RunWorkerAsync();        
        }

        private void ResetEmployeeComboBox(IEnumerable<Employee> employees)
        {
            cmbEmployees.ItemsSource = employees;
        }

        private void SetDataGridAvailability(bool isAvailable)
        {
            dgEquipment.IsEnabled = isAvailable;
            dgArchivedEquipment.IsEnabled = isAvailable;
        }

        private void AddEquipmentToMainGrid(Equipment equip)
        {
            var list = dgEquipment.ItemsSource as ObservableCollection<Equipment>;

            list.Add(equip);
        }

        private void AddEquipmentToArchiveGrid(Equipment equip)
        {
            var list = dgArchivedEquipment.ItemsSource as ObservableCollection<Equipment>;

            list.Add(equip);
        }

        private void FillMainGridAsync(object sender, DoWorkEventArgs e)
        {
            var async = sender as BackgroundWorker;

            if (async.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            FillDataGridAsync(async, (ObservableCollection<Equipment>)dgEquipment.ItemsSource, false);

            if (async.CancellationPending)
                e.Cancel = true;
        }

        private void FillArchiveGridAsync(object sender, DoWorkEventArgs e)
        {
            var async = sender as BackgroundWorker;

            if (async.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            FillDataGridAsync(async, (ObservableCollection<Equipment>)dgArchivedEquipment.ItemsSource, true);

            if (async.CancellationPending)
                e.Cancel = true;
        }

        private void FillDataGridAsync(BackgroundWorker thread, ObservableCollection<Equipment> list, bool archived)
        {
            if (thread.CancellationPending) return;

            Dispatcher.Invoke(DispatcherPriority.Render, new Action<bool>(SetDataGridAvailability), false);

            var employees = (from emp in Database.Employees where emp.Archived != true orderby emp.FirstName, emp.LastName select emp).ToList();
            Dispatcher.Invoke(DispatcherPriority.Render, new Action<IEnumerable<Employee>>(ResetEmployeeComboBox), employees);

            if (thread.CancellationPending) return;

            var equipment = (from eq in Database.Equipment where eq.Archived == archived orderby eq.UnitNumber select eq).ToList();

            foreach (var eq in equipment)
            {
                if (thread.CancellationPending)
                    break;

                if (archived)
                    Dispatcher.Invoke(DispatcherPriority.Render, new Action<Equipment>(AddEquipmentToArchiveGrid), eq);
                else
                    Dispatcher.Invoke(DispatcherPriority.Render, new Action<Equipment>(AddEquipmentToMainGrid), eq);
            }

            Dispatcher.Invoke(DispatcherPriority.Render, new Action<bool>(SetDataGridAvailability), true);
        }

        private void NewEquipment_Click(object sender, RoutedEventArgs e)
        {
            var unit = new Equipment();

            ((ObservableCollection<Equipment>)dgEquipment.ItemsSource).Add(unit);
            Database.Equipment.InsertOnSubmit(unit);
            dgEquipment.SelectedItem = unit;
            dgEquipment.ScrollIntoView(unit);

            txtUnitNumber.Focus();
        }

        private void ArchiveEquipment_Click(object sender, RoutedEventArgs e)
        {
            var unit = SelectedItem;

            if (unit == null) return;

            MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to archive this unit?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            try
            {
                unit.Archived = true;

                Database.SubmitChanges();

                ((ObservableCollection<Equipment>)dgEquipment.ItemsSource).Remove(unit);
            }
            catch (System.Exception ex)
            {
                Windows.NoticeWindow.ShowError("Error while attempting to remove equipment", ex.Message);
            }
        }

        private void ReinstateEquipment_Click(object sender, RoutedEventArgs e)
        {
            var unit = SelectedItem;

            if (unit == null) return;

            try
            {
                unit.Archived = false;

                Database.SubmitChanges();

                ((ObservableCollection<Equipment>)dgArchivedEquipment.ItemsSource).Remove(unit);
            }
            catch (System.Exception ex)
            {
                Windows.NoticeWindow.ShowError("Error while attempting to remove equipment", ex.Message);
            }
        }

        private void cmbEquipmentTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var type = (EquipmentType)cmbEquipmentTypes.SelectedItem;

            if (type != null && type.EquipmentClass != null && type.EquipmentClass.Name == "Tractor")
                gbTractorInfo.IsEnabled = true;
            else
                gbTractorInfo.IsEnabled = false;
        }

        private void EquipmentGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var grid = (DataGrid)sender;
            var item = (Equipment)grid.SelectedItem;

            SelectedItem = null;
            SelectedItem = item;

            e.Handled = true;
        }

        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var tb = (TabControl)sender;

            SelectedItem = null;

            switch (tb.SelectedIndex)
            {
                case 0:
                    UpdateCurrentEquipment();
                    break;
                case 1:
                    UpdateArchivedEquipment();
                    break;
            }
        }
        
    }
}
