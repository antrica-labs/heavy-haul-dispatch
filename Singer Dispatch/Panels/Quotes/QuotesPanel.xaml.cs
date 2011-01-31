using System.Windows;
using System.Windows.Controls.Primitives;
using SingerDispatch.Database;
using System.Windows.Input;
using SingerDispatch.Controls;
using System;
using SingerDispatch.Printing.Documents;
using System.Collections.ObjectModel;
using System.Linq;
using SingerDispatch.Windows;
using System.Windows.Controls;

namespace SingerDispatch.Panels.Quotes
{
    /// <summary>
    /// Interaction logic for QuotesPanel.xaml
    /// </summary>
    public partial class QuotesPanel
    {
        public SingerDispatchDataContext Database { get; set; }

        public QuotesPanel()
        {
            InitializeComponent();

            if (InDesignMode()) return;

            Database = SingerConfigs.CommonDataContext;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode() || IsVisible == false) return;

            UpdateQuoteList();
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            if (!IsVisible) return;

            UpdateQuoteList();
        }

        protected override void SelectedQuoteChanged(Quote newValue, Quote oldValue)
        {
            base.SelectedQuoteChanged(newValue, oldValue);

            if (newValue == null)
                Tabs.SelectedIndex = 0;
        }

        private void UpdateQuoteList()
        {
            if (SelectedCompany != null)
            {
                var selected = dgQuoteList.SelectedItem;
                dgQuoteList.ItemsSource = new ObservableCollection<Quote>(from quote in Database.Quotes where quote.Company == SelectedCompany orderby quote.Number descending, quote.Revision descending select quote);
                dgQuoteList.SelectedItem = selected;
            }
            else
                dgQuoteList.ItemsSource = null;
        }

        private void ViewQuote_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedQuote == null) return;

            var title = String.Format("Quote #{0}-{1}", SelectedQuote.Number, SelectedQuote.Revision);

            var viewer = new Windows.DocumentViewerWindow(new QuoteDocument(), SelectedQuote, title) { IsMetric = !UseImperialMeasurements, IsSpecializedDocument = SelectedCompany.CustomerType.IsEnterprise != true };
            viewer.DisplayPrintout();
        }

        private void NewQuote_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<Quote>)dgQuoteList.ItemsSource;
            var quote = new Quote { CreationDate = DateTime.Today, ExpirationDate = DateTime.Today.AddDays(30), Company = SelectedCompany, Employee = SingerConfigs.OperatingEmployee };

            var addresses = from a in SelectedCompany.Addresses select a;

            if (addresses.Count() > 0)
                quote.BillingAddress = addresses.First();

            SelectedCompany.Quotes.Add(quote);
            list.Insert(0, quote);
            dgQuoteList.SelectedItem = quote;

            // Add any of the default quote conditions            
            foreach (var condition in (from c in Database.Conditions where c.Archived != true && c.AutoInclude == true select c))
            {
                quote.QuoteConditions.Add(new QuoteCondition { ConditionID = condition.ID, Line = condition.Line });
            }

            try
            {
                EntityHelper.SaveAsNewQuote(quote, Database);
            }
            catch (Exception ex)
            {
                NoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }

        private void CreateRevision_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedQuote == null) return;

            var quote = SelectedQuote.Duplicate();
            var list = (ObservableCollection<Quote>)dgQuoteList.ItemsSource;

            SelectedCompany.Quotes.Add(quote);
            list.Insert(0, quote);
            dgQuoteList.SelectedItem = quote;

            try
            {
                EntityHelper.SaveAsQuoteRevision(quote, Database);
            }
            catch (Exception ex)
            {
                NoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }

        private void CreateJob_Click(object sender, RoutedEventArgs e)
        {
            var quote = (Quote)dgQuoteList.SelectedItem;

            if (quote == null) return;

            var confirmation = MessageBox.Show("Are you sure you wish to create a new job from the selected quote?", "New job confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            var window = (MainWindow)Application.Current.MainWindow;
            var job = quote.ToJob();

            try
            {
                job.Status = (from s in Database.Statuses where s.Name == "Pending" select s).First();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }

            SelectedCompany.Jobs.Add(job);
            Database.Jobs.InsertOnSubmit(job);

            try
            {
                EntityHelper.SaveAsNewJob(job, Database);

                window.ViewJob(job);
            }
            catch (Exception ex)
            {
                NoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }


        private void DeleteQuote_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedQuote == null) return;

            var confirmation = MessageBox.Show("Are you sure you wish to delete this quote and all of it's items?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            try
            {
                EntityHelper.PrepareEntityDelete(SelectedQuote, Database);

                SelectedCompany.Quotes.Remove(SelectedQuote);
                Database.SubmitChanges();

                ((ObservableCollection<Quote>)dgQuoteList.ItemsSource).Remove(SelectedQuote);
            }
            catch (Exception ex)
            {
                NoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }

        private void dgQuoteList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var grid = (DataGrid)sender;

            if (grid.SelectedItem == null) return;

            grid.ScrollIntoView(grid.SelectedItem);
            grid.UpdateLayout();
        }
    }
}
