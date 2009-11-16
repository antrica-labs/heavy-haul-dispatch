using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;

namespace SingerDispatch.Panels.Jobs
{
    /// <summary>
    /// Interaction logic for PermitsControl.xaml
    /// </summary>
    public partial class PermitsControl : JobUserControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public PermitsControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            cmbLoads.ItemsSource = SelectedJob != null ? SelectedJob.Loads : null;
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);

            dgPermits.ItemsSource = new ObservableCollection<Permit>((from p in Database.Permits where p.Job == SelectedJob select p).ToList());
        }

        private void NewPermit_Click(object sender, RoutedEventArgs e)
        {
            var permit = new Permit() { JobID = SelectedJob.ID };

            SelectedJob.Permits.Add(permit);
            ((ObservableCollection<Permit>)dgPermits.ItemsSource).Add(permit);
            dgPermits.SelectedItem = permit;

            cmbLoads.Focus();
        }

        private void RemovePermit_Click(object sender, RoutedEventArgs e)
        {
            var permit = (Permit)dgPermits.SelectedItem;

            if (permit == null)
            {
                return;
            }

            SelectedJob.Permits.Remove(permit);
            ((ObservableCollection<Permit>)dgPermits.ItemsSource).Remove(permit);

            dgPermits.SelectedItem = null;
        }
    }
}
