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

namespace SingerDispatch.Panels.Admin
{
    /// <summary>
    /// Interaction logic for EmployeesControl.xaml
    /// </summary>
    public partial class EmployeesControl : UserControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public EmployeesControl()
        {
            InitializeComponent();
            
            Database = SingerConstants.CommonDataContext;            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgEmployees.MaxHeight = dgDetails.ActualHeight;                        
            dgEmployees.ItemsSource = new ObservableCollection<Employee>(from emp in Database.Employees select emp);
        }

        private void NewEmployee_Click(object sender, RoutedEventArgs e)
        {
            var employees = (ObservableCollection<Employee>)dgEmployees.ItemsSource;
            var employee = new Employee();

            employees.Insert(0, employee);
            Database.Employees.InsertOnSubmit(employee);
            dgEmployees.SelectedItem = employee;
            txtFirstName.Focus();
            
        }

        private void RemoveEmployee_Click(object sender, RoutedEventArgs e)
        {
            var employee = (Employee)dgEmployees.SelectedItem;
            var employees = (ObservableCollection<Employee>)dgEmployees.ItemsSource;

            if (employee == null)
            {
                return;
            }

            MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to remove this employee?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes)
            {
                return;
            }

            employees.Remove(employee);
            Database.Employees.DeleteOnSubmit(employee);
            dgEmployees.SelectedItem = null;

            Database.SubmitChanges();
        }

        private void SaveEmployee_Click(object sender, RoutedEventArgs e)
        {
            Database.SubmitChanges();
        }
    }
}
