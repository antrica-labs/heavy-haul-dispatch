using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using SingerDispatch.Controls;
using System.Windows.Controls;

namespace SingerDispatch.Panels.Admin
{
    /// <summary>
    /// Interaction logic for EmployeesControl.xaml
    /// </summary>
    public partial class EmployeesControl
    {
        public static DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(Employee), typeof(EmployeesControl), new PropertyMetadata(null, SelectedItemPropertyChanged));
        public SingerDispatchDataContext Database { get; set; }

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

            Database = SingerConfigs.CommonDataContext;            
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
            var query = from emp in Database.Employees orderby emp.FirstName, emp.LastName select emp;
            var employees = from emp in query where emp.Archived != true select emp;

            dgEmployees.ItemsSource = new ObservableCollection<Employee>(employees);
        }

        private void UpdateArchivedEmployees()
        {
            var query = from emp in Database.Employees orderby emp.FirstName, emp.LastName select emp;
            var employees = from emp in query where emp.Archived == true select emp;

            dgArchivedEmployees.ItemsSource = new ObservableCollection<Employee>(employees);
        }

        private void NewEmployee_Click(object sender, RoutedEventArgs e)
        {
            var employees = (ObservableCollection<Employee>)dgEmployees.ItemsSource;
            var employee = new Employee();

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

            if (tb.SelectedIndex == 0)
            {
                UpdateCurrentEmployees();
                SelectedItem = (Employee)dgEmployees.SelectedItem;
            }
            else if (tb.SelectedIndex == 1)
            {
                UpdateArchivedEmployees();
                SelectedItem = (Employee)dgArchivedEmployees.SelectedItem;
            }       
        }

        
    }
}
