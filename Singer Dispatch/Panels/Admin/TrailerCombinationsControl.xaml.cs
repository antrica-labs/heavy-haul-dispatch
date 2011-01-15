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

namespace SingerDispatch.Panels.Admin
{
    /// <summary>
    /// Interaction logic for TrailerCombinationsControl.xaml
    /// </summary>
    public partial class TrailerCombinationsControl
    {
        public SingerDispatchDataContext Database { get; set; }

        public TrailerCombinationsControl()
        {
            InitializeComponent();

            if (InDesignMode()) return;

            Database = SingerConfigs.CommonDataContext;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (InDesignMode()) return;

            var provider = (ObjectDataProvider)FindResource("RatesDropList");

            if (provider != null)
            {
                var rates = from r in Database.Rates where r.Archived != true && r.RateType.Name == "Trailer" select r;
                var list = (RatesDropList)provider.Data;

                list.Clear();

                foreach (var rate in rates)
                {
                    list.Add(rate);
                }            
            }

            dgCombinations.ItemsSource = new ObservableCollection<TrailerCombination>(from tc in Database.TrailerCombinations where tc.Archived != true orderby tc.Rate.Name select tc);         
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
    }

    public class RatesDropList : ObservableCollection<Rate>
    {
    }
}
