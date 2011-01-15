﻿using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Input;
using SingerDispatch.Controls;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SingerDispatch.Panels.Admin
{
    /// <summary>
    /// Interaction logic for RatesControl.xaml
    /// </summary>
    public partial class RatesControl
    {
        public SingerDispatchDataContext Database { get; set; }
      
        public RatesControl()
        {
            InitializeComponent();

            if (InDesignMode()) return;

            Database = SingerConfigs.CommonDataContext;

            var provider = (ObjectDataProvider)FindResource("RateTypesDropList");

            if (provider != null)
            {
                var types = from rt in Database.RateTypes select rt;
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
            if (InDesignMode()) return;

            dgRates.ItemsSource = new ObservableCollection<Rate>(from r in Database.Rates where r.Archived != true orderby r.RateType.Name, r.Name select r);            
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
    }

    public class RateTypesDropList : ObservableCollection<RateType>
    {
    }
}
