using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace SingerDispatch.Panels.Loads
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

            if (InDesignMode()) return;

            Database = SingerConfigs.CommonDataContext;
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {  

            
        }

        protected override void SelectedLoadChanged(Load newValue, Load oldValue)
        {
            base.SelectedLoadChanged(newValue, oldValue);

            dgServices.ItemsSource = (newValue == null) ? null : new ObservableCollection<ThirdPartyService>(newValue.ThirdPartyServices);
        }

        private void ServiceCompany_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var company = (Company)e.AddedItems[0];

                cmbServiceTypes.ItemsSource = from st in Database.Services where st.Company == company select st.ServiceType;                                
                cmbContacts.ItemsSource = from c in Database.Contacts where c.Company == company orderby c.FirstName, c.LastName select c;
            }
            else
            {
                cmbServiceTypes.ItemsSource = null;
                cmbContacts.ItemsSource = null;
            }
        }

        private void NewService_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedLoad == null) return;

            var list = (ObservableCollection<ThirdPartyService>)dgServices.ItemsSource;
            var service = new ThirdPartyService { LoadID = SelectedLoad.ID };

            SelectedLoad.ThirdPartyServices.Add(service);
            list.Add(service);
            dgServices.SelectedItem = service;
            dgServices.ScrollIntoView(service);

            cmbCompanies.Focus();
        }

        private void DuplicateService_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<ThirdPartyService>)dgServices.ItemsSource;
            var service = (ThirdPartyService)dgServices.SelectedItem;

            if (service == null)
                return;

            service = service.Duplicate();

            SelectedLoad.ThirdPartyServices.Add(service);
            list.Add(service);
            dgServices.SelectedItem = service;
            dgServices.ScrollIntoView(service);
        }

        private void RemoveService_Click(object sender, RoutedEventArgs e)
        {
            var service = (ThirdPartyService)dgServices.SelectedItem;

            if (service == null) return;

            var confirmation = MessageBox.Show(SingerConfigs.DefaultRemoveItemMessage, "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            ((ObservableCollection<ThirdPartyService>)dgServices.ItemsSource).Remove(service);
            SelectedLoad.ThirdPartyServices.Remove(service);

            dgServices.SelectedItem = null;
        }
        
    }
}
