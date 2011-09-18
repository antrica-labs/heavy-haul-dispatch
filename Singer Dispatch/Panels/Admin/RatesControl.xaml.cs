using System.Linq;
using System.Windows;
using System.Windows.Data;
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
    /// Interaction logic for RatesControl.xaml
    /// </summary>
    public partial class RatesControl
    {
        private BackgroundWorker MainGridWorker;        
      
        public RatesControl()
        {
            InitializeComponent();

            if (InDesignMode()) return;

            Database = new SingerDispatchDataContext();

            MainGridWorker = new BackgroundWorker();
            MainGridWorker.WorkerSupportsCancellation = true;
            MainGridWorker.DoWork += FillDataGridAsync;

            RegisterThread(MainGridWorker);

            var provider = (ObjectDataProvider)FindResource("RateTypesDropList");

            if (provider != null)
            {
                var types = (from rt in Database.RateTypes select rt).ToList();
                var list = (RateTypesDropList)provider.Data;

                list.Clear();

                foreach (var type in types)
                {
                    list.Add(type);
                }
            }            
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode() || IsVisible == false) return;

            FillDataGrid();
        }

        private void NewRate_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<Rate>)dgRates.ItemsSource;
            var rate = new Rate();

            Database.Rates.InsertOnSubmit(rate);
            list.Insert(0, rate);

            DataGridHelper.EditFirstColumn(dgRates, rate);
        }

        private void RemoveRate_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<Rate>)dgRates.ItemsSource;
            var rate = (Rate)dgRates.SelectedItem;

            if (rate == null) return;

            var confirmation = MessageBox.Show(SingerConfigs.DefaultRemoveItemMessage, "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;
            
            try
            {
                rate.Archived = true;

                Database.SubmitChanges();

                list.Remove(rate);
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

        private void AddRateToGrid(Rate rate)
        {
            var list = (ObservableCollection<Rate>)dgRates.ItemsSource;

            list.Add(rate);
        }

        private void SetDataGridAvailability(bool isAvailable)
        {
            dgRates.IsEnabled = isAvailable;
        }

        private void FillDataGrid()
        {
            if (MainGridWorker.IsBusy)
                return;

            dgRates.ItemsSource = new ObservableCollection<Rate>();
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

            var rates = from r in Database.Rates where r.Archived != true orderby r.RateType.Name, r.Name select r;

            foreach (var rate in rates)
            {
                if (async.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                Dispatcher.Invoke(DispatcherPriority.Render, new Action<Rate>(AddRateToGrid), rate);
            }

            Dispatcher.Invoke(DispatcherPriority.Render, new Action<bool>(SetDataGridAvailability), true);
        }

    }

    public class RateTypesDropList : ObservableCollection<RateType>
    {
    }
}
