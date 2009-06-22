using System.Windows.Controls;
using System.Windows;
using System.Linq;
using System.Collections.Generic;

namespace SingerDispatch.Panels.Companies
{
    /// <summary>
    /// Interaction logic for CreditAndRatesControl.xaml
    /// </summary>
    public partial class CreditAndRatesControl : CompanyUserControl
    {        
        private SingerDispatchDataContext database;

        public CreditAndRatesControl()
        {
            InitializeComponent();

            database = new SingerDispatchDataContext();            
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            cmbCreditCustomerType.ItemsSource = SingerConstants.CustomerTypes;
            cmbCreditPriority.ItemsSource = (from l in database.CompanyPriorityLevels orderby l.Name     select l).ToList();
        }

        private void DataGridCommit(object sender, Microsoft.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {
            database.SubmitChanges();
        }

        private void btnSaveAdminDetails_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCompany != null)
            {                
                Company company = (from c in database.Companies where c.ID == SelectedCompany.ID select c).Single();                

                company.Type = SelectedCompany.Type;
                company.AvailableCredit = SelectedCompany.AvailableCredit;             
                company.AccPacVendorCode = SelectedCompany.AccPacVendorCode;
                company.PriorityLevelID = SelectedCompany.PriorityLevelID;
                company.Notes = SelectedCompany.Notes;
                company.EquifaxComplete = SelectedCompany.EquifaxComplete;                
                
                database.SubmitChanges();
            }
        }
     
    }
}
