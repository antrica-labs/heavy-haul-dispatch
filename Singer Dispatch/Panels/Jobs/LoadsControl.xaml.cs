using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace SingerDispatch.Panels.Jobs
{
    /// <summary>
    /// Interaction logic for LoadsControl.xaml
    /// </summary>
    public partial class LoadsControl : JobUserControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public LoadsControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            cmbWheelTypes.ItemsSource = (from wt in Database.WheelTypes select wt).ToList();
            cmbUnits.ItemsSource = (from u in Database.Equipments where u.EquipmentClass.Name == "Tractor" || u.EquipmentClass.Name == "Trailor" select u).ToList();
            cmbSeasons.ItemsSource = (from s in Database.Seasons select s).ToList();
            cmbTrailerCombinations.ItemsSource = (from tc in Database.TrailerCombinations select tc).ToList();
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);

            dgLoads.ItemsSource = new ObservableCollection<Load>((from l in Database.Loads where l.Job == newValue select l).ToList());
        }

        private void NewLoad_Click(object sender, RoutedEventArgs e)
        {
            var load = new Load { JobID = SelectedJob.ID };

            SelectedJob.Loads.Add(load);
            ((ObservableCollection<Load>)dgLoads.ItemsSource).Insert(0, load);
            dgLoads.SelectedItem = load;

            cmbUnits.Focus();
        }

        private void AxleWeightChanged(object sender, TextChangedEventArgs e)
        {
            var load = (Load)dgLoads.SelectedItem;

            if (load == null)
            {
                return;
            }
            
            int? gross = 0;

            gross += load.WeightSteer;
            gross += load.WeightDrive;
            gross += load.WeightGroup1;
            gross += load.WeightGroup2;
            gross += load.WeightGroup3;
            gross += load.WeightGroup4;
            gross += load.WeightGroup5;
            gross += load.WeightGroup6;
            gross += load.WeightGroup7;
            gross += load.WeightGroup8;
            gross += load.WeightGroup9;
            gross += load.WeightGroup10;

            load.GrossWeight = gross;
        }

        private void RemoveLoad_Click(object sender, RoutedEventArgs e)
        {
            
        }

        
    }
}
