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
            if (InDesignMode()) return;
            
            UpdateComboBoxes();
            UpdateExtras();
            UpdateReferenceNumbers();
        }

        protected override void SelectedLoadChanged(Load newValue, Load oldValue)
        {
            base.SelectedLoadChanged(newValue, oldValue);

            UpdateComboBoxes();
            UpdateExtras();            
        }

        private void UpdateComboBoxes()
        {
            cmbStatuses.ItemsSource = from s in Database.Statuses select s;
        }

        private void UpdateReferenceNumbers()
        {
            if (SelectedLoad != null)
            {
                dgReferenceNumbers.MaxHeight = dgReferenceNumbers.ActualHeight;
                dgReferenceNumbers.ItemsSource = new ObservableCollection<LoadReferenceNumber>(SelectedLoad.ReferenceNumbers);
            }
            else
            {
                dgReferenceNumbers.ItemsSource = null;
            }
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
                        
            if (load.TrailerCombination != null)
            {   
                load.LoadedWidth = load.TrailerCombination.Width ?? 0.0;
                load.LoadedLength = load.TrailerCombination.Length ?? 0.0;
                load.LoadedHeight = load.TrailerCombination.Height ?? 0.0;
            }

            var widest = 0.0;
            var longest = 0.0;
            var highest = 0.0;

            foreach (var commodity in load.LoadedCommodities)
            {
                var length = commodity.JobCommodity.Length ?? 0.0;
                var height = commodity.JobCommodity.Height ?? 0.0;
                var width = commodity.JobCommodity.Width ?? 0.0;

                if (length > longest)
                    longest = length;

                if (height > highest)
                    highest = height;

                if (width > widest)
                    widest = width;
            }


            load.LoadedHeight += highest;

            if (load.LoadedHeight < SingerConfigs.MinLoadHeight)
                load.LoadedHeight = SingerConfigs.MinLoadHeight;

            if (widest > load.LoadedWidth)
                load.LoadedWidth = widest;

            if (longest > load.LoadedLength)
                load.LoadedLength = longest;
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
    }

    public class ExtraEquipmentTypeDropList : ObservableCollection<ExtraEquipmentType>
    {
    }
}
