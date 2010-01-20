using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using SingerDispatch.Controls;

namespace SingerDispatch.Panels.Companies
{
    /// <summary>
    /// Interaction logic for CommoditiesControl.xaml
    /// </summary>
    public partial class CommoditiesControl : CompanyUserControl
    {
        private CommandBinding SaveCommand { get; set; }

        public SingerDispatchDataContext Database { get; set; }

        public CommoditiesControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
            SaveCommand = new CommandBinding(CustomCommands.GenericSaveCommand);
            CommandBindings.Add(SaveCommand);
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            SaveCommand.Executed += new ExecutedRoutedEventHandler(CommitChanges_Executed);
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);
                        
            dgCommodities.ItemsSource = (newValue == null) ? null : new ObservableCollection<Commodity>(from c in Database.Commodities where c.CompanyID == newValue.ID select c);
        }

        private void NewCommodity_Click(object sender, RoutedEventArgs e)
        {
            var commodity = new Commodity();

            SelectedCompany.Commodities.Add(commodity);
            ((ObservableCollection<Commodity>)dgCommodities.ItemsSource).Insert(0, commodity);
            dgCommodities.SelectedItem = commodity;
            dgCommodities.ScrollIntoView(commodity);

            txtCommodityName.Focus();            
        }

        private void DeleteCommodity_Click(object sender, RoutedEventArgs e)
        {
            var selected = (Commodity)dgCommodities.SelectedItem;

            if (selected == null)
            {                
                return;
            }

            MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to remove this commodity and all of it's history?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation == MessageBoxResult.Yes)
            {                
                SelectedCompany.Commodities.Remove(selected);
                ((ObservableCollection<Commodity>)dgCommodities.ItemsSource).Remove(selected);

                try
                {
                    Database.SubmitChanges();
                }
                catch (System.Exception ex)
                {
                    SingerDispatch.Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
                }
            }
        }

        private void CreateNewCommodity(Company company)
        {
            var commodity = new Commodity();
            
            SelectedCompany.Commodities.Add(commodity);            
            ((ObservableCollection<Commodity>)dgCommodities.ItemsSource).Add(commodity);

            dgCommodities.SelectedItem = commodity;
        }

        private void CommitChanges_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CommitChangesButton.Focus();
            CommitChangesButton.RaiseEvent(new System.Windows.RoutedEventArgs(System.Windows.Controls.Button.ClickEvent, CommitChangesButton));
        }

        private void SaveCommodity_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Database.SubmitChanges();
            }
            catch (System.Exception ex)
            {
                SingerDispatch.Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }        

       
    }
}
