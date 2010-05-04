using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;

namespace SingerDispatch.Panels.Jobs
{
    /// <summary>
    /// Interaction logic for PermitsControl.xaml
    /// </summary>
    public partial class PermitsControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public PermitsControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {            
            cmbLoads.ItemsSource = (SelectedJob == null) ? null : SelectedJob.Loads.ToList();
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);

            dgPermits.ItemsSource = (newValue == null) ? null : new ObservableCollection<Permit>(newValue.Permits);
        }

        private void NewPermit_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob == null) return;

            var list = (ObservableCollection<Permit>)dgPermits.ItemsSource;
            var permit = new Permit { JobID = SelectedJob.ID, StartDate = SelectedJob.StartDate, EndDate = SelectedJob.EndDate };

            SelectedJob.Permits.Add(permit);
            list.Add(permit);
            dgPermits.SelectedItem = permit;
            dgPermits.ScrollIntoView(permit);                

            cmbLoads.Focus();
        }

        private void DuplicatePermit_Click(object sender, RoutedEventArgs e)
        {
            var permit = (Permit)dgPermits.SelectedItem;

            if (permit == null)
                return;

            permit = permit.Duplicate();

            SelectedJob.Permits.Add(permit);
            ((ObservableCollection<Permit>)dgPermits.ItemsSource).Insert(0, permit);
        }

        private void RemovePermit_Click(object sender, RoutedEventArgs e)
        {
            var permit = (Permit)dgPermits.SelectedItem;

            if (permit == null) return;

            var confirmation = MessageBox.Show(SingerConstants.DefaultRemoveItemMessage, "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            SelectedJob.Permits.Remove(permit);
            ((ObservableCollection<Permit>)dgPermits.ItemsSource).Remove(permit);

            dgPermits.SelectedItem = null;
        }

        private void IssuingCompany_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var company = (Company)e.AddedItems[0];

                cmbPermitTypes.ItemsSource = from st in Database.Services where st.Company == company select st.ServiceType;
            }
            else
            {
                cmbPermitTypes.ItemsSource = null;
            }
        }
    }
}
