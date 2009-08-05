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
        public static DependencyProperty SelectedCompanyProperty = DependencyProperty.Register("SelectedCompany", typeof(Company), typeof(QuotesPanel), new PropertyMetadata(null, QuotesPanel.SelectedCompanyPropertyChanged));

        public Company SelectedCompany
        {
            get
            {
                return (Company)GetValue(SelectedCompanyProperty);
            }
            set
            {
                SetValue(SelectedCompanyProperty, value);
            }
        }

        private SingerDispatchDataContext database;

        public QuotesPanel()
        {
            InitializeComponent();

            database = SingerConstants.CommonDataContext;
        }

        public static void SelectedCompanyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            QuotesPanel control = (QuotesPanel)d;
            Company company = (Company)e.NewValue;

            if (company == null)
            {
                control.IsEnabled = false;
            }
            else
            {
                control.IsEnabled = true;
            }
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

            if (confirmation == MessageBoxResult.Yes)
            {
                SelectedQuote = null;
                panelQuoteHistory.SelectedQuote = null;
                tabs.SelectedIndex = 0;
            }
        }

        private void btnCommitChanges_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedQuote != null)
            {
                /*
                MessageBoxResult confirm = MessageBox.Show("Are you sure you wish to commit the changes to this quote and all of its properties?", "Save confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (confirm == MessageBoxResult.No)
                {
                    return;
                }
                */

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
}
