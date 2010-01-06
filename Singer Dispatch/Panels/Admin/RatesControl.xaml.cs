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
            var rate = new Rate();

            Database.Rates.InsertOnSubmit(rate);
            ((ObservableCollection<Rate>)dgRates.ItemsSource).Insert(0, rate);
            dgRates.SelectedItem = rate;

            DataGridHelper.GetCell(dgRates, dgRates.SelectedIndex, 0);
        }

        private void RemoveRate_Click(object sender, RoutedEventArgs e)
        {
            var rate = (Rate)dgRates.SelectedItem;

            if (rate == null)
                return;

            Database.Rates.DeleteOnSubmit(rate);
            ((ObservableCollection<Rate>)dgRates.ItemsSource).Remove(rate);

            Database.SubmitChanges();
        }

        private void RowEditEnding(object sender, Microsoft.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction != DataGridEditAction.Commit)
                return;

            Database.SubmitChanges();            
        }        
    }

    public class RateTypesDropList : ObservableCollection<RateType>
    {
    }
}
