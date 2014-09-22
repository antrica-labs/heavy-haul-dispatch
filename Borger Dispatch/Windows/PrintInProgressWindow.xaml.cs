using System;
using System.Windows;
using System.ComponentModel;
using mshtml;
using System.Windows.Threading;
using Microsoft.Win32;
using System.Threading;

namespace SingerDispatch.Windows
{
    /// <summary>
    /// Interaction logic for PrintInProgressWindow.xaml
    /// </summary>
    public partial class PrintInProgressWindow : Window
    {
        private BackgroundWorker Backgrounder;
        private readonly IHTMLDocument2 Document;        

        public PrintInProgressWindow(IHTMLDocument2 document)
        {
            InitializeComponent();

            Backgrounder = new BackgroundWorker();
            Document = document;
        }

        public void Run()
        {
            Show();

            Backgrounder.DoWork += PrintDocument;
            Backgrounder.RunWorkerCompleted += CompletedDocumentPrint;

            Backgrounder.RunWorkerAsync();
        }


        private void PrintDocument(object sender, DoWorkEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(UpdateStatus), "Preparing document for printing...");

            try
            {
                var psKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\MICROSOFT\\Internet Explorer\\PageSetup");

                var font = psKey.GetValue("font");
                var header = psKey.GetValue("header");
                var footer = psKey.GetValue("footer");
                var mTop = psKey.GetValue("margin_top");
                var mBottom = psKey.GetValue("margin_bottom");
                var mLeft = psKey.GetValue("margin_left");
                var mRight = psKey.GetValue("margin_right");
                var printBackground = psKey.GetValue("Print_Background");
                var srinkToFit = psKey.GetValue("Shrink_To_Fit");

                psKey.SetValue("font", "");
                psKey.SetValue("header", "");
                psKey.SetValue("footer", "");
                psKey.SetValue("margin_top", "0.39370");
                psKey.SetValue("margin_bottom", "0.39370");
                psKey.SetValue("margin_left", "0.39370");
                psKey.SetValue("margin_right", "0.39370");
                psKey.SetValue("Print_Background", "yes");
                psKey.SetValue("Shrink_To_Fit", "yes");

                Document.execCommand("Print", true, null);

                // The registry values actually need to be set back to way they original values AFTER the print job is done... not sure how to tell when printing is done though.

                Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(UpdateStatus), "Document sent to printer...");
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(UpdateStatus), string.Format("Print error: {0}", ex.Message));

                Thread.Sleep(3000);
            }
        }

        private void CompletedDocumentPrint(object sender, RunWorkerCompletedEventArgs e)
        {
            Close();
        }

        private void UpdateStatus(string status)
        {
            lblStatusOutput.Content = status;
        }
    }
}
