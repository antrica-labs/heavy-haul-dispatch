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
using System.Windows.Shapes;

namespace SingerDispatch.Windows
{
    /// <summary>
    /// Interaction logic for GeneralSearchWindow.xaml
    /// </summary>
    public partial class GeneralSearchWindow : Window
    {
        private int TAB_JOBS = 0;
        private int TAB_QUOTE = 1;
        
        private object SelectedEntity;

        public GeneralSearchWindow()
        {
            InitializeComponent();

            SelectedEntity = null;
        }

        public object FindSomething()
        {
            ShowDialog();

            return SelectedEntity;
        }

        private void RunSearch_Click(object sender, RoutedEventArgs e)
        {
            var terms = txtSearchTerm.Text;
            var company = txtCompany.Text;
            var start = startDate.SelectedDate;
            var end = endDate.SelectedDate;

            FindJobs(terms, company, start, end);
        }        

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                SelectedEntity = null;
                Close();
            }
        }

        private void SearchItem_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var grid = (DataGrid)sender;

            SelectedEntity = grid.SelectedItem;

            Close();
        }

        private void FindJobs(string term, string company, DateTime? start, DateTime? end)
        {
            var database = new SingerDispatchDataContext();

            var jobs = from j in database.Jobs select j;

            if (!string.IsNullOrWhiteSpace(term))
            {
                jobs = from j in jobs where j.Name.ToUpper().Contains(term.ToUpper()) select j;
            }

            if (!string.IsNullOrWhiteSpace(company))
            {
                jobs = from j in jobs where j.Company.Name.ToUpper().Contains(company.ToUpper()) || (j.CareOfCompany != null && j.CareOfCompany.Name.ToUpper().Contains(company.ToUpper())) select j;
            }

            dgJobs.ItemsSource = jobs;
        }

        private void FindLoads(string term, string company, DateTime start, DateTime end)
        {

        }

        private void FindDispatches(string term, string company, DateTime start, DateTime end)
        {

        }

        private void FindQuotes(string term, string company, DateTime start, DateTime end)
        {

        }

        private void FindInvoices(string term, string company, DateTime start, DateTime end)
        {

        }

        private void FindThirdPartyServices(string term, string company, DateTime start, DateTime end)
        {

        }

        private void FindPermits(string term, string company, DateTime start, DateTime end)
        {

        }
    }
}
