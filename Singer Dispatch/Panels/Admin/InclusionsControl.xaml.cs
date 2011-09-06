using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using SingerDispatch.Controls;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.ComponentModel;

namespace SingerDispatch.Panels.Admin
{
    /// <summary>
    /// Interaction logic for InclusionsControl.xaml
    /// </summary>
    public partial class InclusionsControl
    {
        private BackgroundWorker MainGridWorker;

        public SingerDispatchDataContext Database { get; set; }

        public InclusionsControl()
        {
            InitializeComponent();

            if (InDesignMode()) return;

            Database = SingerConfigs.CommonDataContext;

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

        private void NewInclusion_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<Inclusion>)TheGrid.ItemsSource;
            var inclusion = new Inclusion();

            Database.Inclusions.InsertOnSubmit(inclusion);
            list.Add(inclusion);

            DataGridHelper.EditFirstColumn(TheGrid, inclusion);
        }

        private void RemoveInclusion_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<Inclusion>)TheGrid.ItemsSource;
            var inclusion = (Inclusion)TheGrid.SelectedItem;

            if (inclusion == null) return;

            var confirmation = MessageBox.Show(SingerConfigs.DefaultRemoveItemMessage, "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;

            try
            {
                inclusion.Archived = true;

                Database.SubmitChanges();

                list.Remove(inclusion);
            }
            catch (Exception ex)
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

        private void FillDataGrid()
        {
            if (MainGridWorker.IsBusy)
                return;

            TheGrid.ItemsSource = new ObservableCollection<Inclusion>();
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

            var entities = from i in Database.Inclusions where i.Archived != true orderby i.Line select i;

            foreach (var entity in entities)
            {
                if (async.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                Dispatcher.Invoke(DispatcherPriority.Render, new Action<Inclusion>(AddToGrid), entity);
            }
        }

        private void AddToGrid(Inclusion entity)
        {
            var list = (ObservableCollection<Inclusion>)TheGrid.ItemsSource;

            list.Add(entity);
        }
    }
}
