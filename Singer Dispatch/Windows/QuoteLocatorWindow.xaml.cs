using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SingerDispatch.Windows
{
    /// <summary>
    /// Interaction logic for QuoteLocatorWindow.xaml
    /// </summary>
    public partial class QuoteLocatorWindow : Window
    {
        private Quote LocatedQuote { get; set; }

        public QuoteLocatorWindow()
        {
            InitializeComponent();

            LocatedQuote = null;
        }

        public Quote GetQuote()
        {
            ShowDialog();

            return LocatedQuote;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
            else if (e.Key == Key.Enter)
            {
                FindQuote();
            }
        }

        private void FindQuote_Click(object sender, RoutedEventArgs e)
        {
            FindQuote();
        }

        private void txtQuoteNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtQuoteNumber.Background = Brushes.White;
            txtQuoteNumber.Foreground = Brushes.Black;
        }

        private void FindQuote()
        {
            try
            {
                LocatedQuote = QuoteLookup(Convert.ToInt32(txtQuoteNumber.Text));

                if (LocatedQuote == null)
                {
                    throw new Exception("Could not find quote");
                }

                Close();
            }
            catch
            {
                txtQuoteNumber.Background = Brushes.OrangeRed;
                txtQuoteNumber.Foreground = Brushes.White;
            }
        }

        private Quote QuoteLookup(int number)
        {
            try
            {
                return (from q in SingerConstants.CommonDataContext.Quotes where q.Number == number select q).First();
            }
            catch
            {
                return null;
            }
        }

        
    }
}
