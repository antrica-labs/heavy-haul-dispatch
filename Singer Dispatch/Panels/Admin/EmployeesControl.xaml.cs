using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using SingerDispatch.Controls;

namespace SingerDispatch.Panels.Admin
{
    /// <summary>
    /// Interaction logic for EmployeesControl.xaml
    /// </summary>
    public partial class EmployeesControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public EmployeesControl()
        {
            InitializeComponent();
            
            if (InDesignMode()) return;

            Database = SingerConfigs.CommonDataContext;            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode()) return;

            dgEmployees.ItemsSource = new ObservableCollection<Employee>(from emp in Database.Employees where emp.Archived != true orderby emp.FirstName, emp.LastName select emp);            
        }

        private void NewEmployee_Click(object sender, RoutedEventArgs e)
        {
            var employees = (ObservableCollection<Employee>)dgEmployees.ItemsSource;
            var employee = new Employee();

            employees.Add(employee);
            Database.Employees.InsertOnSubmit(employee);
            dgEmployees.SelectedItem = employee;
            dgEmployees.ScrollIntoView(employee);

            txtFirstName.Focus();            
        }

        private void RemoveEmployee_Click(object sender, RoutedEventArgs e)
        {
            var employee = (Employee)dgEmployees.SelectedItem;
            var employees = (ObservableCollection<Employee>)dgEmployees.ItemsSource;

            if (employee == null) return;

            MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to remove this employee?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

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
    }
}
