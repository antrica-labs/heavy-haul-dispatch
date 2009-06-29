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
            //dpCreationDate.SelectedDate = DateTime.Now;            
            //dpExpirationDate.SelectedDate = DateTime.Now.AddDays(30);           

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
            }
        }

        private void btnCommitQuoteChanges_Click(object sender, RoutedEventArgs e)
        {
            SingerConstants.CommonDataContext.SubmitChanges();
        }
    }
}
