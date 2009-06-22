using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SingerDispatch.Panels.Companies
{
    /// <summary>
    /// Interaction logic for CommoditiesControl.xaml
    /// </summary>
    public partial class CommoditiesControl : CompanyUserControl
    {
        private SingerDispatchDataContext database;

        public CommoditiesControl()
        {
            InitializeComponent();

            database = new SingerDispatchDataContext();
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            Company company = newValue;

            if (company != null)
            {
                dgCommodities.ItemsSource = new ObservableCollection<Commodity>(
                    (from c in database.Commodities where c.CompanyID == company.ID select c).ToList()
                );
            }
            else
            {
                dgCommodities.ItemsSource = null;
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
            float tmpFloat;
            decimal tmpDecimal;

            commodity.Name = txtCommodityName.Text;
            commodity.Serial = txtCommoditySerial.Text;
            commodity.Owner = txtCommodityOwner.Text;            
            commodity.Unit = txtCommodityUnitNumber.Text;
            commodity.LastAddress = txtCommodityLastAddress.Text;
            commodity.LastLocation = txtCommodityLastLocation.Text;
            commodity.Notes = txtCommodityNotes.Text;
            commodity.WeightEstimated = chbCommodityWeightEstimated.IsChecked == true ? (byte)1 : (byte)0;
            commodity.SizeEstimated = chbCommoditySizeEstimated.IsChecked == true ? (byte)1 : (byte)0;

            decimal.TryParse(txtCommodityValue.Text, out tmpDecimal);
            commodity.Value = tmpDecimal;

            float.TryParse(txtCommodityLength.Text, out tmpFloat);
            commodity.Length = tmpFloat;

            float.TryParse(txtCommodityWidth.Text, out tmpFloat);
            commodity.Width = tmpFloat;
            
            float.TryParse(txtCommodityHeight.Text, out tmpFloat);
            commodity.Height = tmpFloat;

            database.Commodities.InsertOnSubmit(commodity);
            ((ObservableCollection<Commodity>)dgCommodities.ItemsSource).Add(commodity);

            dgCommodities.SelectedItem = commodity;
        }

        private void DataGridCommit(object sender, Microsoft.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {
            database.SubmitChanges();
        }
    }
}
