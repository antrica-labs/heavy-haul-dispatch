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
        private CommandBinding SaveCommand { get; set; }

        public SingerDispatchDataContext Database { get; set; }

        public ServicesControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
            
            SaveCommand = new CommandBinding(CustomCommands.GenericSaveCommand);
            CommandBindings.Add(SaveCommand);

            TheList.ItemsSource = new ObservableCollection<CheckBox>();
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            SaveCommand.Executed += CommitChanges_Executed;

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

        private void CommitChanges_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CommitChangesButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, CommitChangesButton));
        }

        private void UpdateServices_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Database.SubmitChanges();

                ((ButtonBase)sender).Focus();
            }
            catch (System.Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }
    }
}
