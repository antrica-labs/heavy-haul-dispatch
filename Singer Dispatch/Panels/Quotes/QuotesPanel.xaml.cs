using System.Windows;
using System.Windows.Controls.Primitives;
using SingerDispatch.Database;
using System.Windows.Input;
using SingerDispatch.Controls;

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

            Database = SingerConstants.CommonDataContext;

            SaveCommand = new CommandBinding(CustomCommands.GenericSaveCommand);
            CommandBindings.Add(SaveCommand);
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {               
            SaveCommand.Executed += CommitQuoteChanges_Executed;
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
      
        private void CommitQuoteChanges_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CommitChangesButton.Focus();
            CommitChangesButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, CommitChangesButton));
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
                Database.SubmitChanges();
            }
            catch (System.Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        
    }
}
