using System.Windows;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using SingerDispatch.Controls;
using System;

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

            Database = SingerConstants.CommonDataContext;

            SaveCommand = new CommandBinding(CustomCommands.GenericSaveCommand);
            CommandBindings.Add(SaveCommand);

            cmbJobList.Focus();
        }

        private void Panel_Loaded(object sender, RoutedEventArgs e)
        {
            SaveCommand.Executed += CommitJobChanges_Executed;

            RefreshJobList();
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

        private void ViewInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedInvoice == null) return;

            var title = String.Format("Invoice #{0}-{1}", SelectedInvoice.Number, SelectedInvoice.Revision);

            var viewer = new Windows.DocumentViewer();
            viewer.DisplayPrintout(title, SelectedInvoice);
        }

        private void CommitInvoiceChanges_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedInvoice == null) return;

            try
            {
                Database.SubmitChanges();
            }
            catch (Exception ex)
            {
                Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void CommitJobChanges_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CommitChangesButton.Focus();
            CommitChangesButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, CommitChangesButton));
        }
    }
}
