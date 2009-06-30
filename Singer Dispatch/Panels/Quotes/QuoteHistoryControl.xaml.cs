using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SingerDispatch.Panels.Companies;

namespace SingerDispatch.Panels.Quotes
{
    /// <summary>
    /// Interaction logic for QuoteHistoryControl.xaml
    /// </summary>
    public partial class QuoteHistoryControl : CompanyUserControl
    {
        SingerDispatchDataContext database;

        public QuoteHistoryControl()
        {
            InitializeComponent();

            database = SingerConstants.CommonDataContext;
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {            
            dgQuoteContacts.ItemsSource = (from c in database.Contacts select c).ToList();
        }

        private void EmailClick(object sender, RequestNavigateEventArgs e)
        {           
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            if (newValue != null)
            {
                dgQuotes.ItemsSource = newValue.Quotes;
                cmbCareOfCompanies.ItemsSource = (from c in database.Companies where c.ID != newValue.ID select c).ToList();                
                dgQuoteContacts.ItemsSource = (from c in database.Contacts where c.Address.Company == newValue orderby c.LastName, c.FirstName select c).ToList();
            }
        }

        private void btnCommitQuoteChanges_Click(object sender, RoutedEventArgs e)
        {
            Quote quote = (Quote)panelQuoteDetails.DataContext;

            if (quote != null && quote.ID == 0)
            {
                database.Quotes.InsertOnSubmit(quote);
            }

            database.SubmitChanges();
        }

        private void btnCreateQuote_Click(object sender, RoutedEventArgs e)
        {
            Quote quote = new Quote() { CompanyID = SelectedCompany.ID };
                        
            quote.CreationDate = DateTime.Now;
            quote.ExpirationDate = DateTime.Now.AddDays(30);

            panelQuoteDetails.DataContext = quote;
        }

        private void dgQuotes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FrameworkElement parent = (FrameworkElement)this.Parent;

            while (parent != null && !(parent is QuotesPanel))
            {
                parent = (FrameworkElement)parent.Parent;
            }


            if (parent != null)
            {
                QuotesPanel panel = (QuotesPanel)parent;
                panel.SelectedQuote = (Quote)dgQuotes.SelectedItem;
            }

            panelQuoteDetails.DataContext = dgQuotes.SelectedItem;        
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

        
    }
}
