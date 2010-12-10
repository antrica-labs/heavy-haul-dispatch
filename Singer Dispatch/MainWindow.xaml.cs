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
using System.Collections.Generic;
using System;
using System.Reflection;
using SingerDispatch.Windows;
using SingerDispatch.Panels.Admin;
using System.Windows.Input;
using SingerDispatch.Panels.Storage;
using System.Windows.Interop;
using System.ComponentModel;
using SingerDispatch.Security;
using SingerDispatch.Panels.Loads;
using WPFAutoCompleteBox.Controls;

namespace SingerDispatch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private UserSettings Settings { get; set; }
        private Dictionary<Type, BaseUserControl> Panels { get; set; }        
        private SingerDispatchDataContext Database { get; set; }

        public static DependencyProperty CompaniesProperty = DependencyProperty.Register("Companies", typeof(ObservableCollection<Company>), typeof(BaseUserControl));
        public static DependencyProperty UseImperialMeasurementsProperty = DependencyProperty.Register("UseImperialMeasurements", typeof(Boolean), typeof(MainWindow), new PropertyMetadata(false, UseImperialMeasurementsPropertyChanged));
        
        public ObservableCollection<Company> Companies 
        {
            get
            {
                return (ObservableCollection<Company>)GetValue(CompaniesProperty);
            }
            set
            {
                SetValue(CompaniesProperty, value);
            }
        }
        
        public Boolean UseImperialMeasurements
        {
            get
            {
                return (Boolean)GetValue(UseImperialMeasurementsProperty);
            }
            set
            {                
                SetValue(UseImperialMeasurementsProperty, value);                
            }
        }

        public static void UseImperialMeasurementsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (MainWindow)d;

            control.UseImperialMeasurementsChanged((Boolean)e.NewValue);
        }
        
        public MainWindow()
        {
            InitializeComponent();

            Settings = new UserSettings();

            UseImperialMeasurements = !Settings.MetricMeasurements;

            SetupKeyBindings();

            try
            {
                Panels = new Dictionary<Type, BaseUserControl>();
                Database = SingerConfigs.CommonDataContext;
                Companies = new ObservableCollection<Company>();

                if (!Database.DatabaseExists()) 
                    throw new Exception("Unable to connect to the required database!");

                SingerConfigs.OperatingEmployee = GetOperatingEmployee();
            }
            catch (Exception e)
            {
                ErrorNoticeWindow.ShowError("Database Error", e.Message);                

                Application.Current.Shutdown();
            }
        }

        private Employee GetOperatingEmployee()
        {
            try
            {
                var username = AuthenticationService.GetCurrentUserName();
                var employee = (from em in Database.Employees where em.WindowsUserName == username select em).First();

                return employee;
            }
            catch
            {
                return null;
            }
        }
        
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            WindowPlacement.SetPlacement(new WindowInteropHelper(this).Handle, Settings.MainWindowPlacement);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Companies = new ObservableCollection<Company>(
                from c in Database.Companies where c.IsVisible == true orderby c.Name select c
            );

            expanderCompanies.IsExpanded = true;            
            acCompany.Focus();

            try
            {
                var acManager = new WPFAutoCompleteBox.Core.AutoCompleteManager(acCompany);
                acManager.DataProvider = new SingerDispatch.Database.CompleteProviders.CompanyNameACProvider(Database);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Settings.MainWindowPlacement = WindowPlacement.GetPlacement(new WindowInteropHelper(this).Handle);
            Settings.Save();
        }

        public void UseImperialMeasurementsChanged(Boolean value)
        {
            Settings.MetricMeasurements = (value == false);
            Settings.Save();
        }

        private void SetupKeyBindings()
        {
            var cb1 = new CommandBinding(CustomCommands.QuoteLoookupCommand);
            cb1.Executed += QuoteLoookupCommandHandler;
            CommandBindings.Add(cb1);

            var cb2 = new CommandBinding(CustomCommands.JobLookupCommand);
            cb2.Executed += JobLookupCommandHandler;
            CommandBindings.Add(cb2);

            var cb3 = new CommandBinding(CustomCommands.InvoiceLookupCommand);
            cb3.Executed += InvoiceLookupCommandHandler;
            CommandBindings.Add(cb3);

            var cb4 = new CommandBinding(CustomCommands.CreateCompanyCommand);
            cb4.Executed += CreateCompanyCommandHandler;
            CommandBindings.Add(cb4);

            var cb5 = new CommandBinding(CustomCommands.EditCompaniesCommand);
            cb5.Executed += EditCompaniesCommandHandler;
            CommandBindings.Add(cb5);
        }

        private void CreateCompanyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CreateCompany();
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

        private void ExpandSection(Expander expander, Type panelType)
        {
            if (panelMainContent.Child.GetType() == panelType)
            {
                // This panel is already visible, so don't do anything
                return;               
            }
            
            // Attempt to save anything in the common data context
            try
            {
                SingerConfigs.CommonDataContext.SubmitChanges();
            }
            catch { }

            CollapseAllOtherNavigationExpanders(expander);

            BaseUserControl panel;

            if (!Panels.ContainsKey(panelType))
            {
                panel = (BaseUserControl)Activator.CreateInstance(panelType);
                
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

                var companyListBinding = new Binding { ElementName = "ThisWindow", Path = new PropertyPath(MainWindow.CompaniesProperty) };
                var companyBinding = new Binding { ElementName = "acCompany", Path = new PropertyPath(CompletableTextBox.SelectedItemProperty) };
                var imperialBinding = new Binding { ElementName = "ThisWindow", Path = new PropertyPath(MainWindow.UseImperialMeasurementsProperty) };

                panel.SetBinding(BaseUserControl.CompanyListProperty, companyListBinding);
                panel.SetBinding(CompanyUserControl.SelectedCompanyProperty, companyBinding);
                panel.SetBinding(BaseUserControl.UseImperialMeasurementsProperty, imperialBinding);

                Panels.Add(panel.GetType(), panel);
            }
            else
            {
                panel = Panels[panelType];
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
            try
            {
                ExpandSection(expanderQuotes, typeof(QuotesPanel));
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError(ex.Message, ex.ToString());
            }
        }

        private void ExpandJobs(object sender, RoutedEventArgs e)
        {
            ExpandSection(expanderJobs, typeof(JobsPanel));
        }

        private void ExpandLoads(object sender, RoutedEventArgs e)
        {
            ExpandSection(expanderLoads, typeof(LoadsPanel));
        }

        private void ExpandInvoicing(object sender, RoutedEventArgs e)
        {
            ExpandSection(expanderInvoicing, typeof(JobInvoicingPanel));
        }

        private void ExpandStorage(object sender, RoutedEventArgs e)
        {
            ExpandSection(expanderStorage, typeof(StoragePanel));
        }

        private void ExpandAdmin(object sender, RoutedEventArgs e)
        {
            ExpandSection(expanderAdmin, typeof(AdminPanel));
        }

        public void ViewInvoices(Job job)
        {
            if (job == null || job.Company == null) return;

            expanderInvoicing.IsExpanded = true;
            acCompany.SelectedItem = job.Company;

            ((JobInvoicingPanel)panelMainContent.Child).UpdateLayout();
            ((JobInvoicingPanel)panelMainContent.Child).SelectedJob = job;
        }

        public void ViewJob(Job job)
        {
            if (job == null || job.Company == null) return;

            expanderJobs.IsExpanded = true;            
            acCompany.SelectedItem = job.Company;

            ((JobsPanel)panelMainContent.Child).UpdateLayout();
            ((JobsPanel)panelMainContent.Child).SelectedJob = job;
        }

        public void ViewLoads(Job job)
        {
            if (job == null || job.Company == null) return;

            expanderLoads.IsExpanded = true;
            acCompany.SelectedItem = job.Company;

            ((LoadsPanel)panelMainContent.Child).UpdateLayout();
            ((LoadsPanel)panelMainContent.Child).SelectedJob = job;
        }

        public void ViewQuote(Quote quote)
        {
            if (quote == null || quote.Company == null) return;

            expanderQuotes.IsExpanded = true;
            acCompany.SelectedItem = quote.Company;

            ((QuotesPanel)panelMainContent.Child).UpdateLayout();
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
            FindInvoices();
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
            FindInvoices();
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
            var window = new QuoteLocatorWindow { Owner = this };
            var quote = window.GetQuote();

            if (quote != null)
            {
                ViewQuote(quote);
            }            
        }

        private void FindJob()
        {
            var window = new JobLocatorWindow { Owner = this };
            var job = window.GetJob();

            if (job != null)
            {
                ViewJob(job);
            }
        }

        private void FindInvoices()
        {
            var window = new JobLocatorWindow { Owner = this };
            var job = window.GetJob();

            if (job != null)
            {
                ViewInvoices(job);
            }
        }

        private void CreateCompany()
        {
            var window = new CreateCompanyWindow(Database, Companies) { Owner = this };
            Company newCompany = window.CreateCompany();
        }

        private void EditCompanies()
        {
            var window = new EditCompaniesWindow { Owner = this };
            window.ShowDialog();
                        
            acCompany.SelectedItem = null;

            Companies = new ObservableCollection<Company>(
                from c in Database.Companies where c.IsVisible == true orderby c.Name select c
            );
        }

        
    }
}
