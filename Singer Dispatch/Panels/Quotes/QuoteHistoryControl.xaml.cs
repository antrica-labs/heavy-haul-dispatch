using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Collections;
using System.Collections.ObjectModel;

namespace SingerDispatch.Panels.Quotes
{
    /// <summary>
    /// Interaction logic for QuoteHistoryControl.xaml
    /// </summary>
    public partial class QuoteHistoryControl : QuoteUserControl
    {
        SingerDispatchDataContext database;

        public static DependencyProperty SelectedCompanyProperty = DependencyProperty.Register("SelectedCompany", typeof(Company), typeof(QuoteHistoryControl), new PropertyMetadata(null, QuoteHistoryControl.SelectedCompanyPropertyChanged));

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

        public static void SelectedCompanyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            QuoteHistoryControl control = (QuoteHistoryControl)d;

            control.SelectedCompanyChanged((Company)e.NewValue, (Company)e.OldValue);
        }

        public QuoteHistoryControl()
        {
            InitializeComponent();

            database = SingerConstants.CommonDataContext;
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            cmbQuotedBy.ItemsSource = (from u in database.Users select u).ToList();
        }

        protected override void SelectedQuoteChanged(Quote newValue, Quote oldValue)
        {
            base.SelectedQuoteChanged(newValue, oldValue);

            Quote quote = newValue;

            if (quote != null)
            {
                if (quote.ID != 0)
                {
                    ObservableCollection<Quote> quotes = (ObservableCollection<Quote>)dgQuotes.ItemsSource;

                    if (!quotes.Contains(quote))
                    {
                        quotes.Insert(0, quote);
                        dgQuotes.SelectedItem = quote;
                    }

                    quote = (Quote)quote.Clone();
                }

                FrameworkElement parent = (FrameworkElement)this.Parent;

                while (parent != null && !(parent is QuotesPanel))
                {
                    parent = (FrameworkElement)parent.Parent;
                }

                if (parent != null)
                {
                    QuotesPanel panel = (QuotesPanel)parent;
                    panel.SelectedQuote = quote;
                }
            }

            panelQuoteDetails.DataContext = quote;
        }

        protected void SelectedCompanyChanged(Company newValue, Company oldValue)
        {   
            if (newValue != null)
            {                
                dgQuotes.ItemsSource = new ObservableCollection<Quote>((from q in database.Quotes where q.CompanyID == newValue.ID orderby q.Number descending, q.Revision descending select q).ToList());
                cmbCareOfCompanies.ItemsSource = (from c in database.Companies where c.ID != newValue.ID select c).ToList();                
                dgQuoteContacts.ItemsSource = (from c in database.Contacts where c.Address.Company == newValue orderby c.LastName, c.FirstName select c).ToList();
            }
        }   
               
        private void btnNewQuote_Click(object sender, RoutedEventArgs e)
        {
            Quote quote = new Quote() { CompanyID = SelectedCompany.ID, Number = -1, Revision = -1 };

            quote.CreationDate = DateTime.Today;
            quote.ExpirationDate = DateTime.Today.AddDays(1);
                        
            panelQuoteDetails.DataContext = quote;
            ((ObservableCollection<Quote>)dgQuotes.ItemsSource).Insert(0, quote);
                        
            dgQuotes.SelectedItem = quote;
        }

        private void btnCreateQuote_Click(object sender, RoutedEventArgs e)
        {
            Quote quote = (Quote)panelQuoteDetails.DataContext;

            if (quote != null)
            {
                ((ObservableCollection<Quote>)dgQuotes.ItemsSource).Add(quote);
            }
        }

        private void cmbCareOfCompanies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {            
            Company careOf = (Company)cmbCareOfCompanies.SelectedItem;
            
            if (careOf != null && careOf != SelectedCompany)
            {
                dgQuoteContacts.ItemsSource = (from c in database.Contacts where c.Address.Company == careOf || c.Address.Company == SelectedCompany orderby c.Address.Company.Name, c.LastName, c.FirstName select c).ToList();
            }
            else
            {
                dgQuoteContacts.ItemsSource = (from c in database.Contacts where c.Address.Company == SelectedCompany orderby c.LastName, c.FirstName select c).ToList();
            }            
        }

        private void btnCreateJob_Click(object sender, RoutedEventArgs e)
        {
            Quote quote = (Quote)dgQuotes.SelectedItem;

            if (quote != null)
            {
                MessageBoxResult confirmation = MessageBox.Show("Are you sure you wish to create a new job from the selected quote?", "New job confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            }
        }

        private void dpCreationDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void dpExpirationDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
        }        
    }
}
