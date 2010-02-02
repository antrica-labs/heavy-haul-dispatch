using System.Windows;
using SingerDispatch.Printing;
//using mshtml;
//using Microsoft.Win32;

namespace SingerDispatch.Windows
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

        public void DisplayPrintout(object obj)
        {
            Renderer renderer;

            if (obj is Quote)
                renderer = new QuoteRenderer();
            else if (obj is Invoice)
                renderer = new InvoiceRenderer();
            else if (obj is Dispatch)
                renderer = new DispatchRenderer();
            else
                return;

            TheBrowser.NavigateToString(renderer.GenerateHTML(obj));

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
