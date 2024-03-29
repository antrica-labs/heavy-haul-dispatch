﻿using System.Windows;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using SingerDispatch.Controls;
using System;
using SingerDispatch.Printing.Documents;
using System.Collections.ObjectModel;

namespace SingerDispatch.Panels.Invoicing
{
    /// <summary>
    /// Interaction logic for JobInvoicingPanel.xaml
    /// </summary>
    public partial class JobInvoicingPanel
    {
        private CommandBinding SaveCommand { get; set; }

        public SingerDispatchDataContext Database { get; set; }

        public JobInvoicingPanel()
        {
            InitializeComponent();

            Database = SingerConfigs.CommonDataContext;

            SaveCommand = new CommandBinding(CustomCommands.GenericSaveCommand);
            CommandBindings.Add(SaveCommand);

            cmbJobList.Focus();
        }

        private void Panel_Loaded(object sender, RoutedEventArgs e)
        {
            SaveCommand.Executed += CommitJobChanges_Executed;

            RefreshJobList();
        }

        protected override void CompanyListChanged(ObservableCollection<Company> newValue, ObservableCollection<Company> oldValue)
        {
            base.CompanyListChanged(newValue, oldValue);
        }

        protected override void SelectedCompanyChanged(Company newValue, Company oldValue)
        {
            base.SelectedCompanyChanged(newValue, oldValue);

            RefreshJobList();
        }

        private void RefreshJobList()
        {
            cmbJobList.ItemsSource = (SelectedCompany == null) ? null : from j in Database.Jobs where j.Company == SelectedCompany orderby j.Number select j;
        }

        private void ViewJob_Click(object sender, RoutedEventArgs e)
        {
            var job = (Job)cmbJobList.SelectedItem;

            if (job == null) return;
                        
            var window = (MainWindow)Application.Current.MainWindow;
            window.ViewJob(job);
        }

        private void ViewInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedInvoice == null) return;

            var title = String.Format("Invoice #{0}-{1}", SelectedInvoice.Number, SelectedInvoice.Revision);

            var viewer = new Windows.DocumentViewerWindow(new InvoiceDocument(), SelectedInvoice, title) { IsMetric = !UseImperialMeasurements, IsSpecializedDocument = SelectedCompany.CustomerType.IsEnterprise != true };
            viewer.DisplayPrintout();
        }

        private void CommitInvoiceChanges_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedInvoice == null) return;

            try
            {
                ((ButtonBase)sender).Focus();                

                Database.SubmitChanges();

                lblSavedStatus.Content = "Saved";
            }
            catch (Exception ex)
            {
                Windows.NoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }

        private void CommitChangesButton_LostFocus(object sender, RoutedEventArgs e)
        {
            lblSavedStatus.Content = "";
        }

        private void CommitJobChanges_Executed(object sender, ExecutedRoutedEventArgs e)
        {            
            CommitChangesButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, CommitChangesButton));
        }

        
    }
}
