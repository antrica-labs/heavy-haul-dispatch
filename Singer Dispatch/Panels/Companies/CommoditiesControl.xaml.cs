using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SingerDispatch.Panels.Companies
{
    /// <summary>
    /// Interaction logic for CommoditiesControl.xaml
    /// </summary>
    public partial class CommoditiesControl : UserControl
    {
        public static DependencyProperty SelectedCompanyProperty = DependencyProperty.Register("SelectedCompany", typeof(Company), typeof(CommoditiesControl), new PropertyMetadata(null, CommoditiesControl.SelectedCompanyPropertyChanged));

        private SingerDispatchDataContext database;

        public CommoditiesControl()
        {
            InitializeComponent();

            database = new SingerDispatchDataContext();
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            
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

        public static void SelectedCompanyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CommoditiesControl control = (CommoditiesControl)d;
            Company company = (Company)e.NewValue;

            if (company != null)
            {                
                control.dgCommodities.ItemsSource = new ObservableCollection<Commodity>(
                    (from c in control.database.Commodities where c.CompanyID == company.ID select c).ToList()
                );                
            }
            else
            {
                control.dgCommodities.ItemsSource = null;
            }
        }

        private void btnNewCommodity_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCompany == null)
            {
                MessageBox.Show("You must select a company from the company list before you can add or edit any addresses.");
                return;
            }

            dgCommodities.SelectedItem = null;
            txtCommodityName.Focus();            
        }

        private void btnDeleteCommodity_Click(object sender, RoutedEventArgs e)
        {
            Commodity selected = (Commodity)dgCommodities.SelectedItem;

            if (selected == null)
            {                
                return;
            }

            MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to remove this commodity and all of it's history?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation == MessageBoxResult.Yes)
            {
                database.Commodities.DeleteOnSubmit(selected);
                ((ObservableCollection<Commodity>)dgCommodities.ItemsSource).Remove(selected);

                database.SubmitChanges();
            }
        }

        private void btnSaveCommodity_Click(object sender, RoutedEventArgs e)
        {
            Company company = SelectedCompany;
            Commodity commodity = (Commodity)dgCommodities.SelectedItem;

            if (company == null)
            {
                MessageBox.Show("You must select a company from the company list before you can add or edit any addresses.");
                return;
            }

            if (commodity == null)
            {
                CreateNewCommodity(company);
            }

            database.SubmitChanges();           
        }

        private void CreateNewCommodity(Company company)
        {
            Commodity commodity = new Commodity() { CompanyID = company.ID };

            commodity.Name = txtCommodityName.Text;
            commodity.Serial = txtCommoditySerial.Text;
            commodity.Owner = txtCommodityOwner.Text;
            commodity.Value = decimal.Parse(txtCommodityValue.Text);
            commodity.Unit = txtCommodityUnitNumber.Text;
            commodity.LastAddress = txtCommodityLastAddress.Text;
            commodity.LastLocation = txtCommodityLastLocation.Text;
            commodity.Notes = txtCommodityNotes.Text;
            commodity.Length = null;// float.Parse(txtCommodityLength.Text);
            commodity.Width = null;// float.Parse(txtCommodityWidth.Text);
            commodity.Height = null;// float.Parse(txtCommodityHeight.Text);

            database.Commodities.InsertOnSubmit(commodity);
            ((ObservableCollection<Commodity>)dgCommodities.ItemsSource).Add(commodity);

            dgCommodities.SelectedItem = commodity;
        }
    }
}
