using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace SingerDispatch.Panels.Companies
{
    /// <summary>
    /// Interaction logic for ServicesControl.xaml
    /// </summary>
    public partial class ServicesControl : CompanyUserControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public ServicesControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;            
            TheList.ItemsSource = new ObservableCollection<CheckBox>();
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<CheckBox>)TheList.ItemsSource;
            var types = from t in Database.ServiceTypes select t;
            var selected = from s in Database.Services where s.Company == SelectedCompany select s.ServiceType;

            list.Clear();

            foreach (var type in types)
            {
                var cb = new CheckBox { Content = type.Name, DataContext = type, IsChecked = selected.Contains(type) };

                list.Add(cb);
            }
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);           
        }

        private void UpdateServices_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<CheckBox>)TheList.ItemsSource;

            if (SelectedCompany == null) return;
                        
            Database.Services.DeleteAllOnSubmit(SelectedCompany.Services);             

            foreach (CheckBox cb in list)
            {
                if (cb.IsChecked != true)
                {
                    continue;
                }

                var service = new Service { ServiceTypeID = ((ServiceType)cb.DataContext).ID };
                          
                SelectedCompany.Services.Add(service);
            }

            try
            {
                Database.SubmitChanges();
            }
            catch (System.Exception ex)
            {
                SingerDispatch.Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

    }
}
