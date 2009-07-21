using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SingerDispatch.Controls;
using SingerDispatch.Panels.Quotes;
using SingerDispatch.Panels.Companies;
using SingerDispatch.Panels.Jobs;
using System;

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

            this.database = SingerConstants.CommonDataContext;
            this.companies = new ObservableCollection<Company>();

            if (!database.DatabaseExists())
            {
                database.CreateDatabase();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            companies = new ObservableCollection<Company>(
                (from c in database.Companies orderby c.Name select c).ToList()
            );

            cmbCompanies.ItemsSource = companies;
            cmbOperators.ItemsSource = companies;

            expanderCompanies.IsExpanded = true; 
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

        private void AddLinksToExpander(StackPanel panel, ItemCollection tabs)
        {
            panel.Children.Clear();

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

        private void CollapseAllOtherNavigationExpanders(Expander self)
        {
            foreach (Control control in altNavigationStack.Children)
            {
                if (control is Expander)
                {
                    Expander expander = (Expander)control;

                    if (expander != self && expander.IsExpanded)
                    {
                        expander.IsExpanded = false;
                    }
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

            CompaniesPanel panel = new CompaniesPanel();

            Binding binding = new Binding();
            binding.ElementName = "cmbCompanies";
            binding.Path = new PropertyPath(ComboBox.SelectedItemProperty);

            panel.SetBinding(CompaniesPanel.SelectedCompanyProperty, binding);

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

            QuotesPanel panel = new QuotesPanel();

            Binding binding = new Binding();
            binding.ElementName = "cmbCompanies";
            binding.Path = new PropertyPath(ComboBox.SelectedItemProperty);

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

            JobsPanel panel = new JobsPanel();

            panelMainContent.Child = panel;
            AddLinksToExpander(linksJobs, panel.tabs.Items);   
        }
    }
}
