using System.Windows.Controls;
using System.Linq;
using System.Transactions;
using System.Windows;
using System.ComponentModel;
using SingerDispatch.Database;

namespace SingerDispatch.Panels.Quotes
{
    /// <summary>
    /// Interaction logic for QuotesPanel.xaml
    /// </summary>
    public partial class QuotesPanel : QuoteUserControl
    {
        private SingerDispatchDataContext Database { get; set; }

        public QuotesPanel()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
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

        private void CommitQuoteChanges_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedQuote == null) return;
            
            // Check if this quote is new and not yet in the database
            if (SelectedQuote.ID == 0)
            {
                EntityHelper.SaveAsNewQuote(SelectedQuote, Database);
            }
            else
            {
                Database.SubmitChanges();
            }
        }
    }
}
