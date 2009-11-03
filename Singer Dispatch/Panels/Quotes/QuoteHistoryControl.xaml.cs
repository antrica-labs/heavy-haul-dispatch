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
        public SingerDispatchDataContext Database { get; set; }

        public QuoteHistoryControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;            
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            if (newValue != null)
            {
                dgQuotes.ItemsSource = new ObservableCollection<Quote>((from q in Database.Quotes where q.CompanyID == newValue.ID orderby q.Number descending, q.Revision descending select q).ToList());
                cmbCareOfCompanies.ItemsSource = (from c in Database.Companies where c.ID != newValue.ID select c).ToList();
            }
            else
            {
                ((ObservableCollection<Quote>)dgQuotes.ItemsSource).Clear();
                ((List<Company>)cmbCareOfCompanies.ItemsSource).Clear();
            }

            UpdateContactList();
        }

        protected override void SelectedQuoteChanged(Quote newValue, Quote oldValue)
        {
            base.SelectedQuoteChanged(newValue, oldValue);
            
            var quotes = (ObservableCollection<Quote>)dgQuotes.ItemsSource;

            if (newValue != null)
            {
                if (newValue.ID != 0)
                {
                    if (!quotes.Contains(newValue))
                    {
                        quotes.Insert(0, newValue);
                        dgQuotes.SelectedItem = newValue;
                    }                    
                }

                BubbleUpQuote(newValue);
            }
            else
            {
                // Remove any unsaved quotes (user wants them discarded) 
                DiscardUnsavedQuotes();          
            }
                        
            UpdateContactList();
        }

        private void BubbleUpQuote(Quote quote)
        {
            // Updated the Selected Quote of any parent controls that may have the dependency property
            var parent = (FrameworkElement)Parent;

            while (parent != null && !(parent is QuotesPanel))
            {
                parent = (FrameworkElement)parent.Parent;
            }

            if (parent == null) return;

            var panel = (QuotesPanel)parent;
            panel.SelectedQuote = quote;
        }

        private void DiscardUnsavedQuotes()
        {
            var quotes = (ObservableCollection<Quote>)dgQuotes.ItemsSource;

            foreach (Quote q in quotes.ToList())
            {
                if (q.ID == 0)
                {
                    quotes.Remove(q);
                }
            }
        }
               
        private void NewQuote_Click(object sender, RoutedEventArgs e)
        {
            var quote = new Quote { CompanyID = SelectedCompany.ID, Number = 0, Revision = 0, CreationDate = DateTime.Today, ExpirationDate = DateTime.Today.AddDays(1) };
                                    
            ((ObservableCollection<Quote>)dgQuotes.ItemsSource).Insert(0, quote);                        
            dgQuotes.SelectedItem = quote;

            txtPrice.Focus();
        }

        private void CareOfCompanies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateContactList();           
        }

        private void CreateJob_Click(object sender, RoutedEventArgs e)
        {
            var quote = (Quote)dgQuotes.SelectedItem;

            if (quote == null) return;

            MessageBoxResult confirmation = MessageBox.Show("Are you sure you wish to create a new job from the selected quote? Doing so will save any changes made to this quote.", "New job confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes)
            {
                return;
            }
            
            var window = (MainWindow)Application.Current.MainWindow;
            var job = quote.ToJob();

            SelectedCompany.Jobs.Add(job);
            Database.Jobs.InsertOnSubmit(job);
            Database.SubmitChanges();

            window.ViewJob(job);
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
                contacts = (from c in Database.Contacts where c.Address.CompanyID == SelectedCompany.ID || c.Address.CompanyID == SelectedQuote.CareOfCompanyID select c).ToList();
            }
            else
            {
                contacts = (from c in Database.Contacts where c.Address.CompanyID == SelectedCompany.ID select c).ToList();
            }
            
            dgQuoteContacts.ItemsSource = contacts;
        }

        private void CreateRevision_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedQuote == null) return;

            var quote = (Quote)SelectedQuote.Clone();
            var quotes = (ObservableCollection<Quote>)dgQuotes.ItemsSource;

            quote.Revision = (from q in Database.Quotes where q.Number == SelectedQuote.Number select q.Revision).Max() + 1;

            quotes.Insert(0, quote);
            dgQuotes.SelectedItem = quote;
        }

        private void PrintQuote_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedQuote == null) return;

            var viewer = new DocumentViewer();
            viewer.DisplayQuotePrintout(SelectedQuote);
        }        
    }
}
