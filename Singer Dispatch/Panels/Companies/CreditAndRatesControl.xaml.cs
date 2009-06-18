using System.Windows.Controls;
using System.Windows;
using System.Linq;
using System.Collections.Generic;

namespace SingerDispatch.Panels.Companies
{
    /// <summary>
    /// Interaction logic for CreditAndRatesControl.xaml
    /// </summary>
    public partial class CreditAndRatesControl : UserControl
    {
        public static DependencyProperty SelectedCompanyProperty = DependencyProperty.Register("SelectedCompany", typeof(Company), typeof(CreditAndRatesControl), new PropertyMetadata(null, CreditAndRatesControl.SelectedCompanyPropertyChanged));

        private SingerDispatchDataContext database;

        public CreditAndRatesControl()
        {
            InitializeComponent();

            database = new SingerDispatchDataContext();            
        }

        public Company SelectedCompany
        {
            get
            {
                return (Company)GetValue(SelectedCompanyProperty);
            }
            set
            {
                SetValue(SelectedCompanyProperty, value);
            }        
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            cmbCreditCustomerType.ItemsSource = SingerConstants.CustomerTypes;
            cmbCreditPriority.ItemsSource = (from l in database.CompanyPriorityLevels orderby l.Name     select l).ToList();
        }

        public static void SelectedCompanyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {         
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            CompanyPriorityLevel level = (from l in database.CompanyPriorityLevels where l.ID == 2 select l).Single();

            Company company = new Company();
            company.Name = "New test";
            company.PriorityLevelID = 2;// level.ID;

            database.Companies.InsertOnSubmit(company);
            database.SubmitChanges();
            
        }        
    }
}
