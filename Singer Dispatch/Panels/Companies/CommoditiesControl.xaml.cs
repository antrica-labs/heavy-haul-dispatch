using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using SingerDispatch.Controls;

namespace SingerDispatch.Panels.Companies
{
    /// <summary>
    /// Interaction logic for CommoditiesControl.xaml
    /// </summary>
    public partial class CommoditiesControl
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
            SaveCommand.Executed += CommitChanges_Executed;
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
                    Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
                }
            }
        }

        private void CommitChanges_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CommitChangesButton.Focus();
            CommitChangesButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, CommitChangesButton));
        }

        private void SaveCommodity_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Database.SubmitChanges();
            }
            catch (System.Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }        

       
    }
}
