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
using System.ComponentModel;

namespace SingerDispatch.Windows
{
    /// <summary>
    /// Interaction logic for DocumentViewerWindow.xaml
    /// </summary>
    public partial class DocumentViewerWindow
    {
        private BackgroundWorker Backgrounder;

        private IPrintDocument Document { get; set; }
        private bool IsMetric { get; set; }
        private object OriginalEntity { get; set; }
        private string Filename { get; set; }
       
        public DocumentViewerWindow(IPrintDocument document, object entity)
        {
            InitializeComponent();

            Backgrounder = new BackgroundWorker();

            IsMetric = true;
            Document = document;
            OriginalEntity = entity;
            Filename = "";
        }

        public DocumentViewerWindow(IPrintDocument document, object entity, string filename)
        {
            InitializeComponent();

            Backgrounder = new BackgroundWorker();

            IsMetric = true;
            Document = document;
            OriginalEntity = entity;
            Filename = filename;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Document == null) return;

            var loading = new LoadingDocument();
            
            // Spawn a new thread to render the requested document and then display it when ready.
            Backgrounder.DoWork += RenderDocument;
            Backgrounder.RunWorkerCompleted += DisplayDocument;
            
            TheBrowser.NavigateToString(loading.GenerateHTML(null));

            Backgrounder.RunWorkerAsync();
        }

        public void DisplayPrintout()
        {
            ShowDialog();
        }

        private void ApplyMetric_Checked(object sender, RoutedEventArgs e)
        {
            if (Document == null) return;

            IsMetric = true;

            Backgrounder.RunWorkerAsync();
        }

        private void ApplyMetric_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Document == null) return;

            IsMetric = false;

            Backgrounder.RunWorkerAsync();
        }

        private void RenderDocument(object sender, DoWorkEventArgs e)
        {            
            string html;

            Document.PrintMetric = IsMetric;
            html = Document.GenerateHTML(OriginalEntity);

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(ShowHTML), html);
        }

        private void DisplayDocument(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        private void ShowHTML(string html)
        {
            TheBrowser.NavigateToString(html);
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

                Document.PrintMetric = IsMetricCB.IsChecked == true;

                new PdfCreationWindow(outputFile, Document.GenerateHTML(OriginalEntity)).Run();
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Problem saving to PDF", ex.ToString());
            }
        }        
    }
}
