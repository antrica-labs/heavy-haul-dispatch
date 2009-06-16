using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using SingerDispatch.Controls;

namespace SingerDispatch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SingerDispatchDataContext database;
        ObservableCollection<Company> companies;

        public MainWindow()
        {   
            InitializeComponent();

            this.database = new SingerDispatchDataContext();            
            this.companies = new ObservableCollection<Company>(
                (from c in database.Companies orderby c.Name select c).ToList()
            );
            
            BuildMainNavigation();            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cmbCompanies.ItemsSource = companies;
            cmbOperators.ItemsSource = companies;
        }

        private void BuildMainNavigation()
        {
            //CompaniesPanel panel = (CompaniesPanel) panelMainContent.Child;

            //addLinksToExpander(linksCompanies, panel.tabs.Items);

            //addLinksToExpander(linksCompanies, tabCompanies.Items);
            //addLinksToExpander(linksQuotes, tabQuotes.Items);
        }       

        private void addLinksToExpander(StackPanel panel, ItemCollection tabs)
        {
            foreach (TabItem tab in tabs)
            {
                TabIndexHyperlink link = new TabIndexHyperlink(tab);
                link.Click += new RoutedEventHandler(SwitchTabs);

                Label label = new Label();
                label.Content = link;

                panel.Children.Add(label);
            }
        }

        private void SwitchTabs(object sender, RoutedEventArgs e)
        {
            TabIndexHyperlink link = (TabIndexHyperlink)e.Source;

            link.Tab.IsSelected = true;
        }        

        private void menuCreateCompany_Click(object sender, RoutedEventArgs e)
        {
            CreateCompanyWindow window = new CreateCompanyWindow(database, companies);
            window.Owner = this;
            Company newCompany = window.CreateCompany();

            if (newCompany != null)
            {
                cmbCompanies.SelectedItem = newCompany;
            }
        }

        private void cmbCompanies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbOperators.SelectedItem = (Company)cmbCompanies.SelectedItem;
        }

        private void cmbOperators_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbCompanies.SelectedItem = (Company)cmbOperators.SelectedItem;
        }

        private void Terminate(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
