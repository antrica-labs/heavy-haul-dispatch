using System.Windows.Controls;
using System.Linq;
using SingerDispatch.Panels.Companies;
using System.Windows;

namespace SingerDispatch.Panels.Quotes
{
    /// <summary>
    /// Interaction logic for QuotesPanel.xaml
    /// </summary>
    public partial class QuotesPanel : CompanyUserControl
    {
        public static DependencyProperty SelectedQuoteProperty = DependencyProperty.Register("SelectedQuote", typeof(Quote), typeof(QuotesPanel), new PropertyMetadata(null, QuotesPanel.SelectedQuotePropertyChanged));

        private SingerDispatchDataContext database;

        public QuotesPanel()
        {
            InitializeComponent();

            database = SingerConstants.CommonDataContext;
        }

        public Quote SelectedQuote
        {
            get
            {
                return (Quote)GetValue(SelectedQuoteProperty);
            }
            set
            {
                SetValue(SelectedQuoteProperty, value);
            }
        }

        public static void SelectedQuotePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            QuotesPanel panel = (QuotesPanel)d;
            Quote quote = (Quote)e.NewValue;

            if (quote == null)
            {
                panel.tabSupplements.IsEnabled = false;
                ((UserControl)panel.tabSupplements.Content).IsEnabled = false;

                panel.tabCommodities.IsEnabled = false;
                ((UserControl)panel.tabCommodities.Content).IsEnabled = false;

                panel.btnCommitChanges.IsEnabled = false;
                panel.btnDiscardChanges.IsEnabled = false;
            }
            else
            {
                panel.tabSupplements.IsEnabled = true;
                ((UserControl)panel.tabSupplements.Content).IsEnabled = true;

                panel.tabCommodities.IsEnabled = true;
                ((UserControl)panel.tabCommodities.Content).IsEnabled = true;

                panel.btnCommitChanges.IsEnabled = true;
                panel.btnDiscardChanges.IsEnabled = true;
            }
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);
            
            if (newValue == null)
            {
                this.IsEnabled = false;
            }
            else
            {
                this.IsEnabled = true;
            }
        }

        private void btnDiscardChanges_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCommitChanges_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedQuote != null)
            {
                if (SelectedQuote.Number < 0)
                {
                    try
                    {
                        SelectedQuote.Number = (from q in database.Quotes select q.Number).Max() + 1;
                    }
                    catch
                    {
                        SelectedQuote.Number = 1;
                    }
                }

                SelectedQuote.Revision += 1;

                database.Quotes.InsertOnSubmit(SelectedQuote);
                database.SubmitChanges();

                panelQuoteHistory.SelectedQuote = null;
                panelQuoteHistory.SelectedQuote = SelectedQuote;
            }
        }
    }
}
