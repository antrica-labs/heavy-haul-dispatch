﻿using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using SingerDispatch.Windows;
using System;

namespace SingerDispatch.Panels.Loads
{
    /// <summary>
    /// Interaction logic for ThirdPartyServicesControl.xaml
    /// </summary>
    public partial class ThirdPartyServicesControl
    {
        private Boolean SelectionChangedInCode = false;
     
        public static DependencyProperty SelectedThirdPartyServiceProperty = DependencyProperty.Register("SelectedThirdPartyService", typeof(ThirdPartyService), typeof(ThirdPartyServicesControl), new PropertyMetadata(null, SelectedThirdPartyServicePropertyChanged));

        public ThirdPartyService SelectedThirdPartyService
        {
            get
            {
                return (ThirdPartyService)GetValue(SelectedThirdPartyServiceProperty);
            }
            set
            {
                SetValue(SelectedThirdPartyServiceProperty, value);
            }
        }

        public ThirdPartyServicesControl()
        {
            InitializeComponent();

            if (InDesignMode()) return;

            Database = SingerConfigs.CommonDataContext;
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode() || IsVisible == false) return;

            UpdateComboBoxes();
        }

        protected override void SelectedLoadChanged(Load newValue, Load oldValue)
        {
            base.SelectedLoadChanged(newValue, oldValue);

            dgServices.ItemsSource = (newValue == null) ? null : new ObservableCollection<ThirdPartyService>(newValue.ThirdPartyServices);
        }

        public static void SelectedThirdPartyServicePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ThirdPartyServicesControl)d;

            control.SelectionChangedInCode = true;
            control.dgServices.SelectedItem = (ThirdPartyService)e.NewValue;
        }        

        private void ServiceCompany_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateComboBoxes();
        }

        private void UpdateComboBoxes()
        {
            if (SelectedThirdPartyService != null && SelectedThirdPartyService.Company != null)
            {
                cmbServiceTypes.ItemsSource = new ObservableCollection<ServiceType>(from st in Database.Services where st.Company == SelectedThirdPartyService.Company select st.ServiceType);
                cmbContacts.ItemsSource = new ObservableCollection<Contact>(from c in Database.Contacts where c.Company == SelectedThirdPartyService.Company orderby c.FirstName, c.LastName select c);
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

        protected void AddContact_Click(object sender, RoutedEventArgs e)
        {
            var cmb = (ComboBox)((Button)sender).DataContext;
            var tpservice = (ThirdPartyService)dgServices.SelectedItem;

            if (SelectedLoad == null || tpservice == null || tpservice.Company == null) return;

            var window = new CreateContactWindow(Database, tpservice.Company, null) { Owner = Application.Current.MainWindow };
            var contact = window.CreateContact();

            if (contact == null || contact.Company == null) return;

            var list = (ObservableCollection<Contact>)cmb.ItemsSource;

            contact.Company.Contacts.Add(contact);
            list.Add(contact);

            cmb.SelectedItem = contact;
        }

        private void UpdateServices_Click(object sender, RoutedEventArgs e)
        {
            var cmb = (ComboBox)((Button)sender).DataContext;
            var tpservice = (ThirdPartyService)dgServices.SelectedItem;

            if (SelectedLoad == null || tpservice == null || tpservice.Company == null) return;

            var window = new CompanyServicesWindow(Database, tpservice.Company) { Owner = Application.Current.MainWindow };
            var services = window.UpdateServices();

            cmb.ItemsSource = new ObservableCollection<ServiceType>(from s in services select s.ServiceType);
        }

        private void dgServices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectionChangedInCode)
            {
                SelectionChangedInCode = false;
                return;
            }

            var item = (ThirdPartyService)dgServices.SelectedItem;

            SelectedThirdPartyService = null;
            SelectedThirdPartyService = item;
        }

        private void UpdateCompanies_Click(object sender, RoutedEventArgs e)
        {
            var service = (ThirdPartyService)dgServices.SelectedItem;

            if (service == null) return;

            var window = new CreateCompanyWindow(Database) { Owner = Application.Current.MainWindow };
            var company = window.CreateCompany();

            if (company == null) return;

            try
            {
                Database.SubmitChanges();
                CompanyList.Add(company);

                service.Company = company;
            }
            catch (Exception ex)
            {
                NoticeWindow.ShowError("Error while adding company to database", ex.Message);

                Database.RevertChanges();
            }
        }
    }
}
