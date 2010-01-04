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
using SingerDispatch.Panels.Invoicing;
using SingerDispatch.Database;
using System.Collections.Generic;
using System;
using System.Reflection;
using SingerDispatch.Windows;
using SingerDispatch.Panels.Admin;
using System.Windows.Input;

namespace SingerDispatch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Dictionary<System.Type, UserControl> Panels { get; set; }        
        private ObservableCollection<Company> Companies { get; set; }
        private SingerDispatchDataContext Database { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            SetupKeyBindings();
            /*
            InputBinding ib = new InputBinding(CustomCommands.MyCommand, new KeyGesture(Key.Y, ModifierKeys.Control));
            this.InputBindings.Add(ib);

            CommandBinding cb = new CommandBinding(CustomCommands.MyCommand);
            cb.Executed += new ExecutedRoutedEventHandler(HandlerThatDoesSomething);
            this.CommandBindings.Add(cb);
            */

            try
            {
                Panels = new Dictionary<System.Type, UserControl>();
                Database = SingerConstants.CommonDataContext;
                Companies = new ObservableCollection<Company>();

                if (Database.DatabaseExists()) return;

                var builder = new DatabaseBuilder(SingerConstants.CommonDataContext);

                builder.CreateNewDatabase();
            }
            catch (Exception e)
            {
                var box = new ErrorNoticeWindow("Database Error", e.Message);
                box.ShowDialog();

                Application.Current.Shutdown();
            }
        }

        private void SetupKeyBindings()
        {
            var cb1 = new CommandBinding(CustomCommands.QuoteLoookupCommand);
            cb1.Executed += new ExecutedRoutedEventHandler(QuoteLoookupCommandHandler);
            this.CommandBindings.Add(cb1);

            var cb2 = new CommandBinding(CustomCommands.JobLookupCommand);
            cb2.Executed += new ExecutedRoutedEventHandler(JobLookupCommandHandler);
            this.CommandBindings.Add(cb2);

            var cb3 = new CommandBinding(CustomCommands.InvoiceLookupCommand);
            cb3.Executed += new ExecutedRoutedEventHandler(InvoiceLookupCommandHandler);
            this.CommandBindings.Add(cb3);

            var cb4 = new CommandBinding(CustomCommands.CreateCompanyCommand);
            cb4.Executed += new ExecutedRoutedEventHandler(CreateCompanyCommandHandler);
            this.CommandBindings.Add(cb4);

            var cb5 = new CommandBinding(CustomCommands.EditCompaniesCommand);
            cb5.Executed += new ExecutedRoutedEventHandler(EditCompaniesCommandHandler);
            this.CommandBindings.Add(cb5);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Companies = new ObservableCollection<Company>(
                from c in Database.Companies where c.IsVisible == true orderby c.Name select c
            );

            cmbCompanies.ItemsSource = Companies;
            cmbOperators.ItemsSource = Companies;

            expanderCompanies.IsExpanded = true;
            cmbCompanies.Focus();
        }

        private void CreateCompanyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CreateCompany();
        }

        private void cmbCompanies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbOperators.SelectedItem = cmbCompanies.SelectedItem;
        }

        private void cmbOperators_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbCompanies.SelectedItem = cmbOperators.SelectedItem;
        }

        private void Terminate_Click(object sender, RoutedEventArgs e)
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

			if (link.Tab.IsEnabled)
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

        private void ExpandSection(Expander expander, System.Type panelType)
        {
            if (panelMainContent.Child.GetType() == panelType)
            {
                // This panel is already visible, so don't do anything
                return;               
            }
            
            CollapseAllOtherNavigationExpanders(expander);

            UserControl panel;

            try
            {
                panel = Panels[panelType];
            }
            catch
            {
                panel = (UserControl)Activator.CreateInstance(panelType);
                var fields = panel.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
                               
                foreach (var field in fields)
                {
                    if (field.Name == "Tabs" && field.FieldType == typeof(TabControl))
                    {
                        var tabs = (TabControl)field.GetValue(panel);
                        AddLinksToExpander((Panel)expander.Content, tabs.Items);

                        break;
                    }
                }

                var binding = new Binding();
                binding.ElementName = "cmbCompanies";
                binding.Path = new PropertyPath(Selector.SelectedItemProperty);
                panel.SetBinding(CompanyUserControl.SelectedCompanyProperty, binding);
                
                Panels.Add(panel.GetType(), panel);
            }

            expander.IsExpanded = true;
            panelMainContent.Child = panel;
        }

        private void ExpandCompanies(object sender, RoutedEventArgs e)
        {
            ExpandSection(expanderCompanies, typeof(CompaniesPanel));
        }

        private void ExpandQuotes(object sender, RoutedEventArgs e)
        {
            ExpandSection(expanderQuotes, typeof(QuotesPanel));
        }

        private void ExpandJobs(object sender, RoutedEventArgs e)
        {
            ExpandSection(expanderJobs, typeof(JobsPanel));
        }

        private void ExpandInvoicing(object sender, RoutedEventArgs e)
        {
            ExpandSection(expanderInvoicing, typeof(JobInvoicingPanel));
        }

        private void ExpandAdmin(object sender, RoutedEventArgs e)
        {
            ExpandSection(expanderAdmin, typeof(AdminPanel));
        }

        public void ViewInvoice(Job job)
        {
            if (job.Company == null)
            {
                return;
            }

            cmbCompanies.SelectedItem = job.Company;

            ExpandSection(expanderInvoicing, typeof(JobInvoicingPanel));

            ((JobInvoicingPanel)panelMainContent.Child).SelectedJob = null;
            ((JobInvoicingPanel)panelMainContent.Child).SelectedCompany = null;

            ((JobInvoicingPanel)panelMainContent.Child).SelectedCompany = job.Company;
            ((JobInvoicingPanel)panelMainContent.Child).SelectedJob = job;
        }

        public void ViewJob(Job job)
        {
            if (job.Company == null)
            {
                return;
            }

            cmbCompanies.SelectedItem = job.Company;            
                        
            ExpandSection(expanderJobs, typeof(JobsPanel));

            ((JobsPanel)panelMainContent.Child).SelectedJob = null;
            ((JobsPanel)panelMainContent.Child).SelectedCompany = null;             

            ((JobsPanel)panelMainContent.Child).SelectedCompany = job.Company;
            ((JobsPanel)panelMainContent.Child).SelectedJob = job;           
        }

        public void ViewQuote(Quote quote)
        {
            if (quote.Company == null)
            {
                return;
            }
            
            cmbCompanies.SelectedItem = quote.Company;

            ExpandSection(expanderQuotes, typeof(QuotesPanel));

            ((QuotesPanel)panelMainContent.Child).SelectedQuote = null;
            ((QuotesPanel)panelMainContent.Child).SelectedCompany = null;
                
            ((QuotesPanel)panelMainContent.Child).SelectedCompany = quote.Company;            
            ((QuotesPanel)panelMainContent.Child).SelectedQuote = quote;
        }

        private void EditCompaniesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditCompanies();   
        }

        private void FindQuote_Click(object sender, RoutedEventArgs e)
        {
            FindQuote();
        }

        private void FindJob_Click(object sender, RoutedEventArgs e)
        {
            FindJob();
        }

        private void FindJobForInvoice_Click(object sender, RoutedEventArgs e)
        {
            FindInvoice();
        }


        private void QuoteLoookupCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            FindQuote();
        }

        private void JobLookupCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            FindJob();
        }

        private void InvoiceLookupCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            FindInvoice();
        }

        private void CreateCompanyCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            CreateCompany();
        }

        private void EditCompaniesCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            EditCompanies();
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void FindQuote()
        {
            var window = new QuoteLocatorWindow() { Owner = this };
            var quote = window.GetQuote();

            if (quote != null)
            {
                ViewQuote(quote);
            }            
        }

        private void FindJob()
        {
            var window = new JobLocatorWindow() { Owner = this };
            var job = window.GetJob();

            if (job != null)
            {
                ViewJob(job);
            }
        }

        private void FindInvoice()
        {
            var window = new JobLocatorWindow() { Owner = this };
            var job = window.GetJob();

            if (job != null)
            {
                ViewInvoice(job);
            }
        }

        private void CreateCompany()
        {
            var window = new CreateCompanyWindow(Database, Companies) { Owner = this };
            Company newCompany = window.CreateCompany();

            if (newCompany != null)
            {
                cmbCompanies.SelectedItem = newCompany;
            }
        }

        private void EditCompanies()
        {
            var window = new EditCompaniesWindow() { Owner = this };
            window.ShowDialog();

            cmbCompanies.SelectedItem = null;

            Companies = new ObservableCollection<Company>(
                from c in Database.Companies where c.IsVisible == true orderby c.Name select c
            );

            cmbCompanies.ItemsSource = Companies;
            cmbOperators.ItemsSource = Companies;
        }

        
    }
}
