using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SingerDispatch.Panels.Companies
{
    /// <summary>
    /// Interaction logic for ServicesControl.xaml
    /// </summary>
    public partial class ServicesControl : CompanyUserControl
    {
        public SingerDispatchDataContext Database { get; set; }
        private List<CheckBox> ServiceTypes { get; set; }


        public ServicesControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
            ServiceTypes = new List<CheckBox>();

            PopulateServices();            
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);
            
            if (newValue != null)
            {
                var selected = from s in Database.Services where s.CompanyID == newValue.ID select s.ServiceType;

                foreach (CheckBox cb in ServiceTypes)
                {
                    var type = (ServiceType)cb.DataContext;

                    cb.IsChecked = selected.Contains(type);
                }
            }
        }

        private void PopulateServices()
        {
            var types = from t in Database.ServiceTypes select t;

            foreach (ServiceType type in types)
            {
                var cb = new CheckBox { Content = type.Name, DataContext = type };

                ServiceTypes.Add(cb);
                panelServiceTypes.Children.Add(cb);
            }
        }

        private void UpdateServices_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCompany == null)
            {
                return;
            }

            Database.Services.DeleteAllOnSubmit(SelectedCompany.Services);             

            foreach (CheckBox cb in ServiceTypes)
            {
                if (cb.IsChecked != true)
                {
                    continue;
                }

                var service = new Service { ServiceTypeID = ((ServiceType)cb.DataContext).ID };
                          
                SelectedCompany.Services.Add(service);
            }

            Database.SubmitChanges();
        }

    }
}
