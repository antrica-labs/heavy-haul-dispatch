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
using System.Windows.Shapes;

namespace SingerDispatch.Windows
{
    /// <summary>
    /// Interaction logic for InvoiceLocatorWindow.xaml
    /// </summary>
    public partial class InvoiceLocatorWindow : Window
    {
        private Invoice LocatedInvoice;

        public InvoiceLocatorWindow()
        {
            InitializeComponent();

            LocatedInvoice = null;
        }

        public Invoice GetInvoice()
        {
            ShowDialog();

            return LocatedInvoice;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Close();
                    break;
                case Key.Enter:
                    FindInvoice();
                    break;
            }
        }

        private void FindInvoice_Click(object sender, RoutedEventArgs e)
        {
            FindInvoice();
        }

        private void txtInvoiceNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtInvoiceNumber.Background = Brushes.White;
            txtInvoiceNumber.Foreground = Brushes.Black;
        }

        private void FindInvoice()
        {
            try
            {
                LocatedInvoice = InvoiceLookup(Convert.ToInt32(txtInvoiceNumber.Text));

                if (LocatedInvoice == null)
                {
                    throw new Exception("Could not find invoice");
                }

                Close();
            }
            catch
            {
                txtInvoiceNumber.Background = Brushes.OrangeRed;
                txtInvoiceNumber.Foreground = Brushes.White;
            }

        }

        private Invoice InvoiceLookup(Int32 number)
        {
            try
            {
                return (from j in SingerConfigs.CommonDataContext.Invoices where j.Number == number select j).First();
            }
            catch
            {
                return null;
            }
        }
    }
}
