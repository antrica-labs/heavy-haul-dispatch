using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using SingerDispatch.Controls;
using SingerDispatch.Panels;
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

            SingerConstants.CommonDataContext.Log = System.Console.Out;

            Database = SingerConstants.CommonDataContext;
            Companies = new ObservableCollection<Company>();

            if (Database.DatabaseExists()) return;

            var builder = new DatabaseBuilder(SingerConstants.CommonDataContext);

            builder.CreateNewDatabase();
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

        private void ExpandSection(Expander expander, CompanyUserControl content, ItemCollection tabs)
        {
            if (panelMainContent.Child.GetType() == content.GetType())
            {
                return;
            }

            CollapseAllOtherNavigationExpanders(expander);

            var binding = new Binding();
            binding.ElementName = "cmbCompanies";
            binding.Path = new PropertyPath(Selector.SelectedItemProperty);

            content.SetBinding(CompanyUserControl.SelectedCompanyProperty, binding);

            panelMainContent.Child = content;
            AddLinksToExpander((Panel)expander.Content, tabs);
            expander.IsExpanded = true;
        }

        private void ExpandCompanies(object sender, RoutedEventArgs e)
        {
            var panel = new CompaniesPanel();

            ExpandSection((Expander)sender, panel, panel.Tabs.Items);
        }

        private void ExpandQuotes(object sender, RoutedEventArgs e)
        {
            var panel = new QuotesPanel();

            ExpandSection((Expander)sender, panel, panel.Tabs.Items);
        }

        private void ExpandJobs(object sender, RoutedEventArgs e)
        {
            var panel = new JobsPanel();

            ExpandSection((Expander)sender, panel, panel.Tabs.Items);
        }

        private void ExpandPricing(object sender, RoutedEventArgs e)
        {
            var panel = new JobPricingPanel();

            ExpandSection((Expander)sender, panel, panel.Tabs.Items);
        }        

        private void menuOpenQuoteSample_Click(object sender, RoutedEventArgs e)
        {
            var viewer =  new DocumentViewer();

            viewer.Show();
        }

        public void HighlightJob(Job job)
        {
            if (job.Company != cmbCompanies.SelectedItem)
            {
                return;
            }

            var panel = new JobsPanel();

            ExpandSection(expanderJobs, panel, panel.Tabs.Items);

            panel.SelectedCompany = (Company)cmbCompanies.SelectedItem;
            panel.SelectedJob = job;
        }

        private void PrintSampleDispatch(object sender, RoutedEventArgs e)
        {
            var viewer = new DocumentViewer();
            viewer.DisplayDispatchPrintout();
        }
    }
}
