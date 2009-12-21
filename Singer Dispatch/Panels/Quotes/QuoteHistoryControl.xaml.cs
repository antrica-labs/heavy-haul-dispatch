using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using SingerDispatch.Database;

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

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            // refresh database lists in case they have been modified elsewhere.
            cmbQuotedBy.ItemsSource = from emp in Database.Employees select emp;

            cmbCareOfCompanies.ItemsSource = (SelectedCompany == null) ? null : from c in Database.Companies where c != SelectedCompany select c;
            dgQuotes.ItemsSource = (SelectedCompany == null) ? null : new ObservableCollection<Quote>(from quote in Database.Quotes where quote.Company == SelectedCompany orderby quote.Number descending, quote.Revision descending select quote);            
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            cmbCareOfCompanies.ItemsSource = (SelectedCompany == null) ? null : from c in Database.Companies where c != SelectedCompany select c;
            dgQuotes.ItemsSource = (SelectedCompany == null) ? null : new ObservableCollection<Quote>(from quote in Database.Quotes where quote.Company == SelectedCompany orderby quote.Number descending, quote.Revision descending select quote);            
        }

        protected override void SelectedQuoteChanged(Quote newValue, Quote oldValue)
        {
            base.SelectedQuoteChanged(newValue, oldValue);         

            BubbleUpQuote(newValue);
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

            foreach (Quote q in quotes)
            {
                if (q.ID == 0)
                {
                    quotes.Remove(q);
                }
            }
        }
               
        private void NewQuote_Click(object sender, RoutedEventArgs e)
        {
            var quote = new Quote { Company = SelectedCompany, Number = 0, Revision = 0, CreationDate = DateTime.Today, ExpirationDate = DateTime.Today.AddDays(30) };
                                    
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

            var viewer = new SingerDispatch.Windows.DocumentViewer();
            viewer.DisplayQuotePrintout(SelectedQuote);
        }

        private void DeleteQuote_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedQuote == null)
            {
                return;
            }

            var quote = SelectedQuote;

            BubbleUpQuote(null);

            ((ObservableCollection<Quote>)dgQuotes.ItemsSource).Remove(quote);
            SelectedCompany.Quotes.Remove(quote);

            EntityHelper.PrepareEntityDelete(quote, Database);            
            
            Database.SubmitChanges();
        }
    }
}
