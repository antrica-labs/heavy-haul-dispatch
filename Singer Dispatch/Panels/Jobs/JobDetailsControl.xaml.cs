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
using SingerDispatch.Database;
using SingerDispatch.Windows;
using SingerDispatch.Controls;

namespace SingerDispatch.Panels.Jobs
{
    /// <summary>
    /// Interaction logic for JobDetailsControl.xaml
    /// </summary>
    public partial class JobDetailsControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public JobDetailsControl()
        {
            InitializeComponent();
        
            if (InDesignMode()) return;
            // Work below can only be done when the real app is running. It fails during design time.

            Database = SingerConfigs.CommonDataContext;            
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode() || IsVisible == false) return;

            //cmbStatuses.ItemsSource = from s in Database.Statuses orderby s.Name select s;

            UpdateContacts();
            UpdateReferenceNumbers();
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);

            UpdateContacts();
            UpdateReferenceNumbers();
        }

        private void UpdateContacts()
        {
            if (SelectedJob != null)
            {
                var contact = SelectedJob.Contact;
                                
                cmbContacts.ItemsSource = new ObservableCollection<Contact>(from c in Database.Contacts where c.Company != null && c.Company == SelectedJob.Company || c.Company == SelectedJob.CareOfCompany orderby c.FirstName, c.LastName select c);

                cmbContacts.SelectedItem = contact;
            }
            else
            {
                cmbContacts.ItemsSource = null;
            }
        }

        private void UpdateReferenceNumbers()
        {            
            dgReferenceNumbers.ItemsSource = (SelectedJob == null) ? null : new ObservableCollection<JobReferenceNumber>(SelectedJob.ReferenceNumbers);
        }

        private void cmbCareOfCompanies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateContacts();
        }
        
        private void AddReferenceNumber_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob == null) return;

            var reference = new JobReferenceNumber();

            SelectedJob.ReferenceNumbers.Add(reference);
            ((ObservableCollection<JobReferenceNumber>)dgReferenceNumbers.ItemsSource).Add(reference);

            DataGridHelper.EditFirstColumn(dgReferenceNumbers, reference);
        }

        private void RemoveReferenceNumber_Click(object sender, RoutedEventArgs e)
        {
            var selected = (JobReferenceNumber)dgReferenceNumbers.SelectedItem;
            var list = (ObservableCollection<JobReferenceNumber>)dgReferenceNumbers.ItemsSource;
            if (SelectedJob == null || selected == null) return;

            var index = dgReferenceNumbers.SelectedIndex;

            SelectedJob.ReferenceNumbers.Remove(selected);
            list.Remove(selected);

            if (index == list.Count)
                index = index - 1;

            dgReferenceNumbers.SelectedIndex = index;
        }
        
        private void AddCompany_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob == null) return;

            var window = new CreateCompanyWindow(Database) { Owner = Application.Current.MainWindow };
            var company = window.CreateCompany();

            if (company == null) return;

            try
            {
                Database.SubmitChanges();
                CompanyList.Add(company);
                                
                SelectedJob.CareOfCompany = company;

                UpdateContacts();
            }
            catch (Exception ex)
            {
                NoticeWindow.ShowError("Error while adding company to database", ex.Message);
            }
        }

        protected void AddContact_Click(object sender, RoutedEventArgs e)
        {
            var cmb = (ComboBox)((Button)sender).DataContext;

            if (SelectedJob == null) return;

            var window = new CreateContactWindow(Database, SelectedCompany, SelectedJob.CareOfCompany) { Owner = Application.Current.MainWindow };
            var contact = window.CreateContact();

            if (contact == null || contact.Company == null) return;

            var list = (ObservableCollection<Contact>)cmb.ItemsSource;

            contact.Company.Contacts.Add(contact);
            list.Add(contact);

            cmb.SelectedItem = contact;
        }
    }
}
