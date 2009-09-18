using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using SingerDispatch.Controls;
using SingerDispatch.Panels.Quotes;
using SingerDispatch.Panels.Companies;
using SingerDispatch.Panels.Jobs;
using SingerDispatch.Panels.Pricing;
using SingerDispatch.Database;

namespace SingerDispatch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private ObservableCollection<Company> Companies { get; set; }
        private SingerDispatchDataContext Database { get; set; }

        public MainWindow()
        {   
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
            Companies = new ObservableCollection<Company>();

            if (!Database.DatabaseExists())
            {
                var builder = new DatabaseBuilder(SingerConstants.CommonDataContext);

                builder.CreateNewDatabase();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Companies = new ObservableCollection<Company>(
                (from c in Database.Companies orderby c.Name select c).ToList()
            );

            cmbCompanies.ItemsSource = Companies;
            cmbOperators.ItemsSource = Companies;

            expanderCompanies.IsExpanded = true; 
        }                

        private void menuCreateCompany_Click(object sender, RoutedEventArgs e)
        {
            var window = new CreateCompanyWindow(Database, Companies) { Owner = this };
            Company newCompany = window.CreateCompany();

            if (newCompany != null)
            {
                cmbCompanies.SelectedItem = newCompany;
            }
        }

        private void cmbCompanies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbOperators.SelectedItem = cmbCompanies.SelectedItem;
        }

        private void cmbOperators_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbCompanies.SelectedItem = cmbOperators.SelectedItem;
        }

        private void Terminate(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private static void AddLinksToExpander(Panel panel, ItemCollection tabs)
        {
            panel.Children.Clear();

            foreach (TabItem tab in tabs)
            {
                var link = new TabIndexHyperlink(tab);
                link.Click += SwitchTabs;

                var label = new Label { Content = link };

                panel.Children.Add(label);
            }
        }

        private static void SwitchTabs(object sender, RoutedEventArgs e)
        {
            var link = (TabIndexHyperlink)e.Source;

            link.Tab.IsSelected = true;
        }

        private void CollapseAllOtherNavigationExpanders(Expander self)
        {
            foreach (Control control in altNavigationStack.Children)
            {
                if (!(control is Expander)) continue;
                
                var expander = (Expander)control;

                if (expander != self && expander.IsExpanded)
                {
                    expander.IsExpanded = false;
                }
            }
        }

        private void ExpandCompanies(object sender, RoutedEventArgs e)
        {
            if (panelMainContent.Child is CompaniesPanel)
            {
                // This panel is already visible, so we don't need to reload it.
                return;
            }

            CollapseAllOtherNavigationExpanders((Expander)sender);

            var panel = new CompaniesPanel();

            var binding = new Binding();
            binding.ElementName = "cmbCompanies";
            binding.Path = new PropertyPath(Selector.SelectedItemProperty);

            panel.SetBinding(CompanyUserControl.SelectedCompanyProperty, binding);

            panelMainContent.Child = panel;

            AddLinksToExpander(linksCompanies, panel.tabs.Items);            
        }

        private void ExpandQuotes(object sender, RoutedEventArgs e)
        {
            if (panelMainContent.Child is QuotesPanel)
            {
                // This panel is already visible, so we don't need to reload it.
                return;
            }

            CollapseAllOtherNavigationExpanders((Expander)sender);

            var panel = new QuotesPanel();

            var binding = new Binding();
            binding.ElementName = "cmbCompanies";
            binding.Path = new PropertyPath(Selector.SelectedItemProperty);

            panel.SetBinding(QuotesPanel.SelectedCompanyProperty, binding);

            panelMainContent.Child = panel;

            AddLinksToExpander(linksQuotes, panel.tabs.Items);
        }


        private void ExpandJobs(object sender, RoutedEventArgs e)
        {
            if (panelMainContent.Child is JobsPanel)
            {
                return;
            }

            CollapseAllOtherNavigationExpanders((Expander)sender);

            var panel = new JobsPanel();

            var binding = new Binding();
            binding.ElementName = "cmbCompanies";
            binding.Path = new PropertyPath(Selector.SelectedItemProperty);

            panel.SetBinding(JobsPanel.SelectedCompanyProperty, binding);

            panelMainContent.Child = panel;
            AddLinksToExpander(linksJobs, panel.tabs.Items);   
        }

        private void ExpandPricing(object sender, RoutedEventArgs e)
        {
            if (panelMainContent.Child is JobPricingPanel)
            {
                return;
            }

            CollapseAllOtherNavigationExpanders((Expander)sender);

            var panel = new JobPricingPanel();

            var binding = new Binding();
            binding.ElementName = "cmbCompanies";
            binding.Path = new PropertyPath(Selector.SelectedItemProperty);

            panel.SetBinding(JobPricingPanel.SelectedCompanyProperty, binding);

            panelMainContent.Child = panel;
            AddLinksToExpander(linksPricing, panel.tabs.Items);
        }

        private void menuOpenQuoteSample_Click(object sender, RoutedEventArgs e)
        {
            var viewer =  new DocumentViewer();

            viewer.Show();

        }
    }
}
