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
using SingerDispatch.Controls;

namespace SingerDispatch.Panels.Loads
{
    /// <summary>
    /// Interaction logic for LoadEquipmentControl.xaml
    /// </summary>
    public partial class LoadEquipmentControl 
    {
        public SingerDispatchDataContext Database { get; set; }

        public LoadEquipmentControl()
        {
            InitializeComponent();

            if (InDesignMode()) return;

            Database = SingerConfigs.CommonDataContext;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode() || IsVisible == false) return;

            UpdateComboBoxes();
            UpdateExtras();
            UpdateReferenceNumbers();
        }

        protected override void SelectedLoadChanged(Load newValue, Load oldValue)
        {
            base.SelectedLoadChanged(newValue, oldValue);

            UpdateComboBoxes();
            UpdateExtras();
            UpdateReferenceNumbers();
        }

        private void UpdateComboBoxes()
        {
            cmbStatuses.ItemsSource = from s in Database.Statuses select s;
        }

        private void UpdateReferenceNumbers()
        {
            if (dgReferenceNumbers.ActualHeight > 0.0)
                dgReferenceNumbers.MaxHeight = dgReferenceNumbers.ActualHeight;

            dgReferenceNumbers.ItemsSource = (SelectedLoad == null) ? null : new ObservableCollection<LoadReferenceNumber>(SelectedLoad.ReferenceNumbers);         
        }

        private void UpdateExtras()
        {
            if (SelectedLoad == null) return;

            var provider = (ObjectDataProvider)FindResource("ExtraEquipmentTypeDropList");

            if (provider != null)
            {
                var types = from et in Database.ExtraEquipmentTypes orderby et.Name select et;
                var list = (ExtraEquipmentTypeDropList)provider.Data;

                list.Clear();

                foreach (var item in types)
                {
                    list.Add(item);
                }
            }

            dgExtraEquipment.ItemsSource = new ObservableCollection<ExtraEquipment>(SelectedLoad.ExtraEquipment);
        }

        private void AddEquipment_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedLoad == null) return;

            var equipment = new ExtraEquipment();

            SelectedLoad.ExtraEquipment.Add(equipment);
            ((ObservableCollection<ExtraEquipment>)dgExtraEquipment.ItemsSource).Add(equipment);
            dgExtraEquipment.ScrollIntoView(equipment);
            dgExtraEquipment.SelectedItem = equipment;

            DataGridHelper.EditFirstColumn(dgExtraEquipment, equipment);
        }

        private void RemoveEquipment_Click(object sender, RoutedEventArgs e)
        {
            var equipment = (ExtraEquipment)dgExtraEquipment.SelectedItem;

            SelectedLoad.ExtraEquipment.Remove(equipment);
            ((ObservableCollection<ExtraEquipment>)dgExtraEquipment.ItemsSource).Remove(equipment);
        }

        private void GuessLoadWeights_Click(object sender, RoutedEventArgs e)
        {
            // Populate as many of the estimated weights and the dimensions as you can based on the tractor, trailer combo, and commodities.
            var load = SelectedLoad;

            if (load == null) return;

            load.RecaculateDimensionsAndWeight();
        }

        private void AddReferenceNumber_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedLoad == null) return;

            var reference = new LoadReferenceNumber();

            SelectedLoad.ReferenceNumbers.Add(reference);
            ((ObservableCollection<LoadReferenceNumber>)dgReferenceNumbers.ItemsSource).Add(reference);

            DataGridHelper.EditFirstColumn(dgReferenceNumbers, reference);
        }

        private void RemoveReferenceNumber_Click(object sender, RoutedEventArgs e)
        {
            var selected = (LoadReferenceNumber)dgReferenceNumbers.SelectedItem;

            if (SelectedLoad == null || selected == null) return;

            SelectedLoad.ReferenceNumbers.Remove(selected);
            ((ObservableCollection<LoadReferenceNumber>)dgReferenceNumbers.ItemsSource).Remove(selected);
        }

        private void RecalculateGrossWeights(object sender, RoutedEventArgs e)
        {
            if (SelectedLoad == null) return;

            SelectedLoad.EGrossWeight = 0;
            SelectedLoad.EGrossWeight += SelectedLoad.EWeightSteer ?? 0.0;
            SelectedLoad.EGrossWeight += SelectedLoad.EWeightGroup1 ?? 0.0;
            SelectedLoad.EGrossWeight += SelectedLoad.EWeightGroup2 ?? 0.0;
            SelectedLoad.EGrossWeight += SelectedLoad.EWeightGroup3 ?? 0.0;
            SelectedLoad.EGrossWeight += SelectedLoad.EWeightGroup4 ?? 0.0;
            SelectedLoad.EGrossWeight += SelectedLoad.EWeightGroup5 ?? 0.0;
            SelectedLoad.EGrossWeight += SelectedLoad.EWeightGroup6 ?? 0.0;
            SelectedLoad.EGrossWeight += SelectedLoad.EWeightGroup7 ?? 0.0;
            SelectedLoad.EGrossWeight += SelectedLoad.EWeightGroup8 ?? 0.0;
            SelectedLoad.EGrossWeight += SelectedLoad.EWeightGroup9 ?? 0.0;
            SelectedLoad.EGrossWeight += SelectedLoad.EWeightGroup10 ?? 0.0;

            SelectedLoad.SGrossWeight = 0;
            SelectedLoad.SGrossWeight += SelectedLoad.SWeightSteer ?? 0.0;
            SelectedLoad.SGrossWeight += SelectedLoad.SWeightGroup1 ?? 0.0;
            SelectedLoad.SGrossWeight += SelectedLoad.SWeightGroup2 ?? 0.0;
            SelectedLoad.SGrossWeight += SelectedLoad.SWeightGroup3 ?? 0.0;
            SelectedLoad.SGrossWeight += SelectedLoad.SWeightGroup4 ?? 0.0;
            SelectedLoad.SGrossWeight += SelectedLoad.SWeightGroup5 ?? 0.0;
            SelectedLoad.SGrossWeight += SelectedLoad.SWeightGroup6 ?? 0.0;
            SelectedLoad.SGrossWeight += SelectedLoad.SWeightGroup7 ?? 0.0;
            SelectedLoad.SGrossWeight += SelectedLoad.SWeightGroup8 ?? 0.0;
            SelectedLoad.SGrossWeight += SelectedLoad.SWeightGroup9 ?? 0.0;
            SelectedLoad.SGrossWeight += SelectedLoad.SWeightGroup10 ?? 0.0;
        }
    }

    public class ExtraEquipmentTypeDropList : ObservableCollection<ExtraEquipmentType>
    {
    }
}
