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
        private string Filename { get; set; }
        private string SourceHTML { get; set; }
        
        public DocumentViewerWindow()
        {
            InitializeComponent();

            Filename = "";
        }

        public void DisplayPrintout(object obj)
        {
            DisplayPrintout("", obj);
        }

        public void DisplayPrintout(string filename, object obj)
        {
            IRenderer renderer;

            if (obj is Quote)
                renderer = new QuoteRenderer();
            else if (obj is Invoice)
                renderer = new InvoiceRenderer();
            else if (obj is Dispatch || obj is List<Dispatch>)
            {
                var result = MessageBox.Show("Do you wish to inlcude a driver's copy with this printout?", "Include drivers copy?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                renderer = new DispatchRenderer(result == MessageBoxResult.Yes);
            }
            else
                return;

            Filename = filename;
            SourceHTML = renderer.GenerateHTML(obj);

            TheBrowser.NavigateToString(SourceHTML);

            ShowDialog();
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

                new PdfCreationWindow(outputFile, SourceHTML).Run();
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Problem saving to PDF", ex.ToString());
            }
        }
    }
}
