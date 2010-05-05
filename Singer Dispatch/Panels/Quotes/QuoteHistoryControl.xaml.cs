using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using SingerDispatch.Database;
using SingerDispatch.Windows;
using System.Windows.Data;

namespace SingerDispatch.Panels.Quotes
{
    /// <summary>
    /// Interaction logic for QuoteHistoryControl.xaml
    /// </summary>
    public partial class QuoteHistoryControl
    {
        private Style _previousStyle;

        public SingerDispatchDataContext Database { get; set; }

        public QuoteHistoryControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            // refresh database lists in case they have been modified elsewhere.
            cmbQuotedBy.ItemsSource = from emp in Database.Employees orderby emp.FirstName, emp.LastName select emp;
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
            CalculateItemizedCost();
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
            CalculateItemizedCost();
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
            SelectedCompany.Quotes.Add(quote);

            try
            {
                EntityHelper.SaveAsNewQuote(quote, Database);

                txtPrice.Focus();
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void CreateRevision_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedQuote == null) return;

            var quote = SelectedQuote.Duplicate();
            var list = (ObservableCollection<Quote>)dgQuotes.ItemsSource;

            list.Insert(0, quote);
            dgQuotes.SelectedItem = quote;
            dgQuotes.ScrollIntoView(quote);
            SelectedCompany.Quotes.Add(quote);

            try
            {
                EntityHelper.SaveAsQuoteRevision(quote, Database);
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void ViewQuote_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedQuote == null) return;

            var title = String.Format("Quote #{0}-{1}", SelectedQuote.Number, SelectedQuote.Revision);

            var viewer = new Windows.DocumentViewerWindow();
            viewer.DisplayPrintout(title, SelectedQuote);
        }

        private void CareOfCompanies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshAddressesAndContacts();
        }

        private void CreateJob_Click(object sender, RoutedEventArgs e)
        {
            var quote = (Quote)dgQuotes.SelectedItem;

            if (quote == null) return;

            var confirmation = MessageBox.Show("Are you sure you wish to create a new job from the selected quote? Doing so will save any changes made to this quote.", "New job confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

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
                EntityHelper.SaveAsNewJob(job, Database);

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
                var addressQuery = from a in Database.Addresses where a.Company == SelectedQuote.Company || a.Company == SelectedQuote.CareOfCompany orderby a.AddressType.Name select a;

                cmbAddresses.ItemsSource = addressQuery.ToList();
                cmbContacts.ItemsSource = (from c in Database.Contacts where addressQuery.Contains(c.Address) orderby c.FirstName, c.LastName select c).ToList();
            }
            else
            {
                cmbAddresses.ItemsSource = null;
                cmbContacts.ItemsSource = null;
            }
        }

        private void DeleteQuote_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedQuote == null) return;

            var confirmation = MessageBox.Show("Are you sure you wish to delete this quote and all of it's items?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            try
            {
                EntityHelper.PrepareEntityDelete(SelectedQuote, Database);
                                
                SelectedCompany.Quotes.Remove(SelectedQuote);
                Database.SubmitChanges();

                ((ObservableCollection<Quote>)dgQuotes.ItemsSource).Remove(SelectedQuote);
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void ItemizedBilling_Checked(object sender, RoutedEventArgs e)
        {
            if (SelectedQuote == null) return;

            txtPrice.IsReadOnly = true;
            _previousStyle = txtPrice.Style;
            txtPrice.Style = (Style)TryFindResource("ReadOnly");

            CalculateItemizedCost();
        }

        private void ItemizedBilling_Unchecked(object sender, RoutedEventArgs e)
        {
            if (SelectedQuote == null) return;

            txtPrice.IsReadOnly = false;
            txtPrice.Style = _previousStyle;
        }

        private void CalculateItemizedCost()
        {
            if (SelectedQuote == null || SelectedQuote.IsItemizedBilling != true) return;

            SelectedQuote.Price = 0.00m;

            foreach (var item in (from c in SelectedQuote.QuoteCommodities where c.Cost != null select c.Cost))
            {
                SelectedQuote.Price += item;
            }

            foreach (var item in (from c in SelectedQuote.QuoteSupplements where c.CostPerItem != null select c))
            {
                if (item.Quantity != null)
                    SelectedQuote.Price += item.Quantity * item.CostPerItem;
                else
                    SelectedQuote.Price += item.CostPerItem;
            }
        }

        private void dgQuotes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var grid = (DataGrid)sender;

            if (grid.SelectedItem == null) return;

            grid.ScrollIntoView(grid.SelectedItem);
            grid.UpdateLayout();
        }
    }
}
