﻿using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Windows.Controls;
using SingerDispatch.Controls;

namespace SingerDispatch.Panels.Admin
{
    /// <summary>
    /// Interaction logic for RatesControl.xaml
    /// </summary>
    public partial class RatesControl : UserControl
    {
        public SingerDispatchDataContext Database { get; set; }
      
        public RatesControl()
        {
            InitializeComponent();

            Database = SingerConstants.CommonDataContext;

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
            dgRates.ItemsSource = new ObservableCollection<Rate>(from r in Database.Rates select r);
        }

        private void NewRate_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<Rate>)dgRates.ItemsSource;
            var rate = new Rate();

            Database.Rates.InsertOnSubmit(rate);
            list.Add(rate);
            dgRates.SelectedItem = rate;

            DataGridHelper.GetCell(dgRates, dgRates.SelectedIndex, 0);
        }

        private void RemoveRate_Click(object sender, RoutedEventArgs e)
        {
            var list = (ObservableCollection<Rate>)dgRates.ItemsSource;
            var rate = (Rate)dgRates.SelectedItem;

            if (rate == null) return;

            Database.Rates.DeleteOnSubmit(rate);
            list.Remove(rate);

            try
            {
                Database.SubmitChanges();
            }
            catch (System.Exception ex)
            {
                SingerDispatch.Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
            }
        }

        private void RowEditEnding(object sender, Microsoft.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                try
                {
                    Database.SubmitChanges();
                }
                catch (System.Exception ex)
                {
                    SingerDispatch.Windows.ErrorNoticeWindow.ShowError("Error while attempting write changes to database", ex.Message);
                }
            }
        }        
    }

    public class RateTypesDropList : ObservableCollection<RateType>
    {
    }
}
