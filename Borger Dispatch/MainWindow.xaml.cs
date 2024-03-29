﻿using System.Collections.ObjectModel;
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
using SingerDispatch.Printing.Documents;
using SingerDispatch.Database;
using SingerDispatch.Printing.Excel;

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

                SingerConfigs.PreloadConfigs();
                SingerConfigs.OperatingEmployee = GetOperatingEmployee();
            }
            catch (Exception e)
            {
                NoticeWindow.ShowError("Startup error", e.ToString());                

                Application.Current.Shutdown();
            }
        }

        private Employee GetOperatingEmployee()
        {
            try
            {
                var username = AuthenticationService.GetCurrentUserName();
                var employee = (from em in Database.Employees where em.Archived != true && em.WindowsUserName == username select em).First();

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
            RefreshCompanies();

            expanderCompanies.IsExpanded = true;            
            acCompany.Focus();

            try
            {
                var acManager = new WPFAutoCompleteBox.Core.AutoCompleteManager(acCompany);
                acManager.DataProvider = new SingerDispatch.Database.CompleteProviders.CompanyNameACProvider(Companies);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                Database.SubmitChanges();                
            }
            catch (System.Exception ex)
            {                
                Windows.NoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);

                if (Database != null)
                    Database.RevertChanges();
            }

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

            var cb4 = new CommandBinding(CustomCommands.CreateCompanyCommand);
            cb4.Executed += CreateCompanyCommandHandler;
            CommandBindings.Add(cb4);

            var cb5 = new CommandBinding(CustomCommands.EditCompaniesCommand);
            cb5.Executed += EditCompaniesCommandHandler;
            CommandBindings.Add(cb5);

            var cb6 = new CommandBinding(CustomCommands.InvoiceLookupCommand);
            cb6.Executed += InvoiceLookupCommandHandler;
            CommandBindings.Add(cb6);

            var cb7 = new CommandBinding(CustomCommands.LoadsLookupCommand);
            cb7.Executed += LoadsLookupCommandHandler;
            CommandBindings.Add(cb7);
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
            catch
            {
                SingerConfigs.CommonDataContext.RevertChanges();
            }

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
                NoticeWindow.ShowError(ex.Message, ex.ToString());
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
            ExpandSection(expanderInvoicing, typeof(InvoicingPanel));
        }        

        private void ExpandStorage(object sender, RoutedEventArgs e)
        {
            ExpandSection(expanderStorage, typeof(StoragePanel));
        }

        private void ExpandAdmin(object sender, RoutedEventArgs e)
        {
            ExpandSection(expanderAdmin, typeof(AdminPanel));
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

        public void ViewLoad(Load load)
        {
            if (load == null || load.Job == null) return;

            ViewLoads(load.Job);

            ((LoadsPanel)panelMainContent.Child).SelectedLoad = load;
        }

        public void ViewPermit(Permit permit)
        {
            if (permit == null || permit.Load == null || permit.Load.Job == null) return;

            ViewLoad(permit.Load);

            ((LoadsPanel)panelMainContent.Child).SelectedPermit = permit;
        }

        public void ViewThirdPartyService(ThirdPartyService service)
        {
            if (service == null || service.Load == null || service.Load.Job == null) return;

            ViewLoad(service.Load);
            ((LoadsPanel)panelMainContent.Child).SelectedThirdPartyService = service;
        }

        public void ViewDispatch(Dispatch dispatch)
        {
            if (dispatch == null || dispatch.Load == null || dispatch.Load.Job == null) return;

            ViewLoad(dispatch.Load);

            var panel = (LoadsPanel)panelMainContent.Child;

            panel.Tabs.SelectedIndex = 2;
        }

        public void ViewInvoice(Invoice invoice)
        {
            if (invoice == null || invoice.Company == null) return;

            expanderInvoicing.IsExpanded = true;
            acCompany.SelectedItem = invoice.Company;

            ((InvoicingPanel)panelMainContent.Child).UpdateLayout();
            ((InvoicingPanel)panelMainContent.Child).SelectedInvoice = invoice;
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

        private void RefreshCompaniesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            acCompany.SelectedItem = null;

            RefreshCompanies();
        }

        private void FindQuote_Click(object sender, RoutedEventArgs e)
        {
            FindQuote();
        }

        private void FindJob_Click(object sender, RoutedEventArgs e)
        {
            FindJob();
        }

        private void FindLoads_Click(object sender, RoutedEventArgs e)
        {
            FindLoads();
        }

        private void FindInvoice_Click(object sender, RoutedEventArgs e)
        {
            FindInvoice();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            DoSearch();
        }

        private void ViewStorageList_Click(object sender, RoutedEventArgs e)
        {
            var storage = from si in Database.StorageItems where si.Job != null orderby si.Number descending select si;
            var items = from s in storage where s.JobCommodity != null && (s.DateRemoved == null || s.DateRemoved.Value.Date > DateTime.Today.Date) orderby s.JobCommodity.Owner.Name select s;            
            var title = "Storage List - Current";

            var viewer = new DocumentViewerWindow(new StorageListDocument(), items, title) { IsMetric = !UseImperialMeasurements };
            viewer.DisplayPrintout(); 
        }

        private void ViewArchivedStorageList_Click(object sender, RoutedEventArgs e)
        {
            var storage = from si in Database.StorageItems orderby si.Number descending select si;
            var items = from s in storage where s.JobCommodity != null && (s.DateRemoved != null && s.DateRemoved.Value.Date <= DateTime.Today.Date) orderby s.JobCommodity.Owner.Name select s;
            var title = "Storage List - Archive";

            var viewer = new DocumentViewerWindow(new StorageListDocument(), items, title) { IsMetric = !UseImperialMeasurements };
            viewer.DisplayPrintout();
        }

        private void ViewJobList_Click(object sender, RoutedEventArgs e)
        {

            var jobs = from j in Database.Jobs where j.Company != null orderby j.Number descending select j;
            var title = "Job List";

            var viewer = new Windows.DocumentViewerWindow(new JobListDocument(), new JobListExcel(), jobs, title) { IsMetric = !UseImperialMeasurements };
            viewer.DisplayPrintout();
        }

        private void ViewCurrentDispatches_Click(object sender, RoutedEventArgs e)
        {
            var dispatches = from d in Database.Dispatches
                             where d.Load != null && d.Load.Job != null && d.Load.Job.Company != null && d.MeetingDate != null
                             && (d.MeetingDate >= DateTime.Now.AddDays(-1).Date && d.MeetingDate <= DateTime.Now.AddDays(1).Date)
                             && (d.Load.Status.Name == "In Process" || d.Load.Status.Name == "Pending" || d.Load.Status.Name == "Delayed")
                             orderby d.Load.Job.Company.Name, d.Load.Job.Number, d.Load.Number, d.MeetingDate
                             select d;

            var title = "Job Dispatch List";

            var viewer = new Windows.DocumentViewerWindow(new DispatchListDocument(), new DispatchListExcel(), dispatches, title) { IsMetric = !UseImperialMeasurements };
            viewer.DisplayPrintout();
        }
        

        private void ViewQuoteList_Click(object sender, RoutedEventArgs e)
        {
            var quotes = from q in Database.Quotes where q.Company != null orderby q.ID descending select q;
            var title = "Quote List";

            var viewer = new Windows.DocumentViewerWindow(new QuoteListDocument(), new QuoteListExcel(), quotes, title) { IsMetric = !UseImperialMeasurements };
            viewer.DisplayPrintout();
        }

        private void CommodityLocationsReport_Click(object sender, RoutedEventArgs e)
        {
            var company = acCompany.SelectedItem as Company;
            var title = "Commodity List";

            if (company == null)
            {
                MessageBox.Show("Please load a company before running this report.");

                return;
            }

            var viewer = new Windows.DocumentViewerWindow(new CommodityListDocument(), new CommodityListExcel(), company, title) { IsMetric = !UseImperialMeasurements };
            viewer.DisplayPrintout();
                
        }

        private void QuoteLoookupCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            FindQuote();
        }

        private void JobLookupCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            FindJob();
        }

        private void LoadsLookupCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            FindLoads();
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

        private void FindLoads()
        {
            var window = new JobLocatorWindow { Owner = this };
            var job = window.GetJob();

            if (job != null)
            {
                ViewLoads(job);
            }
        }

        private void FindInvoice()
        {
            var window = new InvoiceLocatorWindow { Owner = this };
            var invoice = window.GetInvoice();

            if (invoice != null)
            {
                ViewInvoice(invoice);
            }            
        }

        private void DoSearch()
        {
            var window = new GeneralSearchWindow { Owner = this };
            var entity = window.FindSomething();

            if (entity == null)
                return;
            else if (entity is Job)
                ViewJob(entity as Job);
            else if (entity is Load)
                ViewLoad(entity as Load);
            else if (entity is Permit)
                ViewPermit(entity as Permit);
            else if (entity is Dispatch)
                ViewDispatch(entity as Dispatch);
            else if (entity is ThirdPartyService)
                ViewThirdPartyService(entity as ThirdPartyService);
            else if (entity is Quote)
                ViewQuote(entity as Quote);
            else if (entity is Invoice)
                ViewInvoice(entity as Invoice);
        }

        private void CreateCompany()
        {
            var window = new CreateCompanyWindow(Database) { Owner = this };
            Company newCompany = window.CreateCompany();

            if (newCompany == null) return;

            try
            {
                Database.SubmitChanges();

                Companies.Add(newCompany);
                acCompany.SelectedItem = newCompany;
            }
            catch (Exception ex)
            {
                NoticeWindow.ShowError("Error while creating company", ex.Message);

                Database.RevertChanges();
            }
                
        }

        private void EditCompanies()
        {
            var window = new EditCompaniesWindow { Owner = this };
            window.ShowDialog();
                        
            acCompany.SelectedItem = null;

            RefreshCompanies();            
        }

        private void RefreshCompanies()
        {
            Companies = new ObservableCollection<Company>(
                from c in Database.Companies where c.IsVisible == true orderby c.Name select c
            );
        }

        private void txtCompanyPriorityLevel_TextChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox)sender;
            var company = acCompany.SelectedItem as Company;

            if (company != null && company.CompanyPriorityLevel != null && company.CompanyPriorityLevel.Level >= 6)
                box.Style = TryFindResource("BadInfo") as Style;
            else
                box.Style = TryFindResource("GoodInfo") as Style;
        }

        private void PrintOutOfProvinceReport_Click(object sender, RoutedEventArgs e)
        {
            var selector = new DateRangeSelectionWindow() { Owner = this };
            var range = selector.GetDates();

            if (range != null)
            {
                if (range.StartDate == null || range.EndDate == null)
                    throw new Exception("Both start date and end date must be filled in");

                var generator = new OutOfProvinceBuilderWindow() { Owner = this };
                generator.BuildReport(range.StartDate, range.EndDate);
            }
        }     
   
        private void RevenueReport_Click(object sender, RoutedEventArgs e)
        {
            var selector = new DateRangeSelectionWindow() { Owner = this };
            var range = selector.GetDates();

            if (range != null)
            {
                if (range.StartDate == null || range.EndDate == null)
                    throw new Exception("Both start date and end date must be filled in");

                var generator = new RevenueReportBuilder() { Owner = this };
                generator.BuildReport(range.StartDate, range.EndDate);
            }
        }

        private void GetSupport_Click(object sender, RoutedEventArgs e)
        {
            var window = new GetSupportWindow() { Owner = this };
            window.ShowDialog();
        }
    }
}
