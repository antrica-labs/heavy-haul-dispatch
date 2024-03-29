﻿using System.Collections.Generic;
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
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Interop;
using SingerDispatch.Printing.Excel;
using CefSharp;

namespace SingerDispatch.Windows
{
    /// <summary>
    /// Interaction logic for DocumentViewerWindow.xaml
    /// </summary>
    public partial class DocumentViewerWindow
    {
        private bool HasLoaded = false;

        private bool _isMetric = true;
        public bool IsMetric 
        {
            get
            {
                return _isMetric;
            }
            set
            {
                cmbDisplayUnits.SelectedIndex = value ? 0 : 1;
                _isMetric = value;
            }
        }
        
        private bool _isSpecializedDocument = true;        
        public bool IsSpecializedDocument 
        { 
            get
            {
                return _isSpecializedDocument;
            }

            set
            {
                cmbCompanyType.SelectedIndex = value ? 0 : 1;                
                _isSpecializedDocument = value;
            }
        }

        private UserSettings Settings { get; set; }
        private BackgroundWorker Backgrounder;
        private IPrintDocument Document { get; set; }
        private IExcelDocument Excel { get; set; }
        private object OriginalEntity { get; set; }

        private string _filename;
        private string Filename 
        {
            get
            {
                return _filename;
            }
            set
            {
                _filename = MakeValidFileName(value);
            }
        }
       

        public DocumentViewerWindow(IPrintDocument document, object entity)
        {
            InitializeComponent();

            Settings = new UserSettings();
            Backgrounder = new BackgroundWorker();

            IsSpecializedDocument = true;
            IsMetric = true;
            Document = document;
            Excel = null;
            OriginalEntity = entity;
            Filename = "";
        }

        public DocumentViewerWindow(IPrintDocument document, object entity, string filename)
        {
            InitializeComponent();

            Settings = new UserSettings();
            Backgrounder = new BackgroundWorker();

            IsMetric = true;
            IsSpecializedDocument = true;
            Document = document;
            Excel = null;
            OriginalEntity = entity;
            Filename = filename;
        }

        public DocumentViewerWindow(IPrintDocument document, IExcelDocument excel, object entity)
        {
            InitializeComponent();

            Settings = new UserSettings();
            Backgrounder = new BackgroundWorker();

            IsSpecializedDocument = true;
            IsMetric = true;
            Document = document;
            Excel = excel;
            OriginalEntity = entity;
            Filename = "";
        }

        public DocumentViewerWindow(IPrintDocument document, IExcelDocument excel, object entity, string filename)
        {
            InitializeComponent();

            Settings = new UserSettings();
            Backgrounder = new BackgroundWorker();

            IsMetric = true;
            IsSpecializedDocument = true;
            Document = document;
            Excel = excel;
            OriginalEntity = entity;
            Filename = filename;
        }

        public void DisplayPrintout()
        {
            ShowDialog();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            WindowPlacement.SetPlacement(new WindowInteropHelper(this).Handle, Settings.DocumentViewerWindowPlacement);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Document == null) return;
                       
            // Spawn a new thread to render the requested document and then display it when ready.
            Backgrounder.DoWork += RenderDocument;
            Backgrounder.RunWorkerCompleted += DisplayDocument;

            HasLoaded = true;

            Backgrounder.RunWorkerAsync();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Settings.DocumentViewerWindowPlacement = WindowPlacement.GetPlacement(new WindowInteropHelper(this).Handle);
            Settings.Save();
        }

        private void MetricMeasurements_Selected(object sender, RoutedEventArgs e)
        {
            if (!HasLoaded || Document == null) return;

            IsMetric = true;

            Backgrounder.RunWorkerAsync();
        }

        private void ImperialMeasurements_Selected(object sender, RoutedEventArgs e)
        {
            if (!HasLoaded || Document == null) return;

            IsMetric = false;

            Backgrounder.RunWorkerAsync();
        }

        private void SingerSpecialized_Selected(object sender, RoutedEventArgs e)
        {
            if (!HasLoaded || Document == null) return;

            IsSpecializedDocument = true;

            Backgrounder.RunWorkerAsync();
        }

        private void SingerEnterprises_Selected(object sender, RoutedEventArgs e)
        {
            if (!HasLoaded || Document == null) return;

            IsSpecializedDocument = false;

            Backgrounder.RunWorkerAsync();
        }

        private void RenderDocument(object sender, DoWorkEventArgs e)
        {            
            string html;

            Document.PrintMetric = IsMetric;
            Document.SpecializedDocument = IsSpecializedDocument;
            html = Document.GenerateHTML(OriginalEntity);

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(ShowHTML), html);
        }

        private void DisplayDocument(object sender, RunWorkerCompletedEventArgs e)
        {
            // This function typically runs when RenderDocument completes 
        }

        private void ShowHTML(string html)
        {
            TheBrowser.LoadHtml(html, "file:///customreport/");
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

                Document.PrintMetric = IsMetric == true;

                new PdfCreationWindow(outputFile, Document.GenerateHTML(OriginalEntity)).Run();
            }
            catch (Exception ex)
            {
                NoticeWindow.ShowError("Problem saving to PDF", ex.ToString());
            }
        }

        private void Excel_Click(object sender, RoutedEventArgs e)
        {
            if (Excel == null)
            {
                NoticeWindow.ShowMessage("Not supported", "Sorry, there is no excel output available for this report yet.");
                return;
            }

            var dialog = new SaveFileDialog();

            dialog.FileName = Filename;
            dialog.DefaultExt = "xlsx";
            dialog.Filter = "Microsoft Excel (.xlsx)|*.xlsx";

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                var outputFile = dialog.FileName;

                Excel.PrintMetric = IsMetric == true;

                
                new ExcelCreationWindow(outputFile, Excel.GenerateExcel(outputFile, OriginalEntity)).Run();
                //new PdfCreationWindow(outputFile, Document.GenerateHTML(OriginalEntity)).Run();
            }
            catch (Exception ex)
            {
                NoticeWindow.ShowError("Problem saving to Excel", ex.ToString());
            }
        }

        private static string MakeValidFileName( string name )
        {
           string invalidChars = Regex.Escape( new string( Path.GetInvalidFileNameChars() ) );
           string invalidReStr = string.Format( @"[{0}]", invalidChars );
           return Regex.Replace( name, invalidReStr, "_" );
        }
    }
}
