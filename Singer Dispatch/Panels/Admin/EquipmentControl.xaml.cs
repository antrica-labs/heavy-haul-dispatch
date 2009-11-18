using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace SingerDispatch.Panels.Admin
{
    /// <summary>
    /// Interaction logic for EquipmentControl.xaml
    /// </summary>
    public partial class EquipmentControl : UserControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public EquipmentControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;

            cmbEquipmentClasses.ItemsSource = from ec in Database.EquipmentClasses select ec;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            cmbEmployees.ItemsSource = from emp in Database.Employees orderby emp.FirstName select emp;
            dgEquipment.MaxHeight = gbDetails.ActualHeight;
            dgEquipment.ItemsSource = new ObservableCollection<Equipment>(from equip in Database.Equipment select equip);
        }

        private void NewEquipment_Click(object sender, RoutedEventArgs e)
        {
            var unit = new Equipment();

            ((ObservableCollection<Equipment>)dgEquipment.ItemsSource).Insert(0, unit);
            Database.Equipment.InsertOnSubmit(unit);
            dgEquipment.SelectedItem = unit;
            txtUnitNumber.Focus();
        }

        private void RemoveEquipment_Click(object sender, RoutedEventArgs e)
        {
            var unit = (Equipment)dgEquipment.SelectedItem;

            if (unit == null)
            {
                return;
            }

            MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to remove this unit?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes)
            {
                return;
            }

            ((ObservableCollection<Equipment>)dgEquipment.ItemsSource).Remove(unit);
            Database.Equipment.DeleteOnSubmit(unit);
            dgEquipment.SelectedItem = null;
            Database.SubmitChanges();
        }


        private void SaveEquipment_Click(object sender, RoutedEventArgs e)
        {
            Database.SubmitChanges();
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

        
    }
}
