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
using SingerDispatch.Printing;
using System.Runtime.InteropServices;
using mshtml;
using Microsoft.Win32;

namespace SingerDispatch
{
    /// <summary>
    /// Interaction logic for DocumentViewer.xaml
    /// </summary>
    public partial class DocumentViewer
    {
        public DocumentViewer()
        {
            InitializeComponent();
        }

        public void DisplayQuotePrintout(Quote quote)
        {
            var renderer = new QuoteRenderer();

            Browser.NavigateToString(renderer.GeneratePrintout(quote));

            Show();
        }

        public void DisplayDispatchPrintout()
        {
            var renderer = new DispatchRenderer();

            Browser.NavigateToString(renderer.GeneratePrintout());

            Show();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            IHTMLDocument2 document = Browser.Document as IHTMLDocument2;
            RegistryKey psKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\MICROSOFT\\Internet Explorer\\PageSetup");

            var header = psKey.GetValue("header");
            var footer = psKey.GetValue("footer");
            
            psKey.SetValue("header", "");
            psKey.SetValue("footer", "");

            document.execCommand("Print", true, null);

            // These actually need to be set back the way they were AFTER the execCommand is done... wont work this way.
            psKey.SetValue("header", header);
            psKey.SetValue("footer", footer);
        }
    }
}
