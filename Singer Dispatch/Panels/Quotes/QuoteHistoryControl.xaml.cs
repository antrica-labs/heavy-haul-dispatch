using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using SingerDispatch.Database;
using SingerDispatch.Windows;

namespace SingerDispatch.Panels.Quotes
{
    /// <summary>
    /// Interaction logic for QuoteHistoryControl.xaml
    /// </summary>
    public partial class QuoteHistoryControl
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
            cmbQuotedBy.ItemsSource = from emp in Database.Employees orderby emp.FirstName select emp;
            cmbCareOfCompanies.ItemsSource = (SelectedCompany == null) ? null : from c in Database.Companies where c != SelectedCompany && c.IsVisible == true select c;            

            if (SelectedCompany != null)
            {
                var quotes = new ObservableCollection<Quote>(from quote in Database.Quotes where quote.Company == SelectedCompany orderby quote.Number descending, quote.Revision descending select quote);

                if (SelectedQuote != null && SelectedQuote.ID == 0)
                    quotes.Insert(0, SelectedQuote);

                dgQuotes.ItemsSource = quotes;
            }
            else
            {
                dgQuotes.ItemsSource = null;
            }

            RefreshAddressesAndContacts();
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

            RefreshAddressesAndContacts();
        }
               
        private void NewQuote_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<Quote>)dgQuotes.ItemsSource;
            var quote = new Quote { CreationDate = DateTime.Today, ExpirationDate = DateTime.Today.AddDays(30), Company = SelectedCompany };

            try
            {
                quote.Employee = (from emp in Database.Employees where emp.FirstName == "Dan" && emp.LastName == "Klassen" select emp).First();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }

            list.Insert(0, quote);
            dgQuotes.SelectedItem = quote;
            dgQuotes.ScrollIntoView(quote);

            txtDescription.Focus();
        }

        private void CreateRevision_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedQuote == null) return;

            var quote = SelectedQuote.Duplicate();
            var quotes = (ObservableCollection<Quote>)dgQuotes.ItemsSource;
                        
            quotes.Add(quote);
            dgQuotes.SelectedItem = quote;
            dgQuotes.ScrollIntoView(quote);
        }

        private void CareOfCompanies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshAddressesAndContacts();           
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

            try
            {
                job.JobStatusType = (from s in Database.JobStatusTypes where s.Name == "Pending" select s).First();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }

            SelectedCompany.Jobs.Add(job);
            Database.Jobs.InsertOnSubmit(job);

            try
            {
                Database.SubmitChanges();

                window.ViewJob(job);
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void RefreshAddressesAndContacts()
        {            
            if (SelectedQuote != null)
            {
                var addressQuery = from a in Database.Addresses where a.Company == SelectedQuote.Company || a.Company == SelectedQuote.CareOfCompany select a;

                cmbAddresses.ItemsSource = addressQuery.ToList();
                cmbContacts.ItemsSource = (from c in Database.Contacts where addressQuery.Contains(c.Address) select c).ToList();
            }
            else
            {
                cmbAddresses.ItemsSource = null;
                cmbContacts.ItemsSource = null;
            }
        }
       
        private void ViewQuote_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedQuote == null) return;

            var viewer = new Windows.DocumentViewer();
            viewer.DisplayPrintout(SelectedQuote);
        }

        private void DeleteQuote_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedQuote == null) return;

            try
            {
                EntityHelper.PrepareEntityDelete(SelectedQuote, Database);

                ((ObservableCollection<Quote>)dgQuotes.ItemsSource).Remove(SelectedQuote);
                SelectedCompany.Quotes.Remove(SelectedQuote);

                Database.SubmitChanges();
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }
    }
}
