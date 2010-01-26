using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;

namespace SingerDispatch.Panels.Jobs
{
    /// <summary>
    /// Interaction logic for WireLiftControl.xaml
    /// </summary>
    public partial class WireLiftControl : JobUserControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public WireLiftControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);

            dgWireLifts.ItemsSource = (newValue == null) ? null : new ObservableCollection<WireLift>(newValue.WireLifts);
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            cmbCompanies.ItemsSource = (from c in Database.Companies select c).ToList();
        }

        private void NewWireLift_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<WireLift>)dgWireLifts.ItemsSource;
            var item = new WireLift();
                        
            SelectedJob.WireLifts.Add(item);
            list.Add(item);
            dgWireLifts.SelectedItem = item;
            dgWireLifts.ScrollIntoView(item);

            cmbCompanies.Focus();
        }

        private void RemoveWireLift_Click(object sender, RoutedEventArgs e)
        {         
            var list = (ObservableCollection<WireLift>)dgWireLifts.ItemsSource;
            var item = (WireLift)dgWireLifts.SelectedItem;

            if (item == null)
                return;

            list.Remove(item);
            SelectedJob.WireLifts.Remove(item);
        }

        // Data binding for the AvalonControlLibrary DateTime picker doesn't work, so we need this event to handle date changes.
        private void ThePicker_SelectedDateTimeChanged(object sender, AC.AvalonControlsLibrary.Controls.DateTimeSelectedChangedRoutedEventArgs e)
        {
            var item = (WireLift)dgWireLifts.SelectedItem;

            if (item == null) return;

            item.LiftDateTime = e.NewDate;
        }

        private void cmbCompanies_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var company = (Company)cmbCompanies.SelectedItem;

            if (company != null)
            {
                var addressQuery = from a in Database.Addresses where a.Company == company select a;
                cmbContacts.ItemsSource = (from c in Database.Contacts where addressQuery.Contains(c.Address) select c).ToList();
            }
            else
                cmbContacts.ItemsSource = null;
        }

        private void cmbContacts_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var contact = (Contact)cmbContacts.SelectedItem;

            if (contact == null) return;

            txtContactPhone.Text = contact.PrimaryPhone;
        }
    }
}
