using System;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using SingerDispatch.Database;
using SingerDispatch.Windows;

namespace SingerDispatch.Panels.Jobs
{
    /// <summary>
    /// Interaction logic for DispatchesControl.xaml
    /// </summary>
    public partial class DispatchesControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public DispatchesControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;            
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            cmbLoads.ItemsSource = (SelectedJob == null) ? null : SelectedJob.Loads.ToList();
            cmbUnits.ItemsSource = (SelectedJob == null) ? null : from u in Database.Equipment select u;
            cmbServiceTypes.ItemsSource = (SelectedJob == null) ? null : from r in Database.Rates where r.RateType.Name == "Service" select r;
            cmbEmployees.ItemsSource = (SelectedJob == null) ? null : from emp in Database.Employees orderby emp.FirstName, emp.LastName select emp;
        }

        protected override void SelectedJobChanged(Job newValue, Job oldValue)
        {
            base.SelectedJobChanged(newValue, oldValue);

            dgDispatches.ItemsSource = (newValue == null) ? null : new ObservableCollection<Dispatch>(newValue.Dispatches);
        }

        private void NewDispatch_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob == null) return;

            var list = (ObservableCollection<Dispatch>)dgDispatches.ItemsSource;
            var dispatch = new Dispatch { JobID = SelectedJob.ID, MeetingTime = SelectedJob.StartDate, Description = "Supply men and equipment to transport " };

            SelectedJob.Dispatches.Add(dispatch);
            list.Add(dispatch);
            dgDispatches.SelectedItem = dispatch;
            dgDispatches.ScrollIntoView(dispatch);

            try
            {
                EntityHelper.SaveAsNewDispatch(dispatch, Database);

                cmbLoads.Focus();
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
            
        }

        private void DuplicateDispatch_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<Dispatch>)dgDispatches.ItemsSource;
            var dispatch = (Dispatch)dgDispatches.SelectedItem;

            if (dispatch == null)
                return;

            dispatch = dispatch.Duplicate();

            SelectedJob.Dispatches.Add(dispatch);
            list.Add(dispatch);

            dgDispatches.ScrollIntoView(dispatch);
            dgDispatches.SelectedItem = dispatch;

            try
            {
                EntityHelper.SaveAsNewDispatch(dispatch, Database);

                cmbLoads.Focus();
            }
            catch (Exception ex)
            {
                ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void ViewDispatch_Click(object sender, RoutedEventArgs e)
        {
            if (dgDispatches.SelectedItem == null) return;

            var dispatch = (Dispatch)dgDispatches.SelectedItem;

            var viewer = new DocumentViewerWindow();
            viewer.DisplayPrintout(String.Format("Dispatch #{0}", dispatch.Name), dgDispatches.SelectedItem);
        }

        private void RemoveDispatch_Click(object sender, RoutedEventArgs e)
        {
            if (dgDispatches.SelectedItem == null)
            {
                return;
            }

            var confirmation = MessageBox.Show("Are you sure you want to remove this dispatch?", "Remove dispatch", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes)
            {
                return;
            }

            var dispatch = (Dispatch)dgDispatches.SelectedItem;

            SelectedJob.Dispatches.Remove(dispatch);
            ((ObservableCollection<Dispatch>)dgDispatches.ItemsSource).Remove(dispatch);
        }
    }
}
