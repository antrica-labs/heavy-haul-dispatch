using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using SingerDispatch.Printing;

namespace SingerDispatch.Panels.Jobs
{
    /// <summary>
    /// Interaction logic for DispatchesControl.xaml
    /// </summary>
    public partial class DispatchesControl : JobUserControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public DispatchesControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
            cmbServiceTypes.ItemsSource = from t in Database.ServiceTypes select t;
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            cmbLoads.ItemsSource = null;
            cmbUnits.ItemsSource = null;

            if (SelectedJob != null)
            {
                cmbLoads.ItemsSource = SelectedJob.Loads;
                cmbUnits.ItemsSource = from u in Database.Equipment select u;
            }
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);

            dgDispatches.ItemsSource = null;

            if (newValue != null)
            {
                dgDispatches.ItemsSource = new ObservableCollection<Dispatch>((from d in Database.Dispatches where d.Job == SelectedJob select d).ToList());
            }
        }

        private void NewDispatch_Click(object sender, RoutedEventArgs e)
        {
            var dispatch = new Dispatch { JobID = SelectedJob.ID };

            SelectedJob.Dispatches.Add(dispatch);
            ((ObservableCollection<Dispatch>)dgDispatches.ItemsSource).Insert(0, dispatch);
            dgDispatches.SelectedItem = dispatch;

            cmbLoads.Focus();
        }

        private void PrintDispatch_Click(object sender, RoutedEventArgs e)
        {
            if (dgDispatches.SelectedItem == null)
            {
                return;
            }

            var window = new DispatchRenderer();
            window.GeneratePrintout();
        }

        private void RemoveDispatch_Click(object sender, RoutedEventArgs e)
        {
            if (dgDispatches.SelectedItem == null)
            {
                return;
            }

            MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to remove this dispatch?", "Remove dispatch", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes)
            {
                return;
            }

            var dispatch = (Dispatch)dgDispatches.SelectedItem;

            SelectedJob.Dispatches.Remove(dispatch);
            ((ObservableCollection<Dispatch>)dgDispatches.ItemsSource).Remove(dispatch);
        }
    }
}
