using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SingerDispatch.Panels.Admin
{
    /// <summary>
    /// Interaction logic for AppConfigsControl.xaml
    /// </summary>
    public partial class AppConfigsControl
    {        
        private BackgroundWorker MainGridWorker;

        public static DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(Configuration), typeof(AppConfigsControl), new PropertyMetadata(null, SelectedItemPropertyChanged));        
        
        public Configuration SelectedItem
        {
            get
            {
                return (Configuration)GetValue(SelectedItemProperty);
            }
            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        public AppConfigsControl()
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

        protected static void SelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (AppConfigsControl)d;

            control.SelectedItemChanged((Configuration)e.NewValue, (Configuration)e.OldValue);
        }

        private void SelectedItemChanged(Configuration newValue, Configuration oldValue)
        {
        }

        private void TheGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var grid = (DataGrid)sender;
            var item = (Configuration)grid.SelectedItem;

            SelectedItem = null;
            SelectedItem = item;

            e.Handled = true;
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

                Database.RevertChanges();
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

            TheGrid.ItemsSource = new ObservableCollection<Configuration>();
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

            var entities = (from c in Database.Configurations orderby c.ID select c).ToList();

            foreach (var entity in entities)
            {
                if (async.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                Dispatcher.Invoke(DispatcherPriority.Render, new Action<Configuration>(AddToGrid), entity);
            }

            Dispatcher.Invoke(DispatcherPriority.Render, new Action<bool>(SetDataGridAvailability), true);
        }

        private void AddToGrid(Configuration entity)
        {
            var list = (ObservableCollection<Configuration>)TheGrid.ItemsSource;

            list.Add(entity);
        }
    }
}
