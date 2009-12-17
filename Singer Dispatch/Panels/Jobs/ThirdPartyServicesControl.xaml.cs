using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace SingerDispatch.Panels.Jobs
{
    /// <summary>
    /// Interaction logic for ThirdPartyServicesControl.xaml
    /// </summary>
    public partial class ThirdPartyServicesControl : JobUserControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public ThirdPartyServicesControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;

            cmbServiceTypes.ItemsSource = from t in Database.ThirdPartyServiceTypes select t;
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            cmbLoads.ItemsSource = (SelectedJob == null) ? null : SelectedJob.Loads.ToList();
            cmbCompanies.ItemsSource = (SelectedJob == null) ? null : from c in Database.Companies orderby c.Name select c;
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);

            dgServices.ItemsSource = (newValue == null) ? null : new ObservableCollection<ThirdPartyService>(newValue.ThirdPartyServices);
        }

        private void SelectedCompany_Changed(object sender, SelectionChangedEventArgs e)
        {
                   
        }

        private void NewService_Click(object sender, RoutedEventArgs e)
        {
            var service = new ThirdPartyService() { JobID = SelectedJob.ID };

            SelectedJob.ThirdPartyServices.Add(service);
            ((ObservableCollection<ThirdPartyService>)dgServices.ItemsSource).Add(service);
            dgServices.SelectedItem = service;

            cmbLoads.Focus();
        }

        private void RemoveService_Click(object sender, RoutedEventArgs e)
        {
            var service = (ThirdPartyService)dgServices.SelectedItem;

            if (service == null)
            {
                return;
            }

            ((ObservableCollection<ThirdPartyService>)dgServices.ItemsSource).Remove(service);
            SelectedJob.ThirdPartyServices.Remove(service);

            dgServices.SelectedItem = null;
        }
        
    }
}
