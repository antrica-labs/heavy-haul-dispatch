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
        SingerDispatchDataContext database;

        public ServicesControl()
        {
            InitializeComponent();

            database = SingerConstants.CommonDataContext;    
        }

        private void ControlLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            panelServiceTypes.Children.Clear();
            
            var services = from s in database.ServiceTypes select s;

            foreach (ServiceType service in services)
            {
                CheckBox cb = new CheckBox();
                cb.Content = service.Name;

                panelServiceTypes.Children.Add(cb);
            }
        }


    }
}
