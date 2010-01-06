using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Microsoft.Windows.Controls;
using Microsoft.Windows.Controls.Primitives;
using System.Windows.Media;
using SingerDispatch.Controls;

namespace SingerDispatch.Panels.Invoicing
{
    /// <summary>
    /// Interaction logic for JobInvoicingControl.xaml
    /// </summary>
    public partial class JobInvoicingControl : JobUserControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public JobInvoicingControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            dgJobs.ItemsSource = (SelectedCompany == null) ? null : new ObservableCollection<Job>(from j in Database.Jobs where j.Company == SelectedCompany orderby j.Number descending select j);
            dgCustomerDetails.ItemsSource = (SelectedCompany == null) ? null : new ObservableCollection<CustomerNumber>(SelectedCompany.CustomerNumbers);
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            dgJobs.ItemsSource = (SelectedCompany == null) ? null : new ObservableCollection<Job>(from j in Database.Jobs where j.Company == SelectedCompany orderby j.Number descending select j);
            dgCustomerDetails.ItemsSource = (SelectedCompany == null) ? null : new ObservableCollection<CustomerNumber>(SelectedCompany.CustomerNumbers);
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);

            UpdateContactList();
        }

        private void NewCustomerDetail_Click(object sender, RoutedEventArgs e)
        {
            var number = new CustomerNumber();
            var list = (ObservableCollection<CustomerNumber>)dgCustomerDetails.ItemsSource;

            list.Insert(0, number);            
            dgCustomerDetails.SelectedItem = number;

            DataGridHelper.GetCell(dgCustomerDetails, dgCustomerDetails.SelectedIndex, 0).Focus();
        }

        private void RemoveCustomerDetail_Click(object sender, RoutedEventArgs e)
        {
            var number = (CustomerNumber)dgCustomerDetails.SelectedItem;

            if (number == null) return;
            
            var list = (ObservableCollection<CustomerNumber>)dgCustomerDetails.ItemsSource;
            list.Remove(number);
        }

        private void UpdateContactList()
        {
            List<Contact> contacts;

            if (SelectedJob == null)
            {
                contacts = new List<Contact>();
            }
            else if (SelectedJob.CareOfCompanyID != null)
            {
                contacts = (from c in Database.Contacts where c.Address.CompanyID == SelectedCompany.ID || c.Address.CompanyID == SelectedJob.CareOfCompanyID select c).ToList();
            }
            else
            {
                contacts = (from c in Database.Contacts where c.Address.CompanyID == SelectedCompany.ID select c).ToList();
            }

            dgJobContacts.ItemsSource = contacts;
        }

        private void dgCustomerDetails_RowEditEnding(object sender, Microsoft.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {
            
        }

        public DataGridCell GetCell(DataGrid grid, int row, int column)
        {
            DataGridRow rowContainer = GetRow(grid, row);

            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);

                // try to get the cell but it may possibly be virtualized
                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                if (cell == null)
                {
                    // now try to bring into view and retreive the cell
                    grid.ScrollIntoView(rowContainer, grid.Columns[column]);
                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                }
                return cell;
            }
            return null;
        }

        public DataGridRow GetRow(DataGrid grid, int index)
        {
            DataGridRow row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);

            if (row == null)
            {
                // may be virtualized, bring into view and try again
                grid.UpdateLayout();
                grid.ScrollIntoView(grid.Items[index]);
                row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            }

            return row;
        }

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
    }
}
