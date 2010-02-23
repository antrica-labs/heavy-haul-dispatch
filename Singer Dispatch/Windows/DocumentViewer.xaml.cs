﻿using System.Windows;
using SingerDispatch.Printing;
using Microsoft.Win32;
using mshtml;
using System;
using System.Diagnostics;

namespace SingerDispatch.Windows
{
    /// <summary>
    /// Interaction logic for DocumentViewer.xaml
    /// </summary>
    public partial class DocumentViewer
    {
        private string SourceHTML { get; set; }

        public DocumentViewer()
        {
            InitializeComponent();
        }

        public void DisplayPrintout(object obj)
        {
            IRenderer renderer;

            if (obj is Quote)
                renderer = new QuoteRenderer();
            else if (obj is Invoice)
                renderer = new InvoiceRenderer();
            else if (obj is Dispatch)
                renderer = new DispatchRenderer();
            else
                return;

            SourceHTML = renderer.GenerateHTML(obj);

            TheBrowser.NavigateToString(SourceHTML);

            ShowDialog();
        }

        private void PDF_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            
            dialog.DefaultExt = ".pdf";
            dialog.Filter = "PDF documents (.pdf)|*.pdf"; 

            if (dialog.ShowDialog() != true)
                return;
            
            var pdf = new PDFizer();

            try
            {
                var outputFile = dialog.FileName;

                pdf.SaveHTMLToPDF(SourceHTML, outputFile);

                var process = new Process();
                var shell = new ProcessStartInfo(outputFile);

                shell.UseShellExecute = true;
                shell.WindowStyle = ProcessWindowStyle.Normal;

                process.StartInfo = shell;
                process.Start();
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Problem saving to PDF", ex.Message);
            }                     
        }
        
        private void Print_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IHTMLDocument2 document = TheBrowser.Document as IHTMLDocument2;
                RegistryKey psKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\MICROSOFT\\Internet Explorer\\PageSetup");

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

                document.execCommand("Print", true, null);

                // The registry values actually need to be set back to way they original values AFTER the print job is done... not sure how to tell when it's done yet.
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Printing error", ex.Message);
            }

        }
    }
}
