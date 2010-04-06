using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using SingerDispatch.Controls;

namespace SingerDispatch.Panels.Admin
{
    /// <summary>
    /// Interaction logic for EquipmentControl.xaml
    /// </summary>
    public partial class EquipmentControl
    {
        private CommandBinding SaveCommand { get; set; }

        public SingerDispatchDataContext Database { get; set; }

        public EquipmentControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
            SaveCommand = new CommandBinding(CustomCommands.GenericSaveCommand);

            cmbEquipmentClasses.ItemsSource = from ec in Database.EquipmentClasses select ec;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SaveCommand.Executed += CommitChanges_Executed;

            cmbEmployees.ItemsSource = from emp in Database.Employees orderby emp.FirstName, emp.LastName select emp;            
            dgEquipment.ItemsSource = new ObservableCollection<Equipment>(from equip in Database.Equipment select equip);            
        }

        private void NewEquipment_Click(object sender, RoutedEventArgs e)
        {
            var unit = new Equipment();

            ((ObservableCollection<Equipment>)dgEquipment.ItemsSource).Insert(0, unit);
            Database.Equipment.InsertOnSubmit(unit);
            dgEquipment.SelectedItem = unit;
            dgEquipment.ScrollIntoView(unit);

            txtUnitNumber.Focus();
        }

        private void RemoveEquipment_Click(object sender, RoutedEventArgs e)
        {
            var unit = (Equipment)dgEquipment.SelectedItem;

            if (unit == null) return;

            MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to remove this unit?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            
            
            

            try
            {
                Database.Equipment.DeleteOnSubmit(unit);
                Database.SubmitChanges();

                ((ObservableCollection<Equipment>)dgEquipment.ItemsSource).Remove(unit);
            }
            catch (System.Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting to remove equipment", ex.Message);
            }

        }


        private void SaveEquipment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Database.SubmitChanges();
            }
            catch (System.Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting to save equipment", ex.Message);
            }
        }

        private void EquipmentClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var equipmentClass = (EquipmentClass)cmbEquipmentClasses.SelectedItem;

            if (equipmentClass != null && equipmentClass.Name == "Tractor")
            {
                gbTractorInfo.IsEnabled = true;
            }
            else
            {
                gbTractorInfo.IsEnabled = false;
            }
        }

        private void CommitChanges_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CommitChangesButton.Focus();
            CommitChangesButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, CommitChangesButton));
        }
        
    }
}
