using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using SingerDispatch.Controls;

namespace SingerDispatch.Panels.Companies
{
    /// <summary>
    /// Interaction logic for ServicesControl.xaml
    /// </summary>
    public partial class ServicesControl
    {
        public ServicesControl()
        {
            InitializeComponent();
            TheList.ItemsSource = new ObservableCollection<CheckBox>();

            if (InDesignMode()) return;

            Database = SingerConfigs.CommonDataContext;
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode() || IsVisible == false) return;

            RefreshServiceList();
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            RefreshServiceList();
        }

        private void RefreshServiceList()
        {
            var list = (ObservableCollection<CheckBox>)TheList.ItemsSource;

            if (SelectedCompany == null) return;

            var types = from t in Database.ServiceTypes select t;
            var selected = from s in SelectedCompany.Services select s.ServiceType;

            list.Clear();

            foreach (var type in types)
            {
                var cb = new CheckBox { Content = type.Name, DataContext = type, IsChecked = selected.Contains(type) };

                cb.Checked += CheckBox_Checked;
                cb.Unchecked += CheckBox_Unchecked;

                list.Add(cb);
            }
        }


        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox)sender;
            var serviceType = (ServiceType)cb.DataContext;

            if (SelectedCompany == null || serviceType == null) return;

            var service = new Service { Company = SelectedCompany, ServiceType = serviceType };

            SelectedCompany.Services.Add(service);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox)sender;
            var serviceType = (ServiceType)cb.DataContext;

            if (SelectedCompany == null || serviceType == null) return;

            var list = (from s in SelectedCompany.Services where s.ServiceType == serviceType select s).ToList();

            foreach (var item in list)
            {
                SelectedCompany.Services.Remove(item);
            }
        }
    }
}
