using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Input;
using SingerDispatch.Controls;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Threading;

namespace SingerDispatch.Panels.Admin
{
    /// <summary>
    /// Interaction logic for TrailerCombinationsControl.xaml
    /// </summary>
    public partial class TrailerCombinationsControl
    {
        private BackgroundWorker MainGridWorker;

        public TrailerCombinationsControl()
        {
            InitializeComponent();

            if (InDesignMode()) return;

            Database = new SingerDispatchDataContext();

            MainGridWorker = new BackgroundWorker();
            MainGridWorker.WorkerSupportsCancellation = true;
            MainGridWorker.DoWork += FillDataGridAsync;

            RegisterThread(MainGridWorker);
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode() || IsVisible == false) return;

            var provider = (ObjectDataProvider)FindResource("RatesDropList");

            if (provider != null)
            {
                var rates = (from r in Database.Rates where r.Archived != true && r.RateType.Name == "Trailer" select r).ToList();
                var list = (RatesDropList)provider.Data;

                list.Clear();

                foreach (var rate in rates)
                {
                    list.Add(rate);
                }            
            }

            FillDataGrid();
        }

        protected override void UseImperialMeasurementsChanged(bool value)
        {
            base.UseImperialMeasurementsChanged(value);
        }

        private void NewCombination_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<TrailerCombination>)dgCombinations.ItemsSource;
            var combination = new TrailerCombination();

            try
            {
                combination.Rate = (from r in Database.Rates where r.Archived != true && r.RateType.Name == "Trailer" select r).First();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }

            Database.TrailerCombinations.InsertOnSubmit(combination);
            list.Insert(0, combination);

            DataGridHelper.EditFirstColumn(dgCombinations, combination);
        }

        private void RemoveCombination_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<TrailerCombination>)dgCombinations.ItemsSource;
            var combination = (TrailerCombination)dgCombinations.SelectedItem;

            if (combination == null) return;

            var confirmation = MessageBox.Show(SingerConfigs.DefaultRemoveItemMessage, "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes) return;
                        
            try
            {
                combination.Archived = true;

                Database.SubmitChanges();

                list.Remove(combination);
            }
            catch (Exception ex)
            {
                Windows.NoticeWindow.ShowError("Error while attempting to write changes to database", ex.Message);

                Database.RevertChanges();
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
            dgCombinations.IsEnabled = isAvailable;
        }

        private void FillDataGrid()
        {
            if (MainGridWorker.IsBusy)
                return;

            dgCombinations.ItemsSource = new ObservableCollection<TrailerCombination>();
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

            var combos = (from tc in Database.TrailerCombinations where tc.Archived != true orderby tc.Rate.Name select tc).ToList();

            foreach (var combo in combos)
            {
                if (async.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                Dispatcher.Invoke(DispatcherPriority.Render, new Action<TrailerCombination>(AddToGrid), combo);
            }

            Dispatcher.Invoke(DispatcherPriority.Render, new Action<bool>(SetDataGridAvailability), true);
        }

        private void AddToGrid(TrailerCombination combo)
        {
            var list = (ObservableCollection<TrailerCombination>)dgCombinations.ItemsSource;

            list.Add(combo);
        }
    }

    public class RatesDropList : ObservableCollection<Rate>
    {
    }
}
