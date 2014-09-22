using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using SingerDispatch.Controls;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Threading;
using System;

namespace SingerDispatch.Panels.Admin
{
    /// <summary>
    /// Interaction logic for EmployeesControl.xaml
    /// </summary>
    public partial class EmployeesControl
    {
        private BackgroundWorker MainGridWorker;
        private BackgroundWorker ArchiveGridWorker;

        public static DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(Employee), typeof(EmployeesControl), new PropertyMetadata(null, SelectedItemPropertyChanged));        

        public Employee SelectedItem
        {
            get
            {
                return (Employee)GetValue(SelectedItemProperty);
            }
            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        public EmployeesControl()
        {
            InitializeComponent();
            
            if (InDesignMode()) return;

            Database = new SingerDispatchDataContext();

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
                UpdateCurrentEmployees();
            else if (currentOrArchivedTabs.SelectedIndex == 1)
                UpdateArchivedEmployees();
            
        }

        protected static void SelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (EmployeesControl)d;

            control.SelectedItemChanged((Employee)e.NewValue, (Employee)e.OldValue);
        }

        private void SelectedItemChanged(Employee newValue, Employee oldValue)
        {
        }

        private void UpdateCurrentEmployees()
        {
            if (MainGridWorker.IsBusy)
                return;
                    
            dgEmployees.ItemsSource = new ObservableCollection<Employee>();
            MainGridWorker.RunWorkerAsync();        
        }

        private void UpdateArchivedEmployees()
        {
            if (ArchiveGridWorker.IsBusy)
                return;
            
            dgArchivedEmployees.ItemsSource = new ObservableCollection<Employee>();
            ArchiveGridWorker.RunWorkerAsync();
        }

        private void SetDataGridAvailability(bool isAvailable)
        {
            dgEmployees.IsEnabled = isAvailable;
        }

        private void AddEmployeeToMainGrid(Employee emp)
        {
            var list = dgEmployees.ItemsSource as ObservableCollection<Employee>;

            list.Add(emp);
        }

        private void AddEmployeeToArchiveGrid(Employee emp)
        {
            var list = dgArchivedEmployees.ItemsSource as ObservableCollection<Employee>;

            list.Add(emp);
        }

        private void FillMainGridAsync(object sender, DoWorkEventArgs e)
        {
            var async = sender as BackgroundWorker;

            if (async.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            FillDataGridAsync(async, (ObservableCollection<Employee>)dgEmployees.ItemsSource, false);

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

            FillDataGridAsync(async, (ObservableCollection<Employee>)dgArchivedEmployees.ItemsSource, true);

            if (async.CancellationPending)
                e.Cancel = true;
        }

        private void FillDataGridAsync(BackgroundWorker thread, ObservableCollection<Employee> list, bool archived)
        {
            if (thread.CancellationPending)
                return;

            Dispatcher.Invoke(DispatcherPriority.Render, new Action<bool>(SetDataGridAvailability), false);

            var employees = (from emp in Database.Employees where emp.Archived == archived orderby emp.FirstName, emp.LastName select emp).ToList();

            foreach (var emp in employees)
            {
                if (thread.CancellationPending)
                    break;

                if (archived)
                    Dispatcher.Invoke(DispatcherPriority.Render, new Action<Employee>(AddEmployeeToArchiveGrid), emp);
                else
                    Dispatcher.Invoke(DispatcherPriority.Render, new Action<Employee>(AddEmployeeToMainGrid), emp);
            }

            Dispatcher.Invoke(DispatcherPriority.Render, new Action<bool>(SetDataGridAvailability), true);
        }

        private void NewEmployee_Click(object sender, RoutedEventArgs e)
        {
            var employees = (ObservableCollection<Employee>)dgEmployees.ItemsSource;
            var employee = new Employee() { Archived = false, IsAvailable = true, IsSingerStaff = true, IsSupervisor = false, StartDate = DateTime.Now };

            employees.Add(employee);
            Database.Employees.InsertOnSubmit(employee);
            dgEmployees.ScrollIntoView(employee);
            dgEmployees.SelectedItem = employee;                     

            txtFirstName.Focus();
        }

        private void ArchiveEmployee_Click(object sender, RoutedEventArgs e)
        {
            var employee = SelectedItem;
            var employees = (ObservableCollection<Employee>)dgEmployees.ItemsSource;
           
            if (employee == null) return;

            MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to archive this employee?", "Archive confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;
                        
            try
            {
                employee.Archived = true;

                Database.SubmitChanges();

                employees.Remove(employee);
            }
            catch (System.Exception ex)
            {
                Windows.NoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);

                Database.RevertChanges();
            }
        }

        private void ReinstateEmployee_Click(object sender, RoutedEventArgs e)
        {
            var employee = SelectedItem;
            var employees = (ObservableCollection<Employee>)dgArchivedEmployees.ItemsSource;

            if (employee == null) return;

            try
            {
                employee.Archived = false;

                Database.SubmitChanges();

                employees.Remove(employee);
            }
            catch (System.Exception ex)
            {
                Windows.NoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);

                Database.RevertChanges();
            }
        }

        private void EmployeesGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var grid = (DataGrid)sender;
            var item = (Employee)grid.SelectedItem;

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
                    UpdateCurrentEmployees();
                    break;
                case 1:
                    UpdateArchivedEmployees();
                    break;
            }
        }

        
    }
}
