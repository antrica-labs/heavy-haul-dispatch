using System.Windows;
using SingerDispatch.Database;
using System.Windows.Input;
using SingerDispatch.Controls;
using System.Windows.Automation.Peers;

namespace SingerDispatch.Panels.Quotes
{
    /// <summary>
    /// Interaction logic for QuotesPanel.xaml
    /// </summary>
    public partial class QuotesPanel : QuoteUserControl
    {
        private CommandBinding SaveCommand { get; set; }

        private SingerDispatchDataContext Database { get; set; }

        public QuotesPanel()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;

            SaveCommand = new CommandBinding(CustomCommands.GenericSaveCommand);
            CommandBindings.Add(SaveCommand);
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {               
            SaveCommand.Executed += new ExecutedRoutedEventHandler(CommitQuoteChanges_Executed);
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            IsEnabled = newValue != null;
            SelectedQuote = null;

            Tabs.SelectedIndex = 0;
        }

        protected override void SelectedQuoteChanged(Quote newValue, Quote oldValue)
        {
            base.SelectedQuoteChanged(newValue, oldValue);

            Tabs.SelectedIndex = 0;
        }
      
        private void DiscardQuoteChanges_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to discard all changes made to this quote?", "Discard confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;
            
            SelectedQuote = null;            
            Tabs.SelectedIndex = 0;
        }

        private void CommitQuoteChanges_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CommitChangesButton.Focus();
            CommitChangesButton.RaiseEvent(new System.Windows.RoutedEventArgs(System.Windows.Controls.Button.ClickEvent, CommitChangesButton));
        }

        private void CommitQuoteChanges_Click(object sender, RoutedEventArgs e)
        {
            CommitQuoteChanges();
        }

        private void CommitQuoteChanges()
        {
            if (SelectedQuote == null) return;

            try
            {
                // Check if this quote is new and not yet in the database
                if (SelectedQuote.ID == 0)
                {
                    if (SelectedQuote.Number != null)
                    {
                        SelectedCompany.Quotes.Add(SelectedQuote);
                        EntityHelper.SaveAsQuoteRevision(SelectedQuote, Database);
                    }
                    else
                    {
                        EntityHelper.SaveAsNewQuote(SelectedQuote, Database);
                    }
                }
                else
                {
                    Database.SubmitChanges();
                }
            }
            catch (System.Exception ex)
            {
                SingerDispatch.Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        
    }
}
