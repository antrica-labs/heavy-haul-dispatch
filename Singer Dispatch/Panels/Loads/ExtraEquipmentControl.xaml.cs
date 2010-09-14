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

namespace SingerDispatch.Panels.Loads
{
    /// <summary>
    /// Interaction logic for ExtraEquipmentControl.xaml
    /// </summary>
    public partial class ExtraEquipmentControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public ExtraEquipmentControl()
        {
            InitializeComponent();

            if (InDesignMode()) return;

            Database = SingerConfigs.CommonDataContext;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode()) return;

            UpdateExtras();
        }

        protected override void SelectedLoadChanged(Load newValue, Load oldValue)
        {
            base.SelectedLoadChanged(newValue, oldValue);

            UpdateExtras();
        }

        private void UpdateExtras()
        {
            if (SelectedLoad == null) return;
                        
            var types = (from et in Database.ExtraEquipmentTypes orderby et.Name select et).ToList();
            var list = new ObservableCollection<ExtraEquipment>(SelectedLoad.ExtraEquipment);

            lbExtraEquipmentTypes.ItemsSource = types;

            dgExtraEquipment.ItemsSource = list;
        }

        private void AddEquipment_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedLoad == null) return;

            var equipment = new ExtraEquipment();

            SelectedLoad.ExtraEquipment.Add(equipment);
            ((ObservableCollection<ExtraEquipment>)dgExtraEquipment.ItemsSource).Add(equipment);
            dgExtraEquipment.ScrollIntoView(equipment);
            dgExtraEquipment.SelectedItem = equipment;

            lbExtraEquipmentTypes.Focus();
        }

        private void RemoveEquipment_Click(object sender, RoutedEventArgs e)
        {
            var equipment = (ExtraEquipment)dgExtraEquipment.SelectedItem;

            SelectedLoad.ExtraEquipment.Remove(equipment);
            ((ObservableCollection<ExtraEquipment>)dgExtraEquipment.ItemsSource).Remove(equipment);
        }
    }
}
