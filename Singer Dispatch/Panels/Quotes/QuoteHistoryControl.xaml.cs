using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;

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

        public QuoteHistoryControl()
        {
            InitializeComponent();

            database = SingerConstants.CommonDataContext;
            cmbQuotedBy.ItemsSource = (from u in database.Users select u).ToList();
        }

        public static void SelectedCompanyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            QuoteHistoryControl control = (QuoteHistoryControl)d;

            control.SelectedCompanyChanged((Company)e.NewValue, (Company)e.OldValue);
        }

        protected override void SelectedQuoteChanged(Quote newValue, Quote oldValue)
        {
            base.SelectedQuoteChanged(newValue, oldValue);

            Quote quote = newValue;
            ObservableCollection<Quote> quotes = (ObservableCollection<Quote>)dgQuotes.ItemsSource;

            if (quote != null)
            {
                if (quote.ID != 0)
                {
                    if (!quotes.Contains(quote))
                    {
                        quotes.Insert(0, quote);
                        dgQuotes.SelectedItem = quote;
                    }                    
                }

                BubbleUpQuote(quote);
            }
            else
            {
                // Remove any unsaved quotes (user wants them discarded) 
                DiscardUnsavedQuotes();          
            }

            panelQuoteDetails.DataContext = quote;
            UpdateContactList();
        }

        private void BubbleUpQuote(Quote quote)
        {
            // Updated the Selected Quote of any parent controls that may have the dependency property
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

        private void DiscardUnsavedQuotes()
        {
            ObservableCollection<Quote> quotes = (ObservableCollection<Quote>)dgQuotes.ItemsSource;

            foreach (Quote q in quotes.ToList())
            {
                if (q.ID == 0)
                {
                    quotes.Remove(q);
                }
            }
        }

        protected void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            if (newValue != null)
            {
                dgQuotes.ItemsSource = new ObservableCollection<Quote>((from q in database.Quotes where q.CompanyID == newValue.ID orderby q.Number descending, q.Revision descending select q).ToList());
                cmbCareOfCompanies.ItemsSource = (from c in database.Companies where c.ID != newValue.ID select c).ToList();
            }
            else
            {
                ((ObservableCollection<Quote>)dgQuotes.ItemsSource).Clear();
                ((List<Company>)cmbCareOfCompanies.ItemsSource).Clear();
            }

            UpdateContactList();
        }   
               
        private void btnNewQuote_Click(object sender, RoutedEventArgs e)
        {
            Quote quote = new Quote() { CompanyID = SelectedCompany.ID, Number = 0, Revision = 0 };

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
            UpdateContactList();           
        }

        private void btnCreateJob_Click(object sender, RoutedEventArgs e)
        {
            Quote quote = (Quote)dgQuotes.SelectedItem;

            if (quote != null)
            {
                MessageBoxResult confirmation = MessageBox.Show("Are you sure you wish to create a new job from the selected quote?", "New job confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            }
        }

        private void UpdateContactList()
        {
            List<Contact> contacts;

            if (SelectedQuote == null)
            {
                contacts = new List<Contact>();
            }
            else if (SelectedQuote.CareOfCompanyID != null)
            {
                contacts = (from c in database.Contacts where c.Address.CompanyID == SelectedCompany.ID || c.Address.CompanyID == SelectedQuote.CareOfCompanyID select c).ToList();
            }
            else
            {
                contacts = (from c in database.Contacts where c.Address.CompanyID == SelectedCompany.ID select c).ToList();
            }
            
            dgQuoteContacts.ItemsSource = contacts;
        }

        private void dpCreationDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void dpExpirationDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void btnCreateRevisoin_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedQuote == null)
            {
                return;
            }

            Quote quote = (Quote)SelectedQuote.Clone();
            ObservableCollection<Quote> quotes = (ObservableCollection<Quote>)dgQuotes.ItemsSource;

            quote.Revision = (from q in database.Quotes where q.Number == SelectedQuote.Number select q.Revision).Max() + 1;

            quotes.Insert(0, quote);
            dgQuotes.SelectedItem = quote;
        }

        private void btnPrintQuote_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedQuote == null)
            {
                return;
            }

            SelectedQuote.IsPrinted = 1;

            database.SubmitChanges();
        }        
    }
}
