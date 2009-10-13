using System.Windows.Controls;
using System.Linq;
using SingerDispatch.Panels.Companies;
using System.Windows;

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
        }

        protected override void SelectedQuoteChanged(Quote newValue, Quote oldValue)
        {
            base.SelectedQuoteChanged(newValue, oldValue);
        
            if (newValue == null)
            {
                tabSupplements.IsEnabled = false;
                ((UserControl)tabSupplements.Content).IsEnabled = false;

                tabCommodities.IsEnabled = false;
                ((UserControl)tabCommodities.Content).IsEnabled = false;

                btnCommitChanges.IsEnabled = false;
                btnDiscardChanges.IsEnabled = false;
            }
            else
            {
                tabSupplements.IsEnabled = true;
                ((UserControl)tabSupplements.Content).IsEnabled = true;

                tabCommodities.IsEnabled = true;
                ((UserControl)tabCommodities.Content).IsEnabled = true;

                btnCommitChanges.IsEnabled = true;
                btnDiscardChanges.IsEnabled = true;
            }
        }
      
        private void btnDiscardChanges_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to discard all changes made to this quote?", "Discard confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;
            
            SelectedQuote = null;
            panelQuoteHistory.SelectedQuote = null;
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

            panelQuoteHistory.SelectedQuote = null;
            panelQuoteHistory.SelectedQuote = SelectedQuote;
        }
    }
}
