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
            UpdateComboBoxes();
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);

            UpdateComboBoxes();
            UpdateReferenceNumbers();
        }

        private void UpdateComboBoxes()
        {
            if (SelectedJob != null)
            {                
                cmbContacts.ItemsSource = (SelectedCompany == null) ? from c in Database.Contacts where c.Company == SelectedCompany orderby c.FirstName, c.LastName select c : from c in Database.Contacts where c.Company == SelectedCompany || c.Company == SelectedJob.CareOfCompany orderby c.FirstName, c.LastName select c;
                cmbStatuses.ItemsSource = from s in Database.Statuses orderby s.Name select s;
            }
            else
            {                
                cmbContacts.ItemsSource = null;
                cmbStatuses.ItemsSource = null;
            }
        }

        private void UpdateReferenceNumbers()
        {
            if (SelectedJob != null)
            {
                dgReferenceNumbers.ItemsSource = new ObservableCollection<JobReferenceNumber>(SelectedJob.ReferenceNumbers);
            }
            else
            {
                dgReferenceNumbers.ItemsSource = null;
            }
        }

        private void cmbCareOfCompanies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedJob == null) return;

            cmbContacts.ItemsSource = (SelectedCompany == null) ? from c in Database.Contacts where c.Company == SelectedCompany orderby c.FirstName, c.LastName select c : from c in Database.Contacts where c.Company == SelectedCompany || c.Company == SelectedJob.CareOfCompany orderby c.FirstName, c.LastName select c;
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

            if (SelectedJob == null || selected == null) return;

            SelectedJob.ReferenceNumbers.Remove(selected);
            ((ObservableCollection<JobReferenceNumber>)dgReferenceNumbers.ItemsSource).Remove(selected);
        }
    }
}
