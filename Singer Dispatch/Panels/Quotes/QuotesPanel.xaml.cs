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
        private CommandBinding SaveCommand { get; set; }

        private SingerDispatchDataContext Database { get; set; }

        public QuotesPanel()
        {
            InitializeComponent();
            
            SaveCommand = new CommandBinding(CustomCommands.GenericSaveCommand);
            CommandBindings.Add(SaveCommand);

            if (InDesignMode()) return;

            Database = SingerConfigs.CommonDataContext;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {               
            SaveCommand.Executed += CommitQuoteChanges_Executed;

            if (InDesignMode()) return;

            if (SelectedCompany != null)
            {
                var quotes = new ObservableCollection<Quote>(from quote in Database.Quotes where quote.Company == SelectedCompany orderby quote.Number descending, quote.Revision descending select quote);

                if (SelectedQuote != null && SelectedQuote.ID == 0)
                    quotes.Insert(0, SelectedQuote);

                dgQuoteList.ItemsSource = quotes;
            }
            else
            {
                dgQuoteList.ItemsSource = null;
            }
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            IsEnabled = newValue != null;
            SelectedQuote = null;

            dgQuoteList.ItemsSource = (SelectedCompany == null) ? null : new ObservableCollection<Quote>(from quote in Database.Quotes where quote.Company == SelectedCompany orderby quote.Number descending, quote.Revision descending select quote);

            Tabs.SelectedIndex = 0;
        }

        protected override void SelectedQuoteChanged(Quote newValue, Quote oldValue)
        {
            base.SelectedQuoteChanged(newValue, oldValue);

            Tabs.SelectedIndex = 0;
        }
      
        private void CommitQuoteChanges_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CommitChangesButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, CommitChangesButton));
        }

        private void ViewQuote_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedQuote == null) return;

            var title = String.Format("Quote #{0}-{1}", SelectedQuote.Number, SelectedQuote.Revision);

            var viewer = new Windows.DocumentViewerWindow(new QuoteDocument(), SelectedQuote, title) { IsMetric = !UseImperialMeasurements, IsSpecializedDocument = SelectedCompany.CustomerType.IsEnterprise != true };
            viewer.DisplayPrintout();
        }

        private void CommitQuoteChanges_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedQuote == null) return;

            var button = (ButtonBase)sender;

            try
            {
                Database.SubmitChanges();
                
                button.Focus();
                button.IsEnabled = false;
            }
            catch (Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void CommitChangesButton_LostFocus(object sender, RoutedEventArgs e)
        {
            var button = (ButtonBase)sender;

            button.IsEnabled = true;
        }

        private void NewQuote_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<Quote>)dgQuoteList.ItemsSource;
            var quote = new Quote { CreationDate = DateTime.Today, ExpirationDate = DateTime.Today.AddDays(30), Company = SelectedCompany, Employee = SingerConfigs.OperatingEmployee };
            
            list.Add(quote);
            dgQuoteList.SelectedItem = quote;            
            SelectedCompany.Quotes.Add(quote);

            // Add any of the default quote conditions            
            foreach (var condition in (from c in Database.Conditions where c.AutoInclude == true select c))
            {
                quote.QuoteConditions.Add(new QuoteCondition { ConditionID = condition.ID, Line = condition.Line });
            }

            try
            {
                EntityHelper.SaveAsNewQuote(quote, Database);
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void CreateRevision_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedQuote == null) return;

            var quote = SelectedQuote.Duplicate();
            var list = (ObservableCollection<Quote>)dgQuoteList.ItemsSource;

            list.Add(quote);
            dgQuoteList.SelectedItem = quote;            
            SelectedCompany.Quotes.Add(quote);

            try
            {
                EntityHelper.SaveAsQuoteRevision(quote, Database);
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void CreateJob_Click(object sender, RoutedEventArgs e)
        {
            var quote = (Quote)dgQuoteList.SelectedItem;

            if (quote == null) return;

            var confirmation = MessageBox.Show("Are you sure you wish to create a new job from the selected quote? Doing so will save any changes made to this quote.", "New job confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

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
                ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
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
                ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void dgQuoteList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var grid = (DataGrid)sender;

            if (grid.SelectedItem == null) return;

            grid.ScrollIntoView(grid.SelectedItem);
            grid.UpdateLayout();

            SelectedQuote = (Quote)grid.SelectedItem;
        }
    }
}
