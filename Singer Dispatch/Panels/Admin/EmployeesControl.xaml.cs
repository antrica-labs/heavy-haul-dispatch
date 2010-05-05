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
        private CommandBinding SaveCommand { get; set; }

        public SingerDispatchDataContext Database { get; set; }

        public EmployeesControl()
        {
            InitializeComponent();
            
            Database = SingerConstants.CommonDataContext;
            SaveCommand = new CommandBinding(CustomCommands.GenericSaveCommand);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SaveCommand.Executed += CommitChanges_Executed;
            
            dgEmployees.ItemsSource = new ObservableCollection<Employee>(from emp in Database.Employees orderby emp.FirstName, emp.LastName select emp);            
        }

        private void NewEmployee_Click(object sender, RoutedEventArgs e)
        {
            var employees = (ObservableCollection<Employee>)dgEmployees.ItemsSource;
            var employee = new Employee();

            employees.Insert(0, employee);
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
                Database.Employees.DeleteOnSubmit(employee);
                Database.SubmitChanges();

                employees.Remove(employee);
            }
            catch (System.Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void SaveEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Database.SubmitChanges();

                ((ButtonBase)sender).Focus();
            }
            catch (System.Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void CommitChanges_Executed(object sender, ExecutedRoutedEventArgs e)
        {            
            CommitChangesButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, CommitChangesButton));
        }
    }
}
