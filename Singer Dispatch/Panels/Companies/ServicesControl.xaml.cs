using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Windows.Controls;

namespace SingerDispatch.Panels.Companies
{
    /// <summary>
    /// Interaction logic for ServicesControl.xaml
    /// </summary>
    public partial class ServicesControl : CompanyUserControl
    {
        private SingerDispatchDataContext database;
        private List<CheckBox> serviceTypes;


        public ServicesControl()
        {
            InitializeComponent();

            database = SingerConstants.CommonDataContext;
            serviceTypes = new List<CheckBox>();

            PopulateServices();            
        }

        private void ControlLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            if (newValue != null)
            {
                var selected = from s in database.Services where s.CompanyID == newValue.ID select s.ServiceType;

                foreach (CheckBox cb in serviceTypes)
                {
                    ServiceType type = (ServiceType)cb.DataContext;

                    cb.IsChecked = selected.Contains(type);
                }
            }
        }

        private void PopulateServices()
        {
            var types = (from t in database.ServiceTypes select t).ToList();

            foreach (ServiceType type in types)
            {
                CheckBox cb = new CheckBox() { Content = type.Name };                
                cb.DataContext = type;

                serviceTypes.Add(cb);
                panelServiceTypes.Children.Add(cb);
            }
        }

        private void btnUpdateServices_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCompany != null)
            {
                database.Services.DeleteAllOnSubmit(SelectedCompany.Services);             

                foreach (CheckBox cb in serviceTypes)
                {
                    if (cb.IsChecked == true)
                    {
                        Service service = new Service() { ServiceTypeID = ((ServiceType)cb.DataContext).ID };
                          
                        SelectedCompany.Services.Add(service);
                    }
                }

                database.SubmitChanges();
            }
        }

    }
}
