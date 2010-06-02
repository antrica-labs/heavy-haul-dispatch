using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using SingerDispatch.Printing;
using Microsoft.Win32;
using mshtml;
using System;
using System.Diagnostics;
using SingerDispatch.Printing.Documents;

namespace SingerDispatch.Windows
{
    /// <summary>
    /// Interaction logic for DocumentViewerWindow.xaml
    /// </summary>
    public partial class DocumentViewerWindow
    {
        private IPrintDocument Document { get; set; }
        private object OriginalEntity { get; set; }
        private string Filename { get; set; }
       
        public DocumentViewerWindow(IPrintDocument document, object entity)
        {
            InitializeComponent();

            Document = document;
            OriginalEntity = entity;
            Filename = "";
        }

        public DocumentViewerWindow(IPrintDocument document, object entity, string filename)
        {
            InitializeComponent();

            Document = document;
            OriginalEntity = entity;
            Filename = filename;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Document == null) return;

            TheBrowser.NavigateToString(Document.GenerateHTML(OriginalEntity, IsMetricCB.IsChecked == true));
        }

        public void DisplayPrintout()
        {
            ShowDialog();
        }

        private void ApplyMetric_Checked(object sender, RoutedEventArgs e)
        {
            if (Document == null) return;
                        
            TheBrowser.NavigateToString(Document.GenerateHTML(OriginalEntity, true));            
        }

        private void ApplyMetric_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Document == null)
            
            TheBrowser.NavigateToString(Document.GenerateHTML(OriginalEntity, false));            
        }

        private void PDF_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();

            dialog.FileName = Filename;
            dialog.DefaultExt = "pdf";
            dialog.Filter = "PDF documents (.pdf)|*.pdf";

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                var outputFile = dialog.FileName;

                new PdfCreationWindow(outputFile, Document.GenerateHTML(OriginalEntity, IsMetricCB.IsChecked == true)).Run();
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Problem saving to PDF", ex.ToString());
            }
        }        
    }
}
