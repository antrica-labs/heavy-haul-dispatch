using System.Windows.Controls;
using System.Linq;
using System.Windows;
using System.ComponentModel;

namespace SingerDispatch.Panels.Quotes
{
    /// <summary>
    /// Interaction logic for QuotesPanel.xaml
    /// </summary>
    public partial class QuotesPanel : QuoteUserControl
    {
        private SingerDispatchDataContext database;

        public QuotesPanel()
        {
            InitializeComponent();

            database = SingerConstants.CommonDataContext;
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
      
        private void btnDiscardChanges_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to discard all changes made to this quote?", "Discard confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;
            
            SelectedQuote = null;            
            Tabs.SelectedIndex = 0;
        }

        private void btnCommitChanges_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedQuote == null) return;

            // If this is a brand new quote, generate a quote number for it
            if (SelectedQuote.Number == 0)
            {
                try
                {
                    SelectedQuote.Number = (from q in database.Quotes select q.Number).Max() + 1;
                }
                catch
                {
                    SelectedQuote.Number = 1;
                }

                SelectedQuote.Revision = 0;
            }

            if (SelectedQuote.ID == 0)
            {
                if (SelectedQuote.Revision > 0)
                {
                    SelectedQuote.Revision = (from q in database.Quotes where q.Number == SelectedQuote.Number select q.Revision).Max() + 1;
                }

                database.Quotes.InsertOnSubmit(SelectedQuote);
            }
                 
            database.SubmitChanges();
        }
    }
}
