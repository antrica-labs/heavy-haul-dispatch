using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace SingerDispatch.Panels.Jobs
{
    /// <summary>
    /// Interaction logic for ThirdPartyServicesControl.xaml
    /// </summary>
    public partial class ThirdPartyServicesControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public ThirdPartyServicesControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            cmbLoads.ItemsSource = (SelectedJob == null) ? null : SelectedJob.Loads.ToList();
            cmbCompanies.ItemsSource = (SelectedJob == null) ? null : from c in Database.Companies  where c.IsVisible == true orderby c.Name select c;
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);

            dgServices.ItemsSource = (newValue == null) ? null : new ObservableCollection<ThirdPartyService>(newValue.ThirdPartyServices);
        }

        private void ServiceCompany_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var company = (Company)e.AddedItems[0];

                cmbServiceTypes.ItemsSource = from st in Database.Services where st.Company == company select st.ServiceType;

                var addressQuery = from a in Database.Addresses where a.Company == company select a;                
                cmbContacts.ItemsSource = from c in Database.Contacts where addressQuery.Contains(c.Address) select c;
            }
            else
            {
                cmbServiceTypes.ItemsSource = null;
                cmbContacts.ItemsSource = null;
            }
        }

        private void NewService_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob == null) return;

            var list = (ObservableCollection<ThirdPartyService>)dgServices.ItemsSource;
            var service = new ThirdPartyService { JobID = SelectedJob.ID, ServiceDate = SelectedJob.StartDate };

            SelectedJob.ThirdPartyServices.Add(service);
            list.Add(service);
            dgServices.SelectedItem = service;
            dgServices.ScrollIntoView(service);

            cmbLoads.Focus();
        }

        private void DuplicateService_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<ThirdPartyService>)dgServices.ItemsSource;
            var service = (ThirdPartyService)dgServices.SelectedItem;

            if (service == null)
                return;

            service = service.Duplicate();

            SelectedJob.ThirdPartyServices.Add(service);
            list.Add(service);
            dgServices.SelectedItem = service;
            dgServices.ScrollIntoView(service);
        }

        private void RemoveService_Click(object sender, RoutedEventArgs e)
        {
            var service = (ThirdPartyService)dgServices.SelectedItem;

            if (service == null) return;

            var confirmation = MessageBox.Show(SingerConstants.DefaultRemoveItemMessage, "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            ((ObservableCollection<ThirdPartyService>)dgServices.ItemsSource).Remove(service);
            SelectedJob.ThirdPartyServices.Remove(service);

            dgServices.SelectedItem = null;
        }
        
    }
}
