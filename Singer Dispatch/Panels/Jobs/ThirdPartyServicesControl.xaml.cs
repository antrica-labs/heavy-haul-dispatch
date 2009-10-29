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

            cmbServiceTypes.ItemsSource = (from t in Database.ThirdPartyServiceTypes select t).ToList();
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            cmbLoads.ItemsSource = SelectedJob != null ? SelectedJob.Loads : null;
            cmbCompanies.ItemsSource = (from c in Database.Companies orderby c.Name select c).ToList();            
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);

            dgServices.ItemsSource = new ObservableCollection<ThirdPartyService>((from s in Database.ThirdPartyServices where s.Job == newValue select s).ToList());
        }

        private void SelectedCompany_Changed(object sender, SelectionChangedEventArgs e)
        {
            cmbContacts.ItemsSource = new List<Contact>(0);

            if (cmbCompanies.SelectedItem != null)
            {
                var company = (Company)cmbCompanies.SelectedItem;

                foreach (Address address in company.Addresses)
                {
                    var contacts = address.Contacts.ToList<Contact>();

                    ((List<Contact>)cmbContacts.ItemsSource).AddRange(contacts);
                }
            }       
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
