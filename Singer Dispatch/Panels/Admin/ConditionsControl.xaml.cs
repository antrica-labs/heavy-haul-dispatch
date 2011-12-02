using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Input;
using SingerDispatch.Controls;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Windows.Threading;
using System;

namespace SingerDispatch.Panels.Admin
{
    /// <summary>
    /// Interaction logic for ConditionsControl.xaml
    /// </summary>
    public partial class ConditionsControl
    {
        private BackgroundWorker MainGridWorker;

        public SingerDispatchDataContext Database { get; set; }

        public ConditionsControl()
        {
            InitializeComponent();

            if (InDesignMode()) return;

            Database = new SingerDispatchDataContext();

            MainGridWorker = new BackgroundWorker();
            MainGridWorker.WorkerSupportsCancellation = true;
            MainGridWorker.DoWork += FillDataGridAsync;

            RegisterThread(MainGridWorker);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode() || IsVisible == false) return;

            FillDataGrid();
        }

        private void NewCondition_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<Condition>)TheGrid.ItemsSource;
            var condition = new Condition();

            Database.Conditions.InsertOnSubmit(condition);
            list.Add(condition);

            DataGridHelper.EditFirstColumn(TheGrid, condition);
        }

        private void RemoveCondition_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<Condition>)TheGrid.ItemsSource;
            var condition = (Condition)TheGrid.SelectedItem;

            if (condition == null) return;

            var confirmation = MessageBox.Show(SingerConfigs.DefaultRemoveItemMessage, "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            try
            {
                condition.Archived = true;
                
                Database.SubmitChanges();

                list.Remove(condition);
            }
            catch (System.Exception ex)
            {
                Windows.NoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }

        private void CommitChanges()
        {
            try
            {
                Database.SubmitChanges();
            }
            catch (System.Exception ex)
            {
                Windows.NoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);
            }
        }

        private void RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                CommitChanges();
            }
        }

        private void SetDataGridAvailability(bool isAvailable)
        {
            TheGrid.IsEnabled = isAvailable;
        }

        private void FillDataGrid()
        {
            if (MainGridWorker.IsBusy)
                return;

            TheGrid.ItemsSource = new ObservableCollection<Condition>();
            MainGridWorker.RunWorkerAsync();
        }

        private void FillDataGridAsync(object sender, DoWorkEventArgs e)
        {
            var async = sender as BackgroundWorker;

            if (async.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            Dispatcher.Invoke(DispatcherPriority.Render, new Action<bool>(SetDataGridAvailability), false);

            var entities = (from c in Database.Conditions where c.Archived != true orderby c.ID select c).ToList();

            foreach (var entity in entities)
            {
                if (async.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                Dispatcher.Invoke(DispatcherPriority.Render, new Action<Condition>(AddToGrid), entity);
            }

            Dispatcher.Invoke(DispatcherPriority.Render, new Action<bool>(SetDataGridAvailability), true);
        }

        private void AddToGrid(Condition entity)
        {
            var list = (ObservableCollection<Condition>)TheGrid.ItemsSource;

            list.Add(entity);
        }
        
    }
}
