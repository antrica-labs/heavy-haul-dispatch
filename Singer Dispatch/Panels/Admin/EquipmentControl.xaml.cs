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
        public SingerDispatchDataContext Database { get; set; }

        public EquipmentControl()
        {
            InitializeComponent();
            
            if (InDesignMode()) return;

            Database = SingerConfigs.CommonDataContext;
            cmbEquipmentClasses.ItemsSource = from ec in Database.EquipmentClasses select ec;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode()) return;

            cmbEmployees.ItemsSource = from emp in Database.Employees where emp.Archived != true orderby emp.FirstName, emp.LastName select emp;            
            dgEquipment.ItemsSource = new ObservableCollection<Equipment>(from equip in Database.Equipment where equip.Archived != true orderby equip.UnitNumber select equip);            
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

        private void RemoveEquipment_Click(object sender, RoutedEventArgs e)
        {
            var unit = (Equipment)dgEquipment.SelectedItem;

            if (unit == null) return;

            MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to remove this unit?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            try
            {
                unit.Archived = true;
                
                Database.SubmitChanges();

                ((ObservableCollection<Equipment>)dgEquipment.ItemsSource).Remove(unit);
            }
            catch (System.Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting to remove equipment", ex.Message);
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
        
    }
}
