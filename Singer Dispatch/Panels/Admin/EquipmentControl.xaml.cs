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
        public static DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(Equipment), typeof(EquipmentControl), new PropertyMetadata(null, SelectedItemPropertyChanged));
        public SingerDispatchDataContext Database { get; set; }

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

            Database = SingerConfigs.CommonDataContext;

            cmbEquipmentTypes.ItemsSource = from et in Database.EquipmentTypes orderby et.Prefix select et;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode() || IsVisible == false) return;

            cmbEmployees.ItemsSource = from emp in Database.Employees where emp.Archived != true orderby emp.FirstName, emp.LastName select emp;

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
            var query = from eq in Database.Equipment orderby eq.UnitNumber select eq;
            var equipments = from eq in query where eq.Archived != true select eq;

            dgEquipment.ItemsSource = new ObservableCollection<Equipment>(equipments);
        }

        private void UpdateArchivedEquipment()
        {
            var query = from eq in Database.Equipment orderby eq.UnitNumber select eq;
            var equipments = from eq in query where eq.Archived == true select eq;

            dgArchivedEquipment.ItemsSource = new ObservableCollection<Equipment>(equipments);
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

            if (tb.SelectedIndex == 0)
            {
                UpdateCurrentEquipment();
                SelectedItem = (Equipment)dgEquipment.SelectedItem;
            }
            else if (tb.SelectedIndex == 1)
            {
                UpdateArchivedEquipment();
                SelectedItem = (Equipment)dgArchivedEquipment.SelectedItem;
            }
        }
        
    }
}
