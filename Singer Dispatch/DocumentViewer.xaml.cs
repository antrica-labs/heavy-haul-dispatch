using System.Windows;
using SingerDispatch.Printing;
//using mshtml;
//using Microsoft.Win32;

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

            TheBrowser.NavigateToString(renderer.GeneratePrintout(quote));

            ShowDialog();
        }

        public void DisplayDispatchPrintout()
        {
            var renderer = new DispatchRenderer();

            TheBrowser.NavigateToString(renderer.GeneratePrintout());

            ShowDialog();
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        { }

        /*
        private void Print_Click(object sender, RoutedEventArgs e)
        {
            IHTMLDocument2 document = TheBrowser.Document as IHTMLDocument2;
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
        */
    }
}
